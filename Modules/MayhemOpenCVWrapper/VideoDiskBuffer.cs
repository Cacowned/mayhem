using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// Video disk buffer for Mayhem's camera objects. 
    /// By default, the video disk buffer buffers 30s of video frames from each of the cameras active in Mayhem. 
    /// The buffer is organized as a ring buffer, that over 
    /// 
    /// Maintains a ring buffer for caching video frames on the hard drive.
    /// This helps reduce Mayhem's memory footprint, as caching high-res video frames in memory consumes
    /// too much memory. 
    /// 
    /// The space required for caching 600 640x480 bitmaps, which is 30s worth of video at 20fps,  is about 600mb.
    /// </summary>
    internal class VideoDiskBuffer : IDisposable
    {
        private static int instanceCount;
        private readonly string bufferFilename;
        private readonly Queue<Bitmap> writeQueue;
        private FileStream fs;

        // buffer can accept new frames
        private bool acceptFrames;

        // ensures 30s of footage at 20fps
        private readonly int maxBufferItems;

        // max bitmaps allowed in the queue
        private const int MaxQueuedBitmaps = 64;

        // ring buffer index
        private int currentBlock;
        private long[] blockBytes;
        private readonly object operationLocker;
        private Thread writeThread;
        private int writtenBlocks;
        private volatile bool threadRunning = true;

        // when a bitmap is added to the queue, signal the event to let the WriteThread process it 
        private readonly AutoResetEvent signalNewBitmaps;

        internal VideoDiskBuffer()
        {
            instanceCount++;

            operationLocker = new object();
            signalNewBitmaps = new AutoResetEvent(false);

            bufferFilename = "MVidBuf" + instanceCount + ".bin";

            maxBufferItems = (1000 / CameraSettings.Defaults().UpdateRateMs) * 30;
            if (!(maxBufferItems >= 200 && maxBufferItems <= 2000))
                maxBufferItems = 512;

            blockBytes = new long[maxBufferItems];

            writeQueue = new Queue<Bitmap>();
            acceptFrames = true;

            Logger.WriteLine("MayhemVideoBuffer Nr " + instanceCount + " filename " + bufferFilename + " maxBufferItems " + maxBufferItems);
            fs = new FileStream(bufferFilename, FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);
            writeThread = new Thread(BufferWriteThread);
            writeThread.Name = "Video_Disk_Writer";
            writeThread.IsBackground = true;
            writeThread.Start();
        }

        /// <summary>
        /// Clean up the thread! 
        /// </summary>
        ~VideoDiskBuffer()
        {
            ClearAndResetBuffer();
            threadRunning = false;
            signalNewBitmaps.Set();
            if (writeThread.ThreadState == ThreadState.Running)
                writeThread.Join(50);
        }

        /// <summary>
        /// Calculate the offset by summing up all the buffer positions prior to index. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private long GetOffsetForIndex(int index)
        {
            if (index >= blockBytes.Length || index < 0)
                throw new NotSupportedException();
            long offs = 0;
            for (int i = 0; i < index; i++)
            {
                offs += blockBytes[i];
            }

            return offs;
        }

        /// <summary>
        /// Deletes (clears) buffer file from disk 
        /// </summary>
        internal void ClearAndResetBuffer()
        {
            acceptFrames = false;
            {
                lock (operationLocker)
                {
                    fs.Close();
                    if (File.Exists(bufferFilename))
                        File.Delete(bufferFilename);
                    fs = new FileStream(bufferFilename, FileMode.OpenOrCreate);
                    fs.Seek(0, SeekOrigin.Begin);
                    blockBytes = new long[maxBufferItems];

                    // kill the thread while we're at it
                    threadRunning = false;
                    signalNewBitmaps.Set();
                    writeThread.Join(100);

                    // allow restarts of the thread now
                    threadRunning = true;

                    // reset indices and counters
                    writtenBlocks = 0;
                    currentBlock = 0;
                }
            }

            acceptFrames = true;
        }

        /// <summary>
        /// Allows adding of a Bitmap to the disk buffer. 
        /// </summary>
        /// <param name="b"></param>
        internal void AddBitmap(Bitmap b)
        {
            if (acceptFrames)
            {
                if (writeThread.ThreadState == ThreadState.Stopped)
                {
                    ClearAndResetBuffer();

                    // restart the thread
                    writeThread = new Thread(BufferWriteThread);
                    writeThread.Name = "Video_Disk_Writer";
                    writeThread.Start();
                }

                if (writeQueue.Count < MaxQueuedBitmaps)
                    writeQueue.Enqueue(b);

                // tell the thread to process incoming data
                signalNewBitmaps.Set();
            }
        }

        /// <summary>
        /// Thread that writes pending items to the buffer file. 
        /// </summary>
        private void BufferWriteThread()
        {
            while (threadRunning)
            {
                if (writeQueue.Count > 0)
                {
                    Bitmap bitmap = writeQueue.Dequeue();
                    MemoryStream ms = new MemoryStream();
                    try
                    {
                        bitmap.Save(ms, ImageFormat.Bmp);
                        long nextOffset = GetOffsetForIndex(currentBlock);

                        // lock access to the file
                        lock (operationLocker)
                        {
                            // seek to correct position
                            fs.Seek(nextOffset, SeekOrigin.Begin);

                            // serialize object in memory
                            byte[] memBytes = ms.GetBuffer();
                            fs.Write(memBytes, 0, memBytes.Length);
                            blockBytes[currentBlock] = memBytes.Length;
                        }

                        writtenBlocks++;

                        // update ring buffer index
                        currentBlock = (currentBlock + 1) % maxBufferItems;
                    }
                    finally
                    {
                        bitmap.Dispose();
                        ms.Dispose();
                    }
                }

                // block thread until signaled (when new data is placed on the queue) 
                signalNewBitmaps.WaitOne();
            }
        }

        /// <summary>
        /// Retrieve List of bitmaps from the disk cache and deserialize them to memory. 
        /// </summary>
        /// <returns>List containing the Bitmaps from the disk cache.</returns>
        internal List<Bitmap> RetrieveBitmapsFromDisk()
        {
            List<Bitmap> bitmaps = new List<Bitmap>();

            // abort if we're currently not doing stuff
            if (!acceptFrames)
                return bitmaps;

            lock (operationLocker)
            {
                int begin, end;
                if (writtenBlocks >= maxBufferItems)
                {
                    begin = currentBlock - maxBufferItems;
                    end = currentBlock;
                }
                else
                {
                    begin = 0;
                    end = currentBlock;
                }

                // fix C# modulo implementation! 
                int blck = begin % maxBufferItems;
                if (blck < 0)
                    blck += maxBufferItems;

                for (int i = begin; i < end; i++)
                {
                    long offset = GetOffsetForIndex(blck);
                    int byteCount = (int)blockBytes[blck];
                    Logger.WriteLine("Deserializing Block " + blck + " offset " + string.Format("{0:x}", offset) + " nr of bytes " + byteCount);
                    byte[] bytes = new byte[byteCount];
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Read(bytes, 0, byteCount);
                    MemoryStream ms = new MemoryStream(bytes);
                    try
                    {
                        Image img = Image.FromStream(ms);
                        bitmaps.Add(new Bitmap(img));
                        blck = (blck + 1) % maxBufferItems;
                    }
                    finally
                    {
                        ms.Dispose();
                    }
                }
            }

            return bitmaps;
        }

        public void Dispose()
        {
            signalNewBitmaps.Dispose();
            fs.Dispose();
        }
    }
}

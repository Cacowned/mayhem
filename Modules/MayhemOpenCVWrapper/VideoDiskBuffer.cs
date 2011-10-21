/*
 *  VideoDiskBuffer.cs
 * 
 *  Maintains a ring buffer for caching video frames on the hard drive.
 *  This helps reduce Mayhem's memory footprint, as caching high-res video frames in memory consumes
 *  too much memory. 
 *  
 * The space required for caching 600 640x480 bitmaps, which is 30s worth of video at 20fps,  is about 600mb.
 *  
 *  (c) 2011, Microsoft Applied Sciences Group
 *  
 *  Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;
using MayhemCore;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace MayhemOpenCVWrapper
{

    /// <summary>
    /// Video disk buffer for Mayhem's camera objects. 
    /// By default, the video disk buffer buffers 30s of video frames from each of the cameras active in Mayhem. 
    /// The buffer is organized as a ring buffer, that over 
    /// </summary>
    internal class VideoDiskBuffer
    {
        private static int inst_cnt = 0; 
        private readonly string BUFFER_FILENAME;
        private Queue<Bitmap> writeQueue = new Queue<Bitmap>();
        private FileStream fs;
        // buffer can accept new frames
        private bool accept_frames = true;                 
        // ensures 30s of footage at 20fps
        private int MAX_BUFFER_ITEMS_ = (1000 / CameraSettings.Defaults().UpdateRateMs) * 30 ;          
        private readonly int MAX_BUFFER_ITEMS;
        // max bitmaps allowed in the queue
        private const int MAX_QUEUED_BITMAPS = 64;          
        // ring buffer index
        private int currentBlock = 0;                      
        private long[] block_bytes;         
        private object operation_locker = new object();
        private Thread write_thread;
        private int writtenBlocks = 0;
        private volatile bool threadRunning = true;
        // when a bitmap is added to the queue, signal the event to let the WriteThread process it 
        private AutoResetEvent signalNewBitmaps = new AutoResetEvent(false);            

        internal VideoDiskBuffer()
        {
            inst_cnt++;
            string bufferfileName_ = "MVidBuf" + inst_cnt + ".bin";
            BUFFER_FILENAME = bufferfileName_;
            if (MAX_BUFFER_ITEMS_ >= 200 && MAX_BUFFER_ITEMS_ <= 2000)
                MAX_BUFFER_ITEMS = MAX_BUFFER_ITEMS_;
            else
                MAX_BUFFER_ITEMS = 512; 
            block_bytes = new long[MAX_BUFFER_ITEMS];
            Logger.WriteLine("MayhemVideoBuffer Nr " + inst_cnt + " filename " + BUFFER_FILENAME + " MAX_BUFFER_ITEMS " + MAX_BUFFER_ITEMS);
            fs = new FileStream(BUFFER_FILENAME, FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);
            write_thread = new Thread(new ThreadStart(this.BufferWriteThread));
            write_thread.Name = "Video_Disk_Writer";
            write_thread.IsBackground = true;
            write_thread.Start();
        }

        /// <summary>
        /// Clean up the thread! 
        /// </summary>
        ~VideoDiskBuffer()
        {
            Logger.WriteLine("Dtor");
            ClearAndResetBuffer();
            threadRunning = false;
            signalNewBitmaps.Set();
            if (write_thread.ThreadState == ThreadState.Running)
                write_thread.Join(50);
        }


        /// <summary>
        /// Calculate the offset by summing up all the buffer positions prior to index. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private long GetOffsetForIndex(int index)
        {
            if (index >= block_bytes.Length || index < 0)
                throw new NotSupportedException();
            long offs = 0;
            for (int i = 0; i < index; i++)
            {
                offs += block_bytes[i];
            }
            return offs;
        }


        /// <summary>
        /// Deletes (clears) buffer file from disk 
        /// </summary>
        internal void ClearAndResetBuffer()
        {
            accept_frames = false;
            {
                lock (operation_locker)
                {
                    fs.Close();
                    if (File.Exists(BUFFER_FILENAME))
                        File.Delete(BUFFER_FILENAME);
                    fs = new FileStream(BUFFER_FILENAME, FileMode.OpenOrCreate);
                    fs.Seek(0, SeekOrigin.Begin);
                    block_bytes = new long[MAX_BUFFER_ITEMS]; 
                    // kill the thread while we're at it
                    threadRunning = false;
                    signalNewBitmaps.Set();
                    write_thread.Join(100);
                    // allow restarts of the thread now
                    threadRunning = true;
                    // reset indices and counters
                    writtenBlocks = 0;
                    currentBlock = 0;
                    
                }
            }
            accept_frames = true; 
        }
      
        /// <summary>
        /// Allows adding of a Bitmap to the disk buffer. 
        /// </summary>
        /// <param name="b"></param>
        internal void AddBitmap(Bitmap b)
        {
            if (accept_frames)
            {
                if (write_thread.ThreadState == ThreadState.Stopped)
                {
                    ClearAndResetBuffer();
                    //restart the thread
                    write_thread = new Thread(new ThreadStart(this.BufferWriteThread));
                    write_thread.Name = "Video_Disk_Writer";
                    write_thread.Start();
                }
                if (writeQueue.Count < MAX_QUEUED_BITMAPS)
                    writeQueue.Enqueue(b);
                // tell the thread to process incoming data
                signalNewBitmaps.Set();
            }
        }
       
        /// <summary>
        /// Thread that writes pending items to the buffer file. 
        /// </summary>
        /// <param name="items"></param>
        private void BufferWriteThread()
        {
          // Logger.WriteLine("");
            
            while (threadRunning)
            {
                if (writeQueue.Count > 0)
                {
                    Bitmap bitmap = writeQueue.Dequeue();
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Bmp);
                    long next_offset = GetOffsetForIndex(currentBlock);
                    //Logger.WriteLine("Writing to Block " + currentBlock + " Offset " + string.Format("{0:x}", next_offset));
                    // lock access to the file
                    lock (operation_locker)
                    {
                        // seek to correct position
                        fs.Seek(next_offset, SeekOrigin.Begin);
                        // serialize object in memory
                        byte[] memBytes = ms.GetBuffer();
                        fs.Write(memBytes, 0, memBytes.Length);
                        block_bytes[currentBlock] = memBytes.Length;
                    }
                    writtenBlocks++;
                    // update ring buffer index
                    currentBlock = (currentBlock + 1) % MAX_BUFFER_ITEMS;
                    //fs.Flush();
                    bitmap.Dispose();
                    ms.Dispose();
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
            Logger.WriteLine("");
            List<Bitmap> bitmaps = new List<Bitmap>();

            // abort if we're currently not doing stuff
            if (!accept_frames)
                return bitmaps;

            lock (operation_locker)
            {               
                int begin, end; 
                if (writtenBlocks >= MAX_BUFFER_ITEMS)
                {
                     begin = currentBlock - MAX_BUFFER_ITEMS;
                     end = currentBlock;
                }
                else
                {
                    begin = 0;
                    end = currentBlock;
                }

                // fix C# modulo implementation! 
                int blck = begin % MAX_BUFFER_ITEMS;
                if (blck < 0)
                    blck += MAX_BUFFER_ITEMS;
        
                for (int i = begin; i < end; i++)
                {                   
                    long offset = GetOffsetForIndex(blck);
                    int byteCount = (int)block_bytes[blck];
                    Logger.WriteLine("Deserializing Block " + blck + " offset "+ string.Format("{0:x}", offset) + " nr of bytes " + byteCount);
                    byte[] bytes = new byte[byteCount];
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Read(bytes, 0, byteCount);
                    MemoryStream ms = new MemoryStream(bytes);
                    Image img = Image.FromStream(ms);
                    bitmaps.Add(new Bitmap(img));
                    blck = (blck + 1) % MAX_BUFFER_ITEMS;
                    ms.Dispose();
                }
            }
            return bitmaps;
        }
    }
}

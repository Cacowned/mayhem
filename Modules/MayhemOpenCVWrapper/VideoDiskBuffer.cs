/*
 *  VideoDiskBuffer.cs
 * 
 *  Maintains a ring buffer for caching video frames on the hard drive.
 *  This helps reduce Mayhem's memory footprint, as caching high-res video frames in memory consumes
 *  too much memory. 
 *  
 *  Current buffer size on the disk for the chosen block size (1048576 bytes) is about 1GB. 
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
    public class VideoDiskBuffer
    {
        private static int inst_cnt = 0; 
        private readonly string BUFFER_FILENAME;
        private Queue<Bitmap> writeQueue = new Queue<Bitmap>();
        private FileStream fs;
        //private bool running = true;
        private int blockSize = 1048576;                     // 1MB
        private const int MAX_BLOCKS = 512;
        private int currentBlock = 0;                       // ring buffer index
        private long[] block_bytes = new long[MAX_BLOCKS];        
        private object operation_locker = new object();
        private Thread write_thread;
        private int writtenBlocks = 0; 


        public VideoDiskBuffer()
        {
            inst_cnt++;
            string bufferfileName_ = "MayhemVideoBuffer" + inst_cnt + ".bin";
            BUFFER_FILENAME = bufferfileName_;
            fs = new FileStream(BUFFER_FILENAME, FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);
            write_thread = new Thread(new ThreadStart(this.BufferWriteThread));
            write_thread.Name = "Video_Disk_Writer";
        }

        /// <summary>
        /// Deletes (clears) buffer file from disk 
        /// </summary>
        public void ClearAndResetBuffer()
        {
            fs.Close();
            if (File.Exists(BUFFER_FILENAME))
                File.Delete(BUFFER_FILENAME);
            fs = new FileStream(BUFFER_FILENAME, FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);

            // reset indices and counters
            writtenBlocks = 0;
            currentBlock = 0; 
        }
      
        public void AddBitmap(Bitmap b)
        {
            writeQueue.Enqueue(b);
            if (write_thread.ThreadState != ThreadState.Running)
            {
                if (write_thread.ThreadState == ThreadState.Unstarted)
                {
                    if (fs.CanWrite)
                    {
                        write_thread.Start();
                    }
                }
                else if (write_thread.ThreadState == ThreadState.Stopped)
                {
                    write_thread = new Thread(new ThreadStart(this.BufferWriteThread));
                    write_thread.Name = "Video_Disk_Writer";
                    if (fs.CanWrite)
                    {
                        write_thread.Start();
                    }
                }             
            }         
        }
       
        /// <summary>
        /// Write to buffer file. 
        /// </summary>
        /// <param name="items"></param>
        public void BufferWriteThread()
        {
            Logger.WriteLine("");
            
            while (writeQueue.Count> 0)
            {
                Bitmap bitmap = writeQueue.Dequeue();
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Bmp);
                
                long next_offset = currentBlock * blockSize;
                Logger.WriteLine("Writing to Block " + currentBlock + " Offset " + string.Format("{0:x}", next_offset));
                // lock access to the file
                lock (operation_locker)
                {
                    // seek to correct position
                    fs.Seek(next_offset, SeekOrigin.Begin);
                    // serialize object in memory
                    byte[] memBytes = ms.GetBuffer();
                    fs.Write(memBytes, 0, memBytes.Length);
                    block_bytes[currentBlock] = memBytes.Length;
                    writtenBlocks++;
                }
                // update ring buffer index
                currentBlock = (currentBlock + 1) % MAX_BLOCKS;
                //fs.Flush();
                bitmap.Dispose(); 
            }
        }

        public List<Bitmap> RetrieveBitmapsFromDisk()
        {
            Logger.WriteLine("");
            List<Bitmap> bitmaps = new List<Bitmap>();
            lock (operation_locker)
            {               
                int begin, end; 
                if (writtenBlocks >= MAX_BLOCKS)
                {
                     begin = currentBlock - MAX_BLOCKS;
                     end = currentBlock;
                }
                else
                {
                    begin = 0;
                    end = currentBlock;
                }

                // fix C# modulo implementation! 
                int blck = begin % MAX_BLOCKS;
                if (blck < 0)
                    blck += MAX_BLOCKS;

           
                for (int i = begin; i < end; i++)
                {
                    long offset = blck * blockSize;
                    int byteCount = (int)block_bytes[blck];
                    Logger.WriteLine("Deserializing Block " + blck + " offset " + offset + " nr of bytes " + byteCount);
                    byte[] bytes = new byte[byteCount];
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Read(bytes, 0, byteCount);
                    MemoryStream ms = new MemoryStream(bytes);
                    Image img = Image.FromStream(ms);
                    bitmaps.Add(new Bitmap(img));
                    blck = (blck + 1) % MAX_BLOCKS;
                }
            }
            return bitmaps;
        }
    }
}

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
        private readonly string bufferfileName;
        private Queue<Bitmap> writeQueue = new Queue<Bitmap>();
        private FileStream fs;
        private bool running = true;
        private int blockSize = 1048576;        // 1MB
        private const int max_blocks = 512;
        private int currentBlock = 0; 
        private long[] block_bytes = new long[max_blocks];        
        private object operation_locker = new object();
        private Thread write_thread;
        private int writtenBlocks = 0; 


        public VideoDiskBuffer()
        {
            inst_cnt++;
            string bufferfileName_ = "MayhemVideoBuffer" + inst_cnt + ".bin";
            bufferfileName = bufferfileName_;
            fs = new FileStream(bufferfileName, FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);
            write_thread = new Thread(new ThreadStart(this.BufferWriteThread));
            write_thread.Name = "Video_Disk_Writer";
        }

      

        public void AddBitmap(Bitmap b)
        {
            writeQueue.Enqueue(b);
            if (write_thread.ThreadState != ThreadState.Running)
            {
                if (write_thread.ThreadState == ThreadState.Unstarted)
                {
                    write_thread.Start();
                }
                else if (write_thread.ThreadState == ThreadState.Stopped)
                {
                    write_thread = new Thread(new ThreadStart(this.BufferWriteThread));
                    write_thread.Name = "Video_Disk_Writer";
                    write_thread.Start();
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
                currentBlock = (currentBlock + 1) % max_blocks;
                //fs.Flush();
                bitmap.Dispose(); 
            }
        }

        public List<Bitmap> RetrieveBitmapsFromDisk()
        {
            Logger.WriteLine("");
            List<Bitmap> bitmaps = new List<Bitmap>();

            int begin, end; 
            if (writtenBlocks >= max_blocks)
            {
                 begin = currentBlock - max_blocks;
                 end = currentBlock;
            }
            else
            {
                begin = 0;
                end = currentBlock;
            }

            int blck = begin % max_blocks;
            if (blck < 0)
                blck += max_blocks;

            for (int i = begin; i < end; i++)
            {     
                long offset = blck * blockSize;
                int byteCount = (int) block_bytes[blck];
                Logger.WriteLine("Deserializing Block " + blck + " offset " + offset + " nr of bytes " + byteCount);
                byte[] bytes = new byte[byteCount];
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Read(bytes, 0,  byteCount);
                MemoryStream ms = new MemoryStream(bytes);
                Image img = Image.FromStream(ms);
                bitmaps.Add(new Bitmap(img));
                blck = (blck + 1) % max_blocks;
            }
            return bitmaps;
        }
    }
}

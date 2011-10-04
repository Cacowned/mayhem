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

namespace MayhemOpenCVWrapper
{
    public class VideoDiskBuffer
    {
        private static int inst_cnt = 0; 
        private readonly string bufferfileName;

        private ObservableCollection<BitmapTimestamp> writeQueue = new ObservableCollection<BitmapTimestamp>();

        private FileStream fs;

        private bool running = true;

        private int blockSize = 1048576;        // 1MB
        private int blocks = 512;

        private int currentBlock = 0; 
        private Dictionary<int,long> block_bytes = new Dictionary<int,long>();
         
        private object operation_locker = new object();

        public VideoDiskBuffer()
        {
            inst_cnt++;
            string bufferfileName_ = "MayhemVideoBuffer" + inst_cnt + ".bin";
            bufferfileName = bufferfileName_;
            fs = new FileStream(bufferfileName, FileMode.CreateNew);
            writeQueue.CollectionChanged += new NotifyCollectionChangedEventHandler(writeQueue_CollectionChanged);
            
        }

        void writeQueue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                IList<BitmapTimestamp> newItems = e.NewItems as IList<BitmapTimestamp>;
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.BufferWriteThread), newItems);
            }
        }

        public void AddBitmap(BitmapTimestamp b)
        {
            writeQueue.Add(b);
        }
       
        /// <summary>
        /// Write to buffer file. 
        /// </summary>
        /// <param name="items"></param>
        public void BufferWriteThread(object items)
        {
            Logger.WriteLine("");
            IList<BitmapTimestamp> newImages = items as IList<BitmapTimestamp>;
            while (writeQueue.Count > 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                BitmapTimestamp bitmap = writeQueue[0];

                // serialize object in memory
                bf.Serialize(ms, bitmap);
                writeQueue.RemoveAt(1);
                Logger.WriteLine("Writing to Block " + currentBlock);
                long next_offset = currentBlock * blockSize;
                // lock access to the file
                lock (operation_locker)
                {
                    // seek to correct position
                    fs.Seek(next_offset, 0);
                    ms.CopyTo(fs);
                    block_bytes[currentBlock] = ms.Length;
                }
                currentBlock++;
            }
        }

        public List<BitmapTimestamp> RetrieveBitmapsFromDisk()
        {
            Logger.WriteLine("");
            List<BitmapTimestamp> bitmaps = new List<BitmapTimestamp>();

            for (int blck = 0; blck < currentBlock; blck++)
            {
                long offset = blck * blockSize;
                long byteCount = block_bytes[blck];
                Logger.WriteLine("Deserializing Block " + blck + " offset " + offset);
                BinaryFormatter bf = new BinaryFormatter();
                fs.Seek(offset, 0);
                byte[] bytes = new byte[byteCount];
                fs.Read(bytes,0,  (int) byteCount);
                MemoryStream ms = new MemoryStream();
                ms.Write(bytes, 0, (int) byteCount);
                BitmapTimestamp bitmap = (BitmapTimestamp) bf.Deserialize(ms);
                bitmaps.Add(bitmap);
            }
            return bitmaps;
        }
    }
}

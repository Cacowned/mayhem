using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MayhemVisionModules;

//this cover flow control us based on the tutorial at http://d3dal3.blogspot.com/2009/04/wpf-cover-flow-tutorial-part-7-source.html

namespace MayhemVisionModules.Wpf
{
  
    /// <summary>
    /// Interaction logic for CoverFlowControl.xaml
    /// </summary>
    public partial class CoverFlowControl : UserControl
    {
        public CoverFlowControl()
        {
            InitializeComponent();
        }

        public int Index
        {
            get { return index; }
            set { UpdateIndex(value); }
        }

        public void Add(ImageRenderer renderer)
        {
            imageList.Add(imageList.Count, renderer);
            UpdateRange(index);
        }

        public int Count
        {
            get { return imageList.Count; }
        }

        public void GoToPreviousPage()
        {
            UpdateIndex(Math.Max(index - PageSize, 0));
        }

        public void GoToNext()
        {
            UpdateIndex(Math.Min(index + 1, imageList.Count - 1));
        }

        public void GoToPrevious()
        {
            UpdateIndex(Math.Max(index - 1, 0));
        }

        public void GoToNextPage()
        {
            UpdateIndex(Math.Min(index + PageSize, imageList.Count - 1));
        }

        protected int FirstRealizedIndex
        {
            get { return firstRealized; }
        }
        protected int LastRealizedIndex
        {
            get { return lastRealized; }
        }

        private void RotateCover(int pos, bool animate)
        {
            if (coverList.ContainsKey(pos))
                coverList[pos].Animate(index, animate);
        }

        private void UpdateIndex(int newIndex)
        {
            if (index != newIndex)
            {
                bool animate = Math.Abs(newIndex - index) < PageSize;
                UpdateRange(newIndex);
                int oldIndex = index;
                index = newIndex;
                if (index > oldIndex)
                {
                    if (oldIndex < firstRealized)
                        oldIndex = firstRealized;
                    for (int i = oldIndex; i <= index; i++)
                        RotateCover(i, animate);
                }
                else
                {
                    if (oldIndex > lastRealized)
                        oldIndex = lastRealized;
                    for (int i = oldIndex; i >= index; i--)
                        RotateCover(i, animate);
                }
                camera.Position = new Point3D(CoverFlowElement.CoverStep * index, camera.Position.Y, camera.Position.Z);
            }
        }

        private void UpdateRange(int newIndex)
        {
            int newFirstRealized = Math.Max(newIndex - HalfRealizedCount, 0);
            int newLastRealized = Math.Min(newIndex + HalfRealizedCount, imageList.Count - 1);
            if (lastRealized < newFirstRealized || firstRealized > newLastRealized)
            {
                visualModel.Children.Clear();
                coverList.Clear();
            }
            else if (firstRealized < newFirstRealized)
            {
                for (int i = firstRealized; i < newFirstRealized; i++)
                    RemoveCover(i);
            }
            else if (newLastRealized < lastRealized)
            {
                for (int i = lastRealized; i > newLastRealized; i--)
                    RemoveCover(i);
            }
            for (int i = newFirstRealized; i <= newLastRealized; i++)
            {
                if (!coverList.ContainsKey(i))
                {
                    CoverFlowElement cover = new CoverFlowElement(imageList[i], i, newIndex, visualModel);
                    coverList.Add(i, cover);
                }
            }
            firstRealized = newFirstRealized;
            lastRealized = newLastRealized;
        }

        private void RemoveCover(int pos)
        {
            if (!coverList.ContainsKey(pos))
                return;
            coverList[pos].Destroy();
            coverList.Remove(pos);
        }

        #region Fields
        public const int HalfRealizedCount = 7;
        public const int PageSize = HalfRealizedCount;
        private readonly Dictionary<int, ImageRenderer> imageList = new Dictionary<int, ImageRenderer>();
        private readonly Dictionary<string, int> labelIndex = new Dictionary<string, int>();
        private readonly Dictionary<int, string> indexLabel = new Dictionary<int, string>();
        private readonly Dictionary<int, CoverFlowElement> coverList = new Dictionary<int, CoverFlowElement>();
        private int index;
        private int firstRealized = -1;
        private int lastRealized = -1;
        #endregion

    }
}

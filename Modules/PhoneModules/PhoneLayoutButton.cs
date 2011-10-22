namespace PhoneModules
{
    public class PhoneLayoutButton : PhoneLayoutElement
    {
        public string Text
        {
            get;
            set;
        }

        private string imageFile;

        public string ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                imageFile = value;
                if (!string.IsNullOrEmpty(imageFile))
                {
                    ImageBytes = FileDictionary.Get(imageFile);
                }
            }
        }

        public byte[] ImageBytes
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public PhoneLayoutButton()
        {
            Text = string.Empty;
            imageFile = string.Empty;
            ImageBytes = new byte[0];
            IsEnabled = true;
        }
    }
}

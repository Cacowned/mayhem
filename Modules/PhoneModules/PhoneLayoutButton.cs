namespace PhoneModules
{
    public class PhoneLayoutButton : PhoneLayoutElement
    {
        public string Text = string.Empty;
        private string imageFile = string.Empty;

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

        public byte[] ImageBytes = new byte[0];
        public bool IsEnabled = true;
    }
}

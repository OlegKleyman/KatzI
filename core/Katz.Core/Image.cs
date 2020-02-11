namespace Katz.Core
{
    public class Image
    {
        public Image(string mimeType, byte[] value)
        {
            MimeType = mimeType;
            Value = value;
        }

        public string MimeType { get; }

        public byte[] Value { get; }
    }
}
namespace Katz.Data.Entities
{
    public class Book
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }

        public int Id { get; set; }

        public byte[] Image { get; set; }

        public string ImageMimeType { get; set; }

        public string Series { get; set; }
    }
}
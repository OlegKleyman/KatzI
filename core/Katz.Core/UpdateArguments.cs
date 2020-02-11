namespace Katz.Core
{
    public class UpdateArguments
    {
        public string Author { get; set; }

        public string Description { get; set; }

        public Image Image { get; set; }

        public Rating? Rating { get; set; }

        public string Series { get; set; }

        public string Title { get; set; }
    }
}
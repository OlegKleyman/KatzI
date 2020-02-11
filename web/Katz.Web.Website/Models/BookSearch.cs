namespace Katz.Web.Website.Models
{
    public class BookSearch
    {
        public string Author { get; set; }

        public string Title { get; set; }

        public Rating? Rating { get; set; }

        public string Series { get; set; }
    }
}
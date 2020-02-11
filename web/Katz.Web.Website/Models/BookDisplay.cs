namespace Katz.Web.Website.Models
{
    public class BookDisplay
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }

        public Image Image { get; set; }

        public string Series { get; set; }

        public BookDisplay[] RelatedBooks { get; set; }
    }
}
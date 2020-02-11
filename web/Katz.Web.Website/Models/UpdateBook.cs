using Microsoft.AspNetCore.Http;

namespace Katz.Web.Website.Models
{
    public class UpdateBook
    {
        public string Author { get; set; }

        public string Title { get; set; }

        public Rating Rating { get; set; }

        public string Series { get; set; }

        public string Description { get; set; }

        public IFormFile Image { get; set; }

        public Image DisplayImage { get; set; }

        public int Id { get; set; }
    }
}
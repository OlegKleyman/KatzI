using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Katz.Web.Website.Models
{
    public class AddBook
    {
        [Required]
        public string Author { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public Rating Rating { get; set; }

        public string Series { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
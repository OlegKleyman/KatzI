namespace Katz.Core
{
    public class BookInformation
    {
        public BookInformation(Rating rating, string description, Image image)
        {
            Rating = rating;
            Description = description;
            Image = image;
        }

        public Rating Rating { get; }

        public string Description { get; }

        public Image Image { get; set; }
    }
}
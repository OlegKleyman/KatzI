namespace Katz.Core
{
    public class BookDetail
    {
        public BookDetail(string title, string author, string series)
        {
            Title = title;
            Author = author;
            Series = series;
        }

        public string Title { get; }

        public string Author { get; }

        public string Series { get; }
    }
}
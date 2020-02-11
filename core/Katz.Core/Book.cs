namespace Katz.Core
{
    public class Book
    {
        public Book(int id, BookDetail detail, BookInformation info)
        {
            Id = id;
            Detail = detail;
            Info = info;
        }

        public int Id { get; }

        public BookDetail Detail { get; }

        public BookInformation Info { get; }
    }
}
namespace Katz.Core
{
    public class AddArguments
    {
        public AddArguments(BookDetail detail, BookInformation info)
        {
            Detail = detail;
            Info = info;
        }

        public BookDetail Detail { get; }

        public BookInformation Info { get; }
    }
}
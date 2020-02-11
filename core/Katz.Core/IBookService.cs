using System.Threading.Tasks;

namespace Katz.Core
{
    public interface IBookService
    {
        Task<Book> GetAsync(int id);

        Task<Book[]> GetRelatedBooksByAuthorOrSeriesAsync(Book book);

        Task<bool> DeleteAsync(int id);

        Task<Book[]> FindAsync(SearchArguments arguments);

        Task<int> AddAsync(AddArguments arguments);

        Task EditAsync(int id, UpdateArguments arguments);
    }
}
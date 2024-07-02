using bookStoreApi.DTOs;
using bookStoreApi.Entities;

namespace bookStoreApi.Repository
{
    public interface IBookService
    {
        Task<List<Book>>GetBooks();
        Task<Book> GetBookById(int id);
        Task<List<Book>> GetBooksByName(string name);
        Task<Book> CreateBook(BookRequestDTO book);
        Task<List<Book>> GetBooksByAuthor(string name);
        Task<Book> UpdateBook(int id, BookRequestDTO book);
        Task DeleteBook(int id);
    }
}

using Small_Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Small_Library.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<List<Book>> SearchBooksAsync(string searchstring);
        Task<List<Book>> GetAvailableBooksAsync();
        Task<List<Book>> GetBorrowedBooksAsync();
    }
} 
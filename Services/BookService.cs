using Small_Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Small_Library.Repositories;

namespace Small_Library.Services
{
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _repository;

        public BookService(IGenericRepository<Book> repository)
        {
            _repository = repository;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _repository.GetAll();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task AddAsync(Book book)
        {
            await _repository.Add(book);
        }

        public async Task UpdateAsync(Book book)
        {
            await _repository.Update(book);
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _repository.GetById(id);
            if (book != null)
            {
                await _repository.Delete(book);
            }
        }

        public async Task<List<Book>> SearchBooksAsync(string searchstring)
        {
            var books = await _repository.GetAll();
            if (searchstring != null)
                return books.Where(book => book.Title.ToLower().Contains(searchstring.ToLower())).ToList();
            return books;
        }

        public async Task<List<Book>> GetAvailableBooksAsync()
        {
            var books = await _repository.GetAll();
            return books.Where(book => book.Status == "Available").ToList();
        }

        public async Task<List<Book>> GetBorrowedBooksAsync()
        {
            var books = await _repository.GetAll();
            return books.Where(book => book.Status == "Borrowed").ToList();
        }
    }
} 
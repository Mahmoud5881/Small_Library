using Microsoft.AspNetCore.Mvc;
using Small_Library.Models;
using Small_Library.Services;
using Small_Library.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Small_Library.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IBookService bookService,UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _bookService = bookService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync();
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string title)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return NotFound();
            }

            var books = await _bookService.SearchBooksAsync(title);
            return PartialView("_BooksPartial", books);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableBooks()
        {
            var books = await _bookService.GetAvailableBooksAsync();
            return PartialView("_BooksPartial", books);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllAsync();
            return PartialView("_BooksPartial", books);
        }

        [HttpGet]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var books = await _bookService.GetBorrowedBooksAsync();
            return PartialView("_BooksPartial", books);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetBorrowers()
        {
            var books = await _bookService.GetBorrowedBooksAsync();
            if(!User.IsInRole("Admin"))
                books = books.Where(b=>b.UserId == _userManager.GetUserId(User)).ToList();
            List<string> borrowers = new List<string>();
            foreach (var book in books)
            {
                var user = await _userManager.FindByIdAsync(book.UserId);
                ViewData[book.Title] = user.UserName;
            }
            return PartialView("_BorrowedPartial", books);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var books = await _bookService.GetAllAsync();
            await _bookService.DeleteAsync(id);
            return PartialView("_BooksPartial",books);
        }
        
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book != null)
            {
                // Check if user is admin or the book belongs to the current user
                var currentUser = await _userManager.GetUserAsync(User);
                if (User.IsInRole("Admin") || (currentUser != null && book.UserId == currentUser.Id))
                {
                    book.Status = "Available";
                    book.UserId = null;
                    book.ReturnDate = null;
                    await _bookService.UpdateAsync(book);
                    var books = await _bookService.GetAllAsync();
                    return PartialView("_BooksPartial", books);
                }
                else
                {
                    return Json(new { success = false, message = "You can only return books that you borrowed." });
                }
            }
            return Json(new { success = false, message = "Book not found." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
            if (ModelState.IsValid)
            {
                var existingBook = await _bookService.GetByIdAsync(book.Id);
                if (existingBook != null)
                {
                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.Genre = book.Genre;
                    existingBook.PublishedDate = book.PublishedDate;
                    // Status is not updated - it should remain unchanged
                    
                    await _bookService.UpdateAsync(existingBook);
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            if (ModelState.IsValid)
            {
                // Set default values for new books
                book.Status = "Available";
                book.UserId = null;
                book.ReturnDate = null;
                
                await _bookService.AddAsync(book);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableBooksForAssignment()
        {
            var books = await _bookService.GetAvailableBooksAsync();
            var result = books.Select(b => new { id = b.Id, title = b.Title, author = b.Author }).ToList();
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AssignBook([FromBody] AssignBookModel model)
        {
            if (ModelState.IsValid)
            {
                var book = await _bookService.GetByIdAsync(model.BookId);
                if (book != null && book.Status == "Available")
                {
                    // Find user by username
                    var user = await _userManager.FindByNameAsync(model.BorrowerName);
                    if (user != null)
                    {
                        book.Status = "Borrowed";
                        book.UserId = user.Id;
                        book.ReturnDate = DateTime.Parse(model.ReturnDate);
                        
                        await _bookService.UpdateAsync(book);
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Borrower not found." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Book not available for assignment." });
                }
            }
            return Json(new { success = false, message = "Invalid data." });
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowBookModel model)
        {
            if (ModelState.IsValid)
            {
                var book = await _bookService.GetByIdAsync(model.BookId);
                if (book != null && book.Status == "Available")
                {
                    // Get current user
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null)
                    {
                        book.Status = "Borrowed";
                        book.UserId = currentUser.Id;
                        book.ReturnDate = DateTime.Parse(model.ReturnDate);
                        
                        await _bookService.UpdateAsync(book);
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "User not found." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Book not available for borrowing." });
                }
            }
            return Json(new { success = false, message = "Invalid data." });
        }
    }
}
    
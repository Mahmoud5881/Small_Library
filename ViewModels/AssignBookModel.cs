using System.ComponentModel.DataAnnotations;

namespace Small_Library.ViewModels
{
    public class AssignBookModel
    {
        public int BookId { get; set; }
        
        [Required(ErrorMessage = "Borrower name is required.")]
        public string BorrowerName { get; set; }
        
        [Required(ErrorMessage = "Return date is required.")]
        [FutureDate(ErrorMessage = "Return date must be in the future (after today).")]
        public string ReturnDate { get; set; }
    }
} 
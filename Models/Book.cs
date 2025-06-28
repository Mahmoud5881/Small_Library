using System;
using Microsoft.AspNetCore.Identity;

namespace Small_Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Status { get; set; }
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
} 
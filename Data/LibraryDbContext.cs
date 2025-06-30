using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Small_Library.Models;
using Small_Library.ViewModels;

namespace Small_Library.Data;

public class LibraryDbContext : IdentityDbContext<IdentityUser,IdentityRole, string>
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    protected void onModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Id)
            .IsUnique();
    }

    public DbSet<Book> Books { get; set; }
    
    public DbSet<AuditLog> AuditLogs { get; set; }
}
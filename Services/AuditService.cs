using Small_Library.ViewModels;

namespace Small_Library.Services;

public class AuditService : IAuditService
{
    private readonly LibraryDbContext _context;

    public AuditService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task LogActionAsync(string userId, string action, string details, string ipAddress)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            Details = details,
            IPAddress = ipAddress,
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}

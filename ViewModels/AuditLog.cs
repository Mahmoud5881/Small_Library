using Microsoft.AspNetCore.Identity;

namespace Small_Library.ViewModels;
public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } // who did it
    public string Action { get; set; } // what they did
    public string Details { get; set; } // optional details
    public string IPAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IdentityUser User { get; set; }
}
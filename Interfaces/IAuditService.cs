namespace Small_Library.Services;

public interface IAuditService
{
    Task LogActionAsync(string userId, string action, string details, string ipAddress);
}
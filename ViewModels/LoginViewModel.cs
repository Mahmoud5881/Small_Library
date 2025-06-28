using System.ComponentModel.DataAnnotations;

namespace Small_Library.ViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
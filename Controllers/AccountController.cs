using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Small_Library.Models;
using Small_Library.Services;
using Small_Library.ViewModels;

namespace Small_Library.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuditService _auditService;
    
    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,RoleManager<IdentityRole> roleManager, IAuditService auditService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _auditService = auditService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View("RegisterView");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm]RegisterViewModel model)
    {
        await _roleManager.CreateAsync(new IdentityRole("Admin"));
        if (ModelState.IsValid)
        {
            IdentityUser user = new IdentityUser();
            user.Email = model.Email;
            user.UserName = model.Username;
            user.PhoneNumber = model.PhoneNumber;
            IdentityResult result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                await _auditService.LogActionAsync(
                    user.Id,
                    "Register",
                    $"{user.Id} had registered successfully.",
                    ip
                );
                return RedirectToAction("Index", "Home");
            }
            foreach (var Error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, Error.Description);
            }
        }
        return View("RegisterView", model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("LoginView");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, model.Password);
                if (result)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    await _auditService.LogActionAsync(
                        user.Id,
                        "Login",
                        $"{user.Id} had logged in.",
                        ip
                    );
                    
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Username or password is incorrect.");
        }
        return View("LoginView", model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        await _auditService.LogActionAsync(
            user.Id,
            "Logout",
            $"{user.Id} had logged out.",
            ip
        );
        return RedirectToAction("Login", "Account");
    }
    
}
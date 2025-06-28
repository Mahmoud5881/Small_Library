using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Small_Library.Models;
using Small_Library.ViewModels;

namespace Small_Library.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
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
        return RedirectToAction("Login", "Account");
    }
    
}
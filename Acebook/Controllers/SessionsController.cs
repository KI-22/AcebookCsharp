using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using Microsoft.AspNetCore.Identity;


namespace acebook.Controllers;

public class SessionsController : Controller
{
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(ILogger<SessionsController> logger)
    {
        _logger = logger;
    }

    [Route("/signin")]
    [HttpGet]
    public IActionResult New()
    {
        return View();
    }

    [Route("/signin")]
    [HttpPost]
    public IActionResult Create(string email, string password) {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            TempData["ErrorMessage"] = "Email and password are required.";
            return new RedirectResult("/signin");
        }
        AcebookDbContext dbContext = new AcebookDbContext();
        User? user = dbContext.Users?.Where(user => user.Email == email).FirstOrDefault();
        if (user != null)
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = user.Password != null ? passwordHasher.VerifyHashedPassword(user, user.Password, password) : PasswordVerificationResult.Failed;

            if (result == PasswordVerificationResult.Success)
            {
                if (HttpContext.Session != null)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email ?? string.Empty);
                    HttpContext.Session.SetInt32("user_id", user.Id);
                    HttpContext.Session.SetString("Username", user.Name ?? string.Empty);
                }
                return new RedirectResult("/posts");
            }
        }

        TempData["ErrorMessage"] = "Invalid email or password.";
        return new RedirectResult("/signin");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("/signout")]
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        Response.Cookies.Delete("AspNetCore.Cookies");
        return new RedirectResult("/signin");
    }
}

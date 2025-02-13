using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.EntityFrameworkCore;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class PostsController : Controller
{
    private readonly ILogger<PostsController> _logger;

    public PostsController(ILogger<PostsController> logger)
    {
        _logger = logger;
    }

    [Route("/posts")]
    [HttpGet]
    public IActionResult Index()
    {
        AcebookDbContext dbContext = new AcebookDbContext();  // Direct instantiation
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        // Check if the user is logged in (currentUserId is null)
        if (currentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        // Get the current user from the database using the userId
        User? currentUser = dbContext.Users?.FirstOrDefault(u => u.Id == currentUserId.Value);
        if (currentUser != null)
        {
            ViewBag.CurrentUser = currentUser;
            ViewBag.ProfileUserName = currentUser.Name;
        }

        // Get all posts, including user info (ordered by CreatedAt descending)
        List<Post> posts = dbContext.Posts?
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToList() ?? new List<Post>();
        ViewBag.Posts = posts;

        // Get only the posts created by the current user
        List<Post> currentUsersPosts = dbContext.Posts?
            .Include(r => r.User)
            .Where(r => r.UserId == currentUserId.Value)  // Filter by current user's ID
            .OrderByDescending(r => r.CreatedAt)
            .ToList() ?? new List<Post>();
        ViewBag.CurrentUsersPosts = currentUsersPosts;

        return View();
    }

    [Route("/posts")]
    [HttpPost]
    public IActionResult Create(Post post, IFormFile postImageFile, string postImageUrl) 
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");
        if (currentUserId == null)
        {
            TempData["ErrorMessage"] = "You must be logged in to post. Please try again.";
            return new RedirectResult("/signin");  // Assuming you redirect back to the sign in form
        }
        post.UserId = currentUserId.Value;
        post.CreatedAt = DateTime.UtcNow;

        if (postImageFile != null && postImageFile.Length > 0)
        {
            // Save the image to the server's directory (e.g., /wwwroot/images/)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", postImageFile.FileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                postImageFile.CopyTo(fileStream);
            }

            // Save the relative path of the image in the PostImage property
            post.PostImage = "/images/" + postImageFile.FileName;
        }
        else if (!string.IsNullOrEmpty(postImageUrl))
        {
            // If the user provided a URL, save it in the PostImage property
            post.PostImage = postImageUrl;
        }
        
        if(string.IsNullOrEmpty(post.PostText) && string.IsNullOrWhiteSpace(post.PostImage))
        {
            TempData["ErrorMessage"] = "Post content can not be empty. Please try again.";
            return new RedirectResult("/posts");  // Assuming you redirect back to the add posts form
        }
        dbContext.Posts?.Add(post);
        dbContext.SaveChanges();
        return new RedirectResult("/posts");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

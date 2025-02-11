using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

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
    public IActionResult Index() {
    AcebookDbContext dbContext = new AcebookDbContext();
    List<Post> posts = dbContext.Posts?.ToList() ?? new List<Post>();
    ViewBag.Posts = posts;
    return View();
    }

    [Route("/posts")]
    [HttpPost]
    public IActionResult Create(Post post, IFormFile postImageFile, string postImageUrl) 
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        if(string.IsNullOrEmpty(post.PostText))
        {
            TempData["ErrorMessage"] = "Post content can not be empty. Please try again.";
            return new RedirectResult("/posts");  // Assuming you redirect back to the add posts form
        }
        if(post.PostText.Length < 3 || post.PostText.Length > 1000)
        {
            TempData["ErrorMessage"] = "Post content must be between 3 and 1000 characters. Please try again.";
            return new RedirectResult("/posts");  // Assuming you redirect back to the sign in form
        }
        int? currentUserId = HttpContext.Session.GetInt32("user_id");
        if (currentUserId == null)
        {
            TempData["ErrorMessage"] = "You must be logged in to post. Please try again.";
            return new RedirectResult("/signin");  // Assuming you redirect back to the registration form
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

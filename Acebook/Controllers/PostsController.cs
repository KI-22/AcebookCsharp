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

        // // Like vs Unlike button
        ViewBag.LikeOrUnlike = "Like"; // temp value
        
        // // Get likes count (TBC)
        ViewBag.LikesCount = 10; // temp value

        return View();
    }

[Route("/posts")]
[HttpPost]
public IActionResult Create(Post post, IFormFile postImageFile, string postImageUrl, string returnUrl) 
{
    AcebookDbContext dbContext = new AcebookDbContext();
    int? currentUserId = HttpContext.Session.GetInt32("user_id");
    if (currentUserId == null)
    {
        TempData["ErrorMessage"] = "You must be logged in to post. Please try again.";
        return new RedirectResult("/signin");  // Redirect to sign-in if not logged in
    }
    if (post.PostText?.Length > 500)
    {
        TempData["ErrorMessage"] = "Post content cannot exceed 500 characters. Please try again.";
        return new RedirectResult("/posts");  // Redirect back if content is too long
    }

    post.UserId = currentUserId.Value;
    post.CreatedAt = DateTime.UtcNow;

    // Handle image upload
    if (postImageFile != null && postImageFile.Length > 0)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", postImageFile.FileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            postImageFile.CopyTo(fileStream);
        }
        post.PostImage = "/images/" + postImageFile.FileName;
    }
    else if (!string.IsNullOrEmpty(postImageUrl))
    {
        post.PostImage = postImageUrl;
    }

    // Validate that there is content in the post
    if (string.IsNullOrEmpty(post.PostText) && string.IsNullOrWhiteSpace(post.PostImage))
    {
        TempData["ErrorMessage"] = "Post content cannot be empty. Please try again.";
        return new RedirectResult("/posts");  // Redirect back if content is empty
    }
    


    dbContext.Posts?.Add(post);
    dbContext.SaveChanges();

    // If ReturnUrl is provided, redirect there (either Profile or Posts page)
    if (!string.IsNullOrEmpty(returnUrl))
    {
        return Redirect(returnUrl);  // Redirect to the original page
    }

    // Default redirect to the Posts page if no ReturnUrl is provided
    return new RedirectResult("/posts");
}
   [Route("/posts/{postId}")]
    [HttpGet]
    public async Task<IActionResult> GetPost(int postId)
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

        if (dbContext?.Posts == null)
        {
            return NotFound();
        }

        var post = await dbContext.Posts
            // .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == postId);


        if (post == null)
        {
            TempData["ErrorMessage"] = "Post not found.";
            return new RedirectResult("/posts");  // Redirect back if post not found
        }
        // List<Comments> postComments = await dbContext.Comments
        //     .Where(c => c.PostId == post.Id)
        //     .OrderByDescending(p => p.CreatedAt)
        //     .ToListAsync();

        // // Store the comments in ViewBag so the Profile page can access them
        List<string> postComments = new List<string>{"Loved it", "Great post", "Nice one"};
        ViewBag.PostsComments = postComments;
        ViewBag.Post = post;
        return View(post);
        
    }
}

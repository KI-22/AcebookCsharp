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

        // // Get likes count
        var likesCount = dbContext.Likes
            .Where(l => l.PostId.HasValue)
            .GroupBy(l => l.PostId.Value)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToList();
        Dictionary<int, int> dictPostLikes = likesCount.ToDictionary(l => l.PostId, l => l.Count);
        ViewBag.LikesCount = dictPostLikes;


        // // Like vs Unlike button
        var likedCheck = dbContext.Likes
            .Where(l => l.PostId.HasValue)
            .GroupBy(l => l.PostId.Value)
            .Select(g => new 
            { 
                PostId = g.Key, 
                UserIds = g.Where(l => l.UserId.HasValue).Select(l => l.UserId.Value).ToList()  // Only include non-null UserIds
            })
            .ToList();

        // Convert the result into a dictionary where the key is postId, and the value is a list of userIds
        Dictionary<int, List<int>> dictLikeUnlike = likedCheck.ToDictionary(l => l.PostId, l => l.UserIds);

        // Dictionary<int, string> dictLikeUnlike = likedCheck.ToDictionary(l => l.PostId, l => "Like");
        ViewBag.LikeUnlike = dictLikeUnlike;

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

        var post = dbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefault(p => p.Id == postId);


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
        ViewBag.Post = post;
        return View(post);
        
    }
    
    [HttpPost]
    [Route("/posts/{id}/delete")]
    public IActionResult Delete(int id, string ReturnUrl)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            TempData["ErrorMessage"] = "You must be logged in to delete a post.";
            return new RedirectResult("/signin");  // Redirect to sign-in
        }

        var post = dbContext.Posts?.FirstOrDefault(p => p.Id == id);

        if (post == null)
        {
            TempData["ErrorMessage"] = "Post not found.";
            return new RedirectResult("/posts");  // Redirect if post doesn't exist
        }

        if (post.UserId != currentUserId.Value)
        {
            TempData["ErrorMessage"] = "You can only delete your own posts.";
            return new RedirectResult("/posts");  // Redirect if not the owner
        }
        var likes = dbContext.Likes.Where(l => l.PostId == id);
        dbContext.Likes.RemoveRange(likes);

        dbContext.Posts?.Remove(post);
        dbContext.SaveChanges();

        TempData["SuccessMessage"] = "Post deleted successfully.";
        if (!string.IsNullOrEmpty(ReturnUrl))
        {
            return Redirect(ReturnUrl);  // Redirect to the original page
        }
        return new RedirectResult("/posts");  // Redirect after deletion
    }
}
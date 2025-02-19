using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.EntityFrameworkCore;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class CommentsController : Controller
{
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ILogger<CommentsController> logger)
    {
        _logger = logger;
    }


    [Route("/posts/{postId}/comments")]
    [HttpGet]
    public IActionResult GetComments(int postId)
    {
        AcebookDbContext dbContext = new AcebookDbContext();  
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        // Fetch comments for the post
        var comments = dbContext.Comments
        .Include(c => c.User)
        .Where(c => c.PostId == postId)
        .OrderByDescending(c => c.CreatedAt)  // Ensure newest comments first
        .ToList();

        return View("Comments", comments);  // ✅ Pass the List<Comment> directly
    }

    [Route("/posts/{postId}/comments")]
    [HttpPost]
    public IActionResult CreateComment(Comment comment)
    {
        int? currentUserId = HttpContext.Session.GetInt32("user_id");
        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Sessions");
        }

        using (AcebookDbContext dbContext = new AcebookDbContext())
        {
            comment.UserId = currentUserId.Value;
            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();
        }

        // Redirect back to the post where the comment was added
        return RedirectToAction("GetPost", "Posts", new { postId = comment.PostId });
    }
}

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
        ViewBag.CurrentUserId = currentUserId.Value;
        ViewBag.PostId = postId;

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
    [Route("/posts/{id}/comments/{commentId}/delete")]
    [HttpPost]
    public IActionResult Delete(int id, int commentId)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            TempData["ErrorMessage"] = "You must be logged in to delete a comment.";
            return new RedirectResult("/signin");  // Redirect to sign-in
        }

        var comment = dbContext.Comments?.FirstOrDefault(c => c.Id == commentId);

        if (comment == null)
        {
            TempData["ErrorMessage"] = "Comment not found.";
            return new RedirectResult("/posts");  // Redirect if comment doesn't exist
        }

        if (comment.UserId != currentUserId.Value)
        {
            TempData["ErrorMessage"] = "You can only delete your own comments.";
            return new RedirectResult($"/posts/{id}");  // Redirect if not the owner
        }

        dbContext.Comments?.Remove(comment);
        dbContext.SaveChanges();

        TempData["SuccessMessage"] = "Comment deleted successfully.";

        ViewBag.CommentId = commentId;
        ViewBag.CurrentUserId = currentUserId.Value;

        return new RedirectResult($"/posts/{id}");  // Redirect after deletion

    }
}

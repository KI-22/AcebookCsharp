using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.EntityFrameworkCore;

namespace acebook.Controllers;

public class LikesController : Controller
{
    private readonly ILogger<LikesController> _logger;

    public LikesController(ILogger<LikesController> logger)
    {
        _logger = logger;   
    }

    // POST Request (to like a post)
    [Route("/posts/{postId}/like")]
    [HttpPost]
    public IActionResult LikePost(int postId)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        // // Check if the user has already liked the post
        var existingLike = dbContext.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == currentUserId);

        if (existingLike != null)
        {
            Console.WriteLine("Like already exists. Removed from table.");
            return new RedirectResult("/posts");
        }

        Console.WriteLine("PostId: " + postId);
        Console.WriteLine("UserId: " + currentUserId);

    
        // Add the like
        var like = new Likes
        {
            PostId = postId,
            UserId = currentUserId
        };

        dbContext.Likes.Add(like);
        dbContext.SaveChanges();

        return new RedirectResult("/posts");
    }

}
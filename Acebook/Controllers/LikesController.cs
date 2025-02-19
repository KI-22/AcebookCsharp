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
    // public IActionResult LikePost(int postId, string returnUrl = null)
    public IActionResult LikePost(int postId, string returnUrl)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        // Check if the user has already liked the post
        var existingLike = dbContext.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == currentUserId);

        if (existingLike != null)
        {
            Console.WriteLine("Like already exists.");
            // User has already liked the post, set to "Unlike"
            ViewBag.PostLikeUnlike = "Unlike";
        }
        else
        {
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

            // Set button text to "Unlike" after adding like
            ViewBag.PostLikeUnlike = "Unlike";
        }
        
        return new RedirectResult(returnUrl); 
    }

    // POST Request (to UN-like a post)
    [Route("/posts/{postId}/unlike")]
    [HttpPost]
    public IActionResult UnlikePost(int postId, string returnUrl)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        // Check if the user has already liked the post
        var existingLike = dbContext.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == currentUserId);

        if (existingLike == null)
        {
            Console.WriteLine("This post has not been liked yet.");
            // User has NOT liked the post, set to "Like"
            ViewBag.PostLikeUnlike = "Like";
        }
        else
        {
            Console.WriteLine("PostId: " + postId);
            Console.WriteLine("UserId: " + currentUserId);

            // Remove the like from the database
            dbContext.Likes.Remove(existingLike);
            dbContext.SaveChanges();

            // Set button text to "Unlike" after adding like
            ViewBag.PostLikeUnlike = "Like";
        }
        
        return new RedirectResult(returnUrl); 
    }


}
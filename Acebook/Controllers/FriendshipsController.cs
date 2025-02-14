using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using acebook.Models;
using acebook.ActionFilters;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class FriendshipsController : Controller
{

    private readonly ILogger<FriendshipsController> _logger;


    public FriendshipsController(ILogger<FriendshipsController> logger)
    {
        _logger = logger;
    }

    [Route("/Friendships")]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/Friendships/SendFriendRequest")]
    [HttpGet]
    public IActionResult SendFriendRequest()
    {
        return View();
    }


    [Route("/Friendships/RequestSent")]
    [HttpPost]
    public async Task<IActionResult> SendFriendRequest(int receiverId)
    {
        AcebookDbContext dbContext = new AcebookDbContext();

        int? CurrentUserId = HttpContext.Session.GetInt32("user_id");

        // Check if the user is logged in (currentUserId is null)
        if (CurrentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        int senderId = CurrentUserId.Value;

        var existingRequest = await dbContext.Friendships.FirstOrDefaultAsync(f => f.User1Id == senderId && f.User2Id == receiverId);
        if (existingRequest != null)
        {
            TempData["ErrorMessage"] = "Friend request already sent.";
            return RedirectToAction("Index", "Home");
        }
        var friendship = new Friendship
        {
            User1Id = senderId,
            User2Id = receiverId,
            FriendshipStatus = "Pending"
        };

        dbContext.Friendships.Add(friendship);
        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Friend request sent!";
        return RedirectToAction("Index", "Home");
    }
}
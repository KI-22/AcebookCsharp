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
        AcebookDbContext dbContext = new AcebookDbContext();  // Direct instantiation
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        User? currentUser = dbContext.Users?.FirstOrDefault(u => u.Id == currentUserId.Value);
        if (currentUser != null)
        {
            ViewBag.CurrentUser = currentUser;
            //ViewBag.ProfileUserName = currentUser.Name;
        }

        List<Friendship> currentUsersFriendships = dbContext.Friendships? 
            .Where(r => r.User2Id == currentUserId.Value)
            .Where(r => r.FriendshipStatus == "Pending")
            //.OrderByDescending(r => r.User1Id)
            .ToList() ?? new List<Friendship>();
        ViewBag.CurrentUsersFriendships = currentUsersFriendships;

        List<Friendship> currentUsersAcceptedFriendships = dbContext.Friendships? 
            .Where(r => r.User2Id == currentUserId.Value)
            .Where(r => r.FriendshipStatus == "Accepted")
            .ToList() ?? new List<Friendship>();
        ViewBag.CurrentUsersAcceptedFriendships = currentUsersAcceptedFriendships;
        return View();
    }

    [Route("/Friendships/SendFriendRequest")]
    [HttpGet]
    public IActionResult SendFriendRequest()
    {
        return View();
    }


    [Route("/Friendships/SendFriendRequest")]
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



    // [Route("/Friendships/AcceptRequest")]
    // [HttpGet]
    // public IActionResult AcceptingRequest()
    // {
    //     return RedirectToAction("Index", "Friendships");
    // }



    [Route("/Friendships/AcceptRequest")]
    [HttpPost]
    public async Task<IActionResult> AcceptRequest()
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        var FriendshipToUpdate = await dbContext.Friendships.FirstOrDefaultAsync(u => u.User2Id == currentUserId.Value);;
        FriendshipToUpdate.FriendshipStatus = "Accepted";
        dbContext.Friendships.Update(FriendshipToUpdate);
        await dbContext.SaveChangesAsync();
        return RedirectToAction("Index", "Friendships");
    }



    [Route("/Friendships/RejectRequest")]
    [HttpGet]
    public IActionResult RejectingRequest()
    {
        return RedirectToAction("Index", "Friendships");
    }


    [Route("/Friendships/RejectRequest")]
    [HttpPost]
    public async Task<IActionResult> RejectRequest()
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        var FriendshipToUpdate = await dbContext.Friendships.FirstOrDefaultAsync(u => u.User2Id == currentUserId.Value);;
        FriendshipToUpdate.FriendshipStatus = "Rejected";
        dbContext.Friendships.Update(FriendshipToUpdate);
        await dbContext.SaveChangesAsync();
        return RedirectToAction("Index", "Friendships");
    }
}
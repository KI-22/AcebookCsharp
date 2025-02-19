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
        using (AcebookDbContext dbContext = new AcebookDbContext())
        {
            int? currentUserId = HttpContext.Session.GetInt32("user_id");

            if (currentUserId == null)
            {
                return RedirectToAction("Signin", "Sessions");
            }

            User? currentUser = dbContext.Users?.FirstOrDefault(u => u.Id == currentUserId.Value);
            if (currentUser != null)
            {
                ViewBag.CurrentUser = currentUser;
            }

            // Fetch Pending Friend Requests
            List<Friendship> currentUsersFriendships = dbContext.Friendships? 
                .Where(r => r.User2Id == currentUserId.Value)
                .Where(r => r.FriendshipStatus == "Pending")
                .Include(f => f.User1)
                .ToList() ?? new List<Friendship>();
            ViewBag.CurrentUsersFriendships = currentUsersFriendships;

            // ✅ Fetch Accepted Friendships and Exclude Self-Friendships
            List<Friendship> currentUsersAcceptedFriendships = dbContext.Friendships?
                .Where(r => (r.User1Id == currentUserId.Value || r.User2Id == currentUserId.Value) &&
                             r.FriendshipStatus == "Accepted" &&
                             r.User1Id != r.User2Id)  // ✅ EXCLUDE self-friendships
                .Include(f => f.User1)
                .Include(f => f.User2)
                .ToList() ?? new List<Friendship>();

            var uniqueFriendships = currentUsersAcceptedFriendships
                .GroupBy(f => new { MinId = Math.Min(f.User1Id, f.User2Id), MaxId = Math.Max(f.User1Id, f.User2Id) })
                .Select(g => g.First())
                .ToList();

            ViewBag.CurrentUsersAcceptedFriendships = uniqueFriendships;

            return View();
        }
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
            return Json(new { success = false, message = "Please sign in to send a friend request." });
        }

        int senderId = CurrentUserId.Value;

        var existingRequest = await dbContext.Friendships.FirstOrDefaultAsync(f => f.User1Id == senderId && f.User2Id == receiverId);
        if (existingRequest != null)
        {
             return Json(new { success = false, message = "Friend request already sent." });
        }

        var friendship = new Friendship
        {
            User1Id = senderId,
            User2Id = receiverId,
            FriendshipStatus = "Pending"
        };

        dbContext.Friendships.Add(friendship);
        await dbContext.SaveChangesAsync();
        
        return Json(new { success = true, message = "Friend request sent!" });
    }


    [Route("/Friendships/AcceptRequest")]
    [HttpPost]
    public async Task<IActionResult> AcceptRequest(int requesterId)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        // Fetch the pending friendship request
        var friendshipToUpdate = await dbContext.Friendships
            .FirstOrDefaultAsync(f => f.User1Id == requesterId && f.User2Id == currentUserId.Value && f.FriendshipStatus == "Pending");

        if (friendshipToUpdate == null)
        {
            Console.WriteLine($"No pending request found from {requesterId} to {currentUserId.Value}");
            return RedirectToAction("Index", "Friendships"); // No pending request found
        }

        // ✅ Update friendship to "Accepted"
        friendshipToUpdate.FriendshipStatus = "Accepted";
        dbContext.Friendships.Update(friendshipToUpdate);

        // ✅ Ensure reverse friendship is created **only once**
        var reverseFriendshipExists = await dbContext.Friendships
            .AnyAsync(f => f.User1Id == currentUserId.Value && f.User2Id == requesterId && f.FriendshipStatus == "Accepted");

        if (!reverseFriendshipExists && requesterId != currentUserId.Value)
        {
            var reverseFriendship = new Friendship
            {
                User1Id = currentUserId.Value,
                User2Id = requesterId,
                FriendshipStatus = "Accepted"
            };

            dbContext.Friendships.Add(reverseFriendship);
        }

        await dbContext.SaveChangesAsync();
        Console.WriteLine($"Friendship accepted: {currentUserId.Value} and {requesterId} are now friends.");

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
    public async Task<IActionResult> RejectRequest(int requesterId)
    {
        AcebookDbContext dbContext = new AcebookDbContext();
        int? currentUserId = HttpContext.Session.GetInt32("user_id");

        if (currentUserId == null)
        {
            return RedirectToAction("Signin", "Sessions");
        }

        // Debugging log to ensure correct user data
        Console.WriteLine($"User {currentUserId.Value} is rejecting a request from {requesterId}");

        var friendshipToDelete = await dbContext.Friendships
            .FirstOrDefaultAsync(f => f.User1Id == requesterId && f.User2Id == currentUserId.Value && f.FriendshipStatus == "Pending");

        if (friendshipToDelete == null)
        {
            Console.WriteLine("No pending request found.");
            return RedirectToAction("Index", "Friendships"); // No pending request found
        }

        dbContext.Friendships.Remove(friendshipToDelete);
        await dbContext.SaveChangesAsync();
        
        Console.WriteLine("Friend request rejected.");
        return RedirectToAction("Index", "Friendships");
    }


    [Route("/Friendships/CancelFriendRequest")]
    [HttpPost]
    public async Task<IActionResult> CancelFriendRequest(int receiverId)
    {
        using (AcebookDbContext dbContext = new AcebookDbContext())
        {
            int? currentUserId = HttpContext.Session.GetInt32("user_id");

            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Please sign in to cancel a friend request." });
            }

            var friendRequest = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.User1Id == currentUserId.Value && f.User2Id == receiverId && f.FriendshipStatus == "Pending");

            if (friendRequest == null)
            {
                return Json(new { success = false, message = "No pending request to cancel." });
            }
            
            dbContext.Friendships.Remove(friendRequest);
            await dbContext.SaveChangesAsync();
              
            return Json(new { success = true, message = "Friend request canceled." });
        
        }
    }
    
    [Route("/Friendships/RemoveFriend")]
    [HttpPost]
    public async Task<IActionResult> RemoveFriend(int friendId)
    {
        using (AcebookDbContext dbContext = new AcebookDbContext())
        {
            int? currentUserId = HttpContext.Session.GetInt32("user_id");

            if (currentUserId == null)
            {
                return RedirectToAction("Signin", "Sessions");
            }

            // Remove friendships in both directions
             var friendshipsToRemove = await dbContext.Friendships
                .Where(f => 
                    (f.User1Id == currentUserId.Value && f.User2Id == friendId) ||
                    (f.User1Id == friendId && f.User2Id == currentUserId.Value))
                .ToListAsync();

            if (!friendshipsToRemove.Any())
            {
                return RedirectToAction("Index", "Friendships");
            }

            dbContext.Friendships.RemoveRange(friendshipsToRemove);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Friendships");
        }
    }
}
    


    

 

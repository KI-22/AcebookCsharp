using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace acebook.Controllers;

public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [Route("/signup")]
    [HttpGet]
    public IActionResult New()
    {
        return View();
    }

    [Route("/users")]
    [HttpPost]
    public IActionResult Create(User user) {
        using (var dbContext = new AcebookDbContext())
        {
        // Explicitly check if password is null or empty
        if (string.IsNullOrWhiteSpace(user.Password))
        {
            ModelState.AddModelError("Password", "Password is required.");
        }
        // Check if email is already in use
        if (dbContext.Users != null && dbContext.Users.Any(u => u.Email == user.Email))
        {
            ModelState.AddModelError("Email", "Email is already in use.");
        }
        if (string.IsNullOrWhiteSpace(user.FullName))
        {
            ModelState.AddModelError("FullName", "Full name is required.");
        }

        if (!ModelState.IsValid)
        {
            return View("New");
        }

        // Ensure password meets security requirements
        var passwordHasher = new PasswordHasher<User>();
        if (user.Password != null)
        {
            user.Password = passwordHasher.HashPassword(user, user.Password);
        }

        user.JoinedDate = DateTime.UtcNow;
        user.Bio = "";
        user.IsPrivate = false;

        // Set default profile picture if not provided
        if (string.IsNullOrEmpty(user.profilePicture))
        {
            user.profilePicture = "/images/default-picture/default.png"; // Adjust the path as needed
        }

        dbContext.Users?.Add(user);
        dbContext.SaveChanges();
        return new RedirectResult("/signin");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("/{username}")]
    [HttpGet]
    public async Task<IActionResult> Profile(string username)
    {
        using (var dbContext = new AcebookDbContext())
        {
        if (dbContext?.Users == null)
        {
            return NotFound();
        }

        var user = await dbContext.Users
            .Include(u => u.Posts)
            .FirstOrDefaultAsync(u => u.Name == username);

        if (user == null)
        {
            return NotFound();
        }
        var currentUserId = HttpContext.Session.GetInt32("user_id");
        bool isFriend = false;
        bool isFriendRequestPending = false;

        if (currentUserId.HasValue)
        {
            isFriend = await dbContext.Friendships
                .AnyAsync(f => 
                    (f.User1Id == currentUserId.Value && f.User2Id == user.Id ||
                    f.User2Id == currentUserId.Value && f.User1Id == user.Id) &&
                    f.FriendshipStatus == "Accepted");

            isFriendRequestPending = await dbContext.Friendships
                .AnyAsync(f => f.User1Id == currentUserId.Value && f.User2Id == user.Id && f.FriendshipStatus == "Pending");
        }

        ViewBag.IsFriends = isFriend;
        ViewBag.IsFriendRequestPending = isFriendRequestPending;
        ViewBag.RestrictedProfile = user.IsPrivate && !isFriend && currentUserId != user.Id;
        List<Post>? userPosts = null;
        if (dbContext.Posts != null)
        {
            userPosts = await dbContext.Posts
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        ViewBag.CurrentUsersPosts = userPosts;

        // Get likes count        
        if (userPosts != null)
            {
                var postLikesCount = new Dictionary<int, int>();
                
                foreach (var post in userPosts)
                {
                    int likesCountForPost = dbContext.Likes
                        .Where(l => l.PostId == post.Id)
                        .Count();
                    
                    postLikesCount[post.Id] = likesCountForPost;
                }
                ViewBag.PostLikesCount = postLikesCount;
            }

            // Like vs Unlike button
            ViewBag.currentUserId = HttpContext.Session.GetInt32("user_id");
            var likedCheck = dbContext.Likes
                .Where(l => l.PostId.HasValue)
                .GroupBy(l => l.PostId.Value)
                .Select(g => new 
                { 
                    PostId = g.Key, 
                    UserIds = g.Where(l => l.UserId.HasValue).Select(l => l.UserId.Value).ToList()  // Only include non-null UserIds
                })
                .ToList();

            Dictionary<int, List<int>> dictLikeUnlike = likedCheck.ToDictionary(l => l.PostId, l => l.UserIds);

            ViewBag.LikeUnlike = dictLikeUnlike;

            var commentsCount = dbContext.Comments
            .Where(c => c.PostId.HasValue)
            .GroupBy(c => c.PostId.Value)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToList();

        // Convert to dictionary: PostId -> Comment Count
            Dictionary<int, int> dictPostComments = commentsCount.ToDictionary(c => c.PostId, c => c.Count);
            ViewBag.CommentsCount = dictPostComments;
            

            // current URL
            ViewBag.CurrentURL = Request.Path + Request.QueryString;

        return View(user);
        }
    }

    [Route("/{username}/edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(string username)
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (loggedInUser != username)
        {
            TempData["ErrorMessage"] = "You are not allowed to edit someone else's profile.";
            return RedirectToAction("Profile", "Users", new { username = username });
        }
        
        using (var dbContext = new AcebookDbContext())
        {
        if (dbContext?.Users == null)
        {
            return NotFound();
        }

        var userToEdit = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Name == username);

        if (userToEdit == null)
        {
            return NotFound();
        }

        return View(userToEdit);
        }
    }

    [Route("/{username}/edit")]
    [HttpPost]
    public async Task<IActionResult> Edit(string username, User user, IFormFile? profilePictureFile)
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (loggedInUser != username)
        {
            TempData["ErrorMessage"] = "You are not allowed to edit someone else's profile.";
            return RedirectToAction("Profile", "Users", new { username = username });
        }
        
        using (var dbContext = new AcebookDbContext())
        {
        if (dbContext?.Users == null)
        {
            return NotFound();
        }

        var userToUpdate = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Name == username);

        if (userToUpdate == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(userToUpdate);
        }

        if (!string.IsNullOrWhiteSpace(user.FullName))
        {
            userToUpdate.FullName = user.FullName;
        }

        if (!string.IsNullOrWhiteSpace(user.Bio))
        {
            userToUpdate.Bio = user.Bio;
        }

        //  Allow users to toggle Privacy setting
        userToUpdate.IsPrivate = user.IsPrivate;

        // Ensure `JoinedDate` is NOT changed
        userToUpdate.JoinedDate = userToUpdate.JoinedDate;

        // Update fields if changed
        if (!string.IsNullOrWhiteSpace(user.Name))
        {
            userToUpdate.Name = user.Name;
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            userToUpdate.Email = user.Email;
        }

        if (!string.IsNullOrEmpty(user.Password))
        {
            var passwordHasher = new PasswordHasher<User>();
            userToUpdate.Password = passwordHasher.HashPassword(userToUpdate, user.Password);
        }

         // Handle profile picture upload
        if (profilePictureFile != null && profilePictureFile.Length > 0)
        {
            // Check file format (only jpg, png, jpeg, gif allowed)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(profilePictureFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("ProfilePicture", "Only JPG, JPEG, PNG, and GIF files are allowed.");
                return View(userToUpdate);
            }

            try
            {
                // Ensure wwwroot/user_picture exists
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "user_picture");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Generate unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(profilePictureFile.FileName)}";
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePictureFile.CopyToAsync(fileStream);
                }

                // Save relative path
                userToUpdate.profilePicture = "/user_picture/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ProfilePicture", $"Error uploading image: {ex.Message}");
                return View(userToUpdate);
            }
        }
        else if (!string.IsNullOrEmpty(user.profilePicture))
        {
            // If the user provided a URL, save it
            userToUpdate.profilePicture = user.profilePicture;
        }

        dbContext.Users.Update(userToUpdate);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Profile", new { username = userToUpdate.Name });
        }
    }

    [Route("/{username}/change-password")]
    [HttpGet]
    public IActionResult ChangePassword(string username)
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (loggedInUser != username)
        {
            return Forbid(); // Prevent unauthorized access
        }
    
        return View(new ChangePasswordModelView { Username = username });
    }
        

    [Route("/{username}/change-password")]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string username, ChangePasswordModelView model)
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (loggedInUser != username)
        {
            return Forbid(); // Prevent unauthorized access
        }
        using (var dbContext = new AcebookDbContext())
        {
        if (dbContext?.Users == null)
        {
            return NotFound();
        }

        var userToUpdate = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Name == username);

        if (userToUpdate == null)
        {
            return NotFound();
        }

        var passwordHasher = new PasswordHasher<User>();
        if (userToUpdate.Password == null)
        {
            ModelState.AddModelError("OldPassword", "Password cannot be null.");
            return View(model);
        }
        
        var passwordVerification = passwordHasher.VerifyHashedPassword(userToUpdate, userToUpdate.Password, model.OldPassword);

        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("OldPassword", "Incorrect old password.");
            return View(model);
        }

        // Hash and update new password
        userToUpdate.Password = passwordHasher.HashPassword(userToUpdate, model.NewPassword);
        dbContext.Users.Update(userToUpdate);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Profile", new { username = userToUpdate.Name });
        }
    }

    [Route("/users/search")]
    [HttpGet]
    public async Task<IActionResult> SearchUser(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            TempData["ErrorMessage"] = "Please enter a valid username.";
            return RedirectToAction("Index", "Posts");
        }

        using (var dbContext = new AcebookDbContext())
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Name.ToLower() == query.ToLower());

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Posts");
            }

            return RedirectToAction("Profile", "Users", new { username = user.Name });
        }
    }   
}



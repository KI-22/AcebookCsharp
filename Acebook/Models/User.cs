namespace acebook.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class User
{
  [Key]
  public int Id {get; set;}
  [Required(ErrorMessage = "Username is required.")]
  [StringLength(50)]
  public string? Name {get; set;}

  [StringLength(300, ErrorMessage = "Bio cannot be longer than 300 characters.")]
  [RegularExpression(@"^[a-zA-Z0-9\s.,!?@#&()'\""-]*$", 
        ErrorMessage = "Bio can only contain letters, numbers, spaces, and common punctuation.")]
    public string? Bio { get; set; }

  [Required(ErrorMessage = " Please enter your name.")]
  [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
  [RegularExpression(@"^[a-zA-ZÀ-ÿ' -]+$", 
        ErrorMessage = "Name can only contain letters, spaces, hyphens, and apostrophes.")]
    public string? FullName { get; set; }

  public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

  public bool IsPrivate { get; set; } = false; 


  [Required]
  [EmailAddress]
  public string? Email {get; set;}

  
  [DataType(DataType.Password)]
  [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
  ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.")] 
  public string? Password {get; set;}

  [RegularExpression(@"(https?:\/\/.*\.(?:png|jpg|jpeg|gif|svg))", ErrorMessage = "Please enter a valid image URL (png, jpg, jpeg, gif, svg).")]
  public string? profilePicture {get; set;}

  

  public virtual ICollection<Friendship> SentFriendRequests { get; set; } = new List<Friendship>();
  public virtual ICollection<Friendship> ReceivedFriendRequests { get; set; } = new List<Friendship>();
  public ICollection<Post>? Posts {get; set;} // relationship
}
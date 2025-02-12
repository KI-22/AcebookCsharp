namespace acebook.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class User
{
  [Key]
  public int Id {get; set;}
  [Required]
  [StringLength(50)]
  public string? Name {get; set;}

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



  public ICollection<Post>? Posts {get; set;} // relationship
}
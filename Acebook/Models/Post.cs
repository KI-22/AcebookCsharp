namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class Post
{
  [Key]
  public int Id {get; set;}
  public string? PostText {get; set;}
  public string? PostImage {get; set;}
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public int UserId {get; set;}
  public User? User {get; set;}
  public ICollection<Comment>? Comments {get; set;} // relationship
}
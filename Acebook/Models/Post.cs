namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class Post
{
  [Key]
  public int Id {get; set;}
  public string? PostText {get; set;}
  public string? PostImage {get; set;}
  public int UserId {get; set;}
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public User? User {get; set;}
}
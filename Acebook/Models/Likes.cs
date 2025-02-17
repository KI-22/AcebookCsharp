namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class Likes
{
  [Key]
  public int Id {get; set;}
  public int? PostId {get; set;}
  public int? UserId {get; set;}
  
  public User? User {get; set;}
  public Post? Post {get; set;}

  // zero argument constructor << TBC
  public Likes() {}

}
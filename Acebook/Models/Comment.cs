namespace acebook.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class Comment
{
    [Key]
    public int Id {get; set;}
    [Required]
    [StringLength(500)]
    public string? Content {get; set;}

    public int? PostId {get; set;}
    public Post? Post {get; set;}

    public int? UserId {get; set;}
    public User? User {get; set;}
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
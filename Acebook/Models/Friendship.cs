namespace acebook.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Friendship
{
    [Key]
    public int Id {get; set;}
    public int User1Id { get; set; }
    public int User2Id { get; set; }

    [ForeignKey("User1Id")]
    public virtual User? User1 { get; set; } //This is the user who sent the request

    [ForeignKey("User2Id")]
    public virtual User? User2 { get; set; } //This is who they sent it to
    public string? FriendshipStatus {get; set;} //Accepted, Pending, Rejected


    public Friendship(int User1Id, int User2Id, string FriendshipStatus) {
        this.User1Id = User1Id;
        this.User2Id = User2Id;
        this.FriendshipStatus = FriendshipStatus;
    }

    public Friendship(){}
}
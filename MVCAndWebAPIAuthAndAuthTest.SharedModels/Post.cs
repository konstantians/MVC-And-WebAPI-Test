using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MVCAndWebAPIAuthAndAuthTest.SharedModels;

public class Post
{
    public int Id { get; set; }
    public DateTime SentAt { get; set; }
    [Required]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "The Post Title Can Not Exceed 40 Characters")]
    public string? Title { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "The Post Content Can Not Exceed 500 Characters")]
    public string? Content { get; set; }

    [NotMapped]
    public IdentityUser? AppUser { get; set; }
    public string? UserId { get; set; }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MVCAndWebAPIAuthAndAuthTest.API.Models.ResponseModels;

internal class ApiPostResponseModel
{
    public string? Guid { get; set; }
    public DateTime SentAt { get; set; }
    [Required]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "The Post Title Can Not Exceed 40 Characters")]
    public string? Title { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "The Post Content Can Not Exceed 500 Characters")]
    public string? Content { get; set; }
    public string? UserId { get; set; }
    public IdentityUser? AppUser { get; set; }
}

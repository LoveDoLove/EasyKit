using System.ComponentModel.DataAnnotations;

namespace CommonUtilities.Models;

public class User
{
    [Key] public int Id { get; set; }

    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)] public string LastName { get; set; } = string.Empty;

    [MaxLength(255)] public string UserName { get; set; } = string.Empty;

    [MaxLength(255)] public string Email { get; set; } = string.Empty;

    [MaxLength(255)] public string Password { get; set; } = string.Empty;

    [MaxLength(50)] public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(50)] public string Role { get; set; } = UserRoles.Member.ToString();

    [MaxLength(255)] public string? ProfileImageUrl { get; set; }

    public bool IsBlocked { get; set; } = false;

    public int IsDeleted { get; set; } = 0;

    public bool IsMfaEnabled { get; set; } = false;

    [MaxLength(50)] public string GoogleMfaSecretKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    //public ICollection<Cart> Carts { get; set; } = [];
}
using System.ComponentModel.DataAnnotations;

namespace CommonUtilities.Models;

public enum UserRoles
{
    SuperAdmin,
    Admin,
    Member
}

public class Role
{
    [Key] public int Id { get; set; }

    [MaxLength(100)] public string Name { get; set; } = string.Empty;
}
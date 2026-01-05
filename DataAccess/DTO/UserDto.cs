using BusinessObject;
using BusinessObject.Enums;

namespace DataAccess.DTO;

public class UserDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
    public Gender Gender { get; set; }
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? FcmToken { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    
}
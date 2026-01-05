using BusinessObject.Enums;

namespace DataAccess.DTO.Request;

public class UserRequest
{
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
    public Gender Gender { get; set; }
    public Role Role { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
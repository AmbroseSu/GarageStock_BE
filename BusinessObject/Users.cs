using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessObject.Enums;

namespace BusinessObject;

public class Users
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
    public Gender Gender { get; set; }
    public Role Role { get; set; }
    
    private DateTime _createdAt;
    public DateTime CreatedAt
    {
        get => DateTime.SpecifyKind(_createdAt, DateTimeKind.Utc).ToLocalTime();
        set => _createdAt = value.ToUniversalTime();
    }
    private DateTime _updatedAt;
    public DateTime UpdatedAt
    {
        get => DateTime.SpecifyKind(_updatedAt, DateTimeKind.Utc).ToLocalTime();
        set => _updatedAt = value.ToUniversalTime();
    }
    public string? FcmToken { get; set; }
    private DateTime? _dateOfBirth;
    public DateTime? DateOfBirth
    {
        get => _dateOfBirth.HasValue ? DateTime.SpecifyKind(_dateOfBirth.Value, DateTimeKind.Utc).ToLocalTime() : (DateTime?)null;
        set => _dateOfBirth = value?.ToUniversalTime();
    }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    
    public List<SalesOrders>? SalesOrders { get; set; }
    public List<PurchaseOrders>? PurchaseOrders { get; set; }
    public List<Vehicles>? Vehicles { get; set; }
    public List<VerificationOtp>? VerificationOtps { get; set; }
    
}
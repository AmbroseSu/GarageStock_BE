using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class ProductDiscountGroups
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid ProductDiscountGroupId { get; set; }
    private DateTime _createdDate;
    public DateTime CreatedDate
    {
        get => DateTime.SpecifyKind(_createdDate, DateTimeKind.Utc).ToLocalTime();
        set => _createdDate = value.ToUniversalTime();
    }
    public bool IsDeleted { get; set; }
    
    public Guid ProductId { get; set; }
    public Products? Product { get; set; }
    public Guid DiscountId { get; set; }
    public Discounts? Discount { get; set; }
}
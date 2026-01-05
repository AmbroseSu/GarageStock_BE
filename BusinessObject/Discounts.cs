using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class Discounts
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid DiscountId { get; set; }
    public string DiscountName { get; set; }
    public decimal DiscountPercentage { get; set; }
    private DateTime _startDate;
    public DateTime StartDate
    {
        get => DateTime.SpecifyKind(_startDate, DateTimeKind.Utc).ToLocalTime();
        set => _startDate = value.ToUniversalTime();
    }
    private DateTime _endDate;
    public DateTime EndDate
    {
        get => DateTime.SpecifyKind(_endDate, DateTimeKind.Utc).ToLocalTime();
        set => _endDate = value.ToUniversalTime();
    }
    public bool IsDeleted { get; set; }
    
    public List<ProductDiscountGroups>? ProductDiscountGroups { get; set; }
}
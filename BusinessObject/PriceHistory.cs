using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessObject.Enums;

namespace BusinessObject;

public class PriceHistory
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid PriceHistoryId { get; set; }
    public PriceType PriceType { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    private DateTime _changeAt;
    public DateTime ChangeAt
    {
        get => DateTime.SpecifyKind(_changeAt, DateTimeKind.Utc).ToLocalTime();
        set => _changeAt = value.ToUniversalTime();
    }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid ProductId { get; set; }
    public Products? Product { get; set; }
}
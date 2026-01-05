using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class Inventory
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid InventoryId { get; set; }
    public int Quantity { get; set; }
    private DateTime _updatedAt;
    public DateTime UpdatedAt
    {
        get => DateTime.SpecifyKind(_updatedAt, DateTimeKind.Utc).ToLocalTime();
        set => _updatedAt = value.ToUniversalTime();
    }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid ProductId { get; set; }
    public Products? Product { get; set; }
}
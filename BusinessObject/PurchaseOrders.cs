using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class PurchaseOrders
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid PurchaseOrderId { get; set; }
    private DateTime _orderDate;
    public DateTime OrderDate
    {
        get => DateTime.SpecifyKind(_orderDate, DateTimeKind.Utc).ToLocalTime();
        set => _orderDate = value.ToUniversalTime();
    }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string? Note { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid SupplierId { get; set; }
    public Users? Supplier { get; set; }
    
    public List<PurchaseOrderDetails>? PurchaseOrderDetails { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class SalesOrders
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid SalesOrderId { get; set; }
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
    
    public Guid UserId { get; set; }
    public Users? User { get; set; }
    
    public List<SalesOrderDetails>? SalesOrderDetails { get; set; }
}
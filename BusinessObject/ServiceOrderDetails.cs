using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class ServiceOrderDetails
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid ServiceOrderDetailId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid ServiceOrderId { get; set; }
    public ServiceOrders? ServiceOrder { get; set; }
    public Guid ProductId { get; set; }
    public Products? Product { get; set; }
}
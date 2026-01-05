using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class PurchaseOrderDetails
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid PurchaseOrderDetailId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrders? PurchaseOrder { get; set; }
    public Guid ProductId { get; set; }
    public Products? Product { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class SalesOrderDetails
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid SalesOrderDetailId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid SalesOrderId { get; set; }
    public SalesOrders? SalesOrder { get; set; }
    public Guid ProductId { get; set; }
    public Products? Product { get; set; }
}
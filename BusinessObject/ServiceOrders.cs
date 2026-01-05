using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class ServiceOrders
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid ServiceOrderId { get; set; }
    private DateTime _orderDate;
    public DateTime OrderDate
    {
        get => DateTime.SpecifyKind(_orderDate, DateTimeKind.Utc).ToLocalTime();
        set => _orderDate = value.ToUniversalTime();
    }
    public string MechanicName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string? Note { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid VehicleId { get; set; }
    public Vehicles? Vehicle { get; set; }
    
    public List<ServiceOrderDetails>? ServiceOrderDetails { get; set; }
}
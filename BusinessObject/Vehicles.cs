using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class Vehicles
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid VehicleId { get; set; }
    public string LicensePlate { get; set; }
    public int Year { get; set; }
    public string? Note { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid UserId { get; set; }
    public Users? User { get; set; }
    public Guid VehicleTypeId { get; set; }
    public VehicleTypes? VehicleType { get; set; }
    
    public List<ServiceOrders>? ServiceOrders { get; set; }
}
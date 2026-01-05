using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessObject.Enums;

namespace BusinessObject;

public class VehicleTypes
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid VehicleTypeId { get; set; }
    public string Name { get; set; }
    public string? EngineType { get; set; }
    public VehicleClass VehicleClass { get; set; }
    public string? Note { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid ManufacturerId { get; set; }
    public Manufacturers? Manufacturer { get; set; }
    
    public List<Vehicles>? Vehicles { get; set; }
}
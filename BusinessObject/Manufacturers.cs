using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class Manufacturers
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid ManufacturerId { get; set; }
    public string ManufacturerName { get; set; }
    public string logoUrl { get; set; }
    public bool IsDeleted { get; set; }
    
    public List<VehicleTypes>? VehicleTypes { get; set; }
}
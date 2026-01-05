using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class ProductImage
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid ProductImageId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid ProductId { get; set; }
    public Products? Product { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject;

public class Products
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Unit { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    
    public Guid CategoryId { get; set; }
    public Categories? Category { get; set; }
    
    public Guid? InventoryId { get; set; }
    public Inventory? Inventory { get; set; }
    
    public List<ProductDiscountGroups>? ProductDiscountGroups { get; set; }
    public List<PriceHistory>? PriceHistories { get; set; }
    public List<SalesOrderDetails>? SalesOrderDetails { get; set; }
    public List<PurchaseOrderDetails>? PurchaseOrderDetails { get; set; }
    public List<ServiceOrderDetails>? ServiceOrderDetails { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
}
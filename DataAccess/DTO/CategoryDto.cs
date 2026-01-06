namespace DataAccess.DTO;

public class CategoryDto
{
    public Guid? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsDeleted { get; set; }
}
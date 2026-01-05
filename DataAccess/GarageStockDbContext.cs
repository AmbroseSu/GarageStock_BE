using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataAccess;

public class GarageStockDbContext : DbContext
{
    private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    public GarageStockDbContext()
    {
    }
    
    public GarageStockDbContext(DbContextOptions<GarageStockDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<Categories> Categories { get; set; }
    public virtual DbSet<Discounts> Discounts { get; set; }
    public virtual DbSet<Inventory> Inventories { get; set; }
    public virtual DbSet<MailTemplate> MailTemplates { get; set; }
    public virtual DbSet<SendMail> SendMails { get; set; }
    public virtual DbSet<Manufacturers> Manufacturers { get; set; }
    public virtual DbSet<PriceHistory> PriceHistories { get; set; }
    public virtual DbSet<ProductDiscountGroups> ProductDiscountGroups { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<Products> Products { get; set; }
    public virtual DbSet<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
    public virtual DbSet<PurchaseOrders> PurchaseOrders { get; set; }
    public virtual DbSet<SalesOrderDetails> SalesOrderDetails { get; set; }
    public virtual DbSet<SalesOrders> SalesOrders { get; set; }
    public virtual DbSet<ServiceOrderDetails> ServiceOrderDetails { get; set; }
    public virtual DbSet<ServiceOrders> ServiceOrders { get; set; } 
    public virtual DbSet<Users> Users { get; set; }
    public virtual DbSet<Vehicles> Vehicles { get; set; }
    public virtual DbSet<VehicleTypes> VehicleTypes { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(GetConnectionString())
                .EnableSensitiveDataLogging() // Bật log dữ liệu nhạy cảm
                .UseLoggerFactory(MyLoggerFactory) // Kích hoạt logger
                .EnableDetailedErrors(); // Hiển thị lỗi chi tiết;
    }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        var strConn = config.GetConnectionString("DB");

        return strConn;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categories>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name);
            entity.Property(e => e.Description);
            entity.Property(e => e.IdDeleted);

            entity.HasMany(e => e.Products)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId);
        });

        modelBuilder.Entity<Discounts>(entity =>
        {
            entity.ToTable("Discounts");
            entity.HasKey(e => e.DiscountId);
            entity.Property(e => e.DiscountId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.DiscountName);
            entity.Property(e => e.DiscountPercentage);
            entity.Property(e => e.StartDate);
            entity.Property(e => e.EndDate);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.ProductDiscountGroups)
                .WithOne(e => e.Discount)
                .HasForeignKey(e => e.DiscountId);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventories");
            entity.HasKey(e => e.InventoryId);
            entity.Property(e => e.InventoryId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Quantity);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.IsDeleted);
        });
        
        
        modelBuilder.Entity<MailTemplate>(entity =>
        {
            entity.ToTable("MailTemplates");
            entity.HasKey(e => e.TemplateId);
            entity.Property(e => e.TemplateId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.TemplateCode);
            entity.Property(e => e.Subject);
            entity.Property(e => e.Body);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.UpdatedAt);
            
            entity.HasMany(e => e.SendMails)
                .WithOne(e => e.MailTemplate)
                .HasForeignKey(e => e.MailTemplateId);
        });
        
        modelBuilder.Entity<SendMail>(entity =>
        {
            entity.ToTable("SendMails");
            entity.HasKey(e => e.SendMailId);
            entity.Property(e => e.SendMailId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ToEmail);
            entity.Property(e => e.Subject);
            entity.Property(e => e.Body);
            entity.Property(e => e.Status);
            entity.Property(e => e.ErrorMessage);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.ProcessedAt);
            entity.Property(e => e.SentAt);
            entity.Property(e => e.HangfireJobId);
        });

        modelBuilder.Entity<Manufacturers>(entity =>
        {
            entity.ToTable("Manufacturers");
            entity.HasKey(e => e.ManufacturerId);
            entity.Property(e => e.ManufacturerId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ManufacturerName);
            entity.Property(e => e.logoUrl);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.VehicleTypes)
                .WithOne(e => e.Manufacturer)
                .HasForeignKey(e => e.ManufacturerId);
        });
        
        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.ToTable("PriceHistories");
            entity.HasKey(e => e.PriceHistoryId);
            entity.Property(e => e.PriceHistoryId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.PriceType);
            entity.Property(e => e.OldPrice);
            entity.Property(e => e.NewPrice);
            entity.Property(e => e.ChangeAt);
            entity.Property(e => e.ChangedBy);
            entity.Property(e => e.IsDeleted);
        });
        
        modelBuilder.Entity<ProductDiscountGroups>(entity =>
        {
            entity.ToTable("ProductDiscountGroups");
            entity.HasKey(e => e.ProductDiscountGroupId);
            entity.Property(e => e.ProductDiscountGroupId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedDate);
            entity.Property(e => e.IsDeleted);
        });
        
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImages");
            entity.HasKey(e => e.ProductImageId);
            entity.Property(e => e.ProductImageId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ImageUrl);
            entity.Property(e => e.IsDeleted);
        });
        
        modelBuilder.Entity<Products>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ProductCode);
            entity.Property(e => e.ProductName);
            entity.Property(e => e.Unit);
            entity.Property(e => e.ImageUrl);
            entity.Property(e => e.CostPrice);
            entity.Property(e => e.SellingPrice);
            entity.Property(e => e.DiscountPrice);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.ProductImages)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            entity.HasMany(e => e.PriceHistories)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            entity.HasMany(e => e.SalesOrderDetails)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            entity.HasMany(e => e.PurchaseOrderDetails)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            entity.HasMany(e => e.ServiceOrderDetails)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            entity.HasMany(e => e.ProductDiscountGroups)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            entity.HasOne(e => e.Inventory)
                .WithOne(e => e.Product)
                .HasForeignKey<Inventory>(e => e.InventoryId);
        });
        
        modelBuilder.Entity<PurchaseOrderDetails>(entity =>
        {
            entity.ToTable("PurchaseOrderDetails");
            entity.HasKey(e => e.PurchaseOrderDetailId);
            entity.Property(e => e.PurchaseOrderDetailId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Quantity);
            entity.Property(e => e.UnitPrice);
            entity.Property(e => e.IsDeleted);
        });
        
        modelBuilder.Entity<PurchaseOrders>(entity =>
        {
            entity.ToTable("PurchaseOrders");
            entity.HasKey(e => e.PurchaseOrderId);
            entity.Property(e => e.PurchaseOrderId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.OrderDate);
            entity.Property(e => e.TotalAmount);
            entity.Property(e => e.Discount);
            entity.Property(e => e.Note);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.PurchaseOrderDetails)
                .WithOne(e => e.PurchaseOrder)
                .HasForeignKey(e => e.PurchaseOrderId);
        });
        
        modelBuilder.Entity<SalesOrderDetails>(entity =>
        {
            entity.ToTable("SalesOrderDetails");
            entity.HasKey(e => e.SalesOrderDetailId);
            entity.Property(e => e.SalesOrderDetailId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Quantity);
            entity.Property(e => e.UnitPrice);
            entity.Property(e => e.IsDeleted);
        });
        
        modelBuilder.Entity<SalesOrders>(entity =>
        {
            entity.ToTable("SalesOrders");
            entity.HasKey(e => e.SalesOrderId);
            entity.Property(e => e.SalesOrderId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.OrderDate);
            entity.Property(e => e.TotalAmount);
            entity.Property(e => e.Discount);
            entity.Property(e => e.Note);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.SalesOrderDetails)
                .WithOne(e => e.SalesOrder)
                .HasForeignKey(e => e.SalesOrderId);
        });
        
        modelBuilder.Entity<ServiceOrderDetails>(entity =>
        {
            entity.ToTable("ServiceOrderDetails");
            entity.HasKey(e => e.ServiceOrderDetailId);
            entity.Property(e => e.ServiceOrderDetailId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Quantity);
            entity.Property(e => e.UnitPrice);
            entity.Property(e => e.IsDeleted);
        });
        
        modelBuilder.Entity<ServiceOrders>(entity =>
        {
            entity.ToTable("ServiceOrders");
            entity.HasKey(e => e.ServiceOrderId);
            entity.Property(e => e.ServiceOrderId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.OrderDate);
            entity.Property(e => e.MechanicName);
            entity.Property(e => e.TotalAmount);
            entity.Property(e => e.Discount);
            entity.Property(e => e.Note);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.ServiceOrderDetails)
                .WithOne(e => e.ServiceOrder)
                .HasForeignKey(e => e.ServiceOrderId);
        });
        
        modelBuilder.Entity<Users>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.FullName);
            entity.Property(e => e.Username);
            entity.Property(e => e.Email);
            entity.Property(e => e.Password);
            entity.Property(e => e.PhoneNumber);
            entity.Property(e => e.Address);
            entity.Property(e => e.Avatar);
            entity.Property(e => e.Gender);
            entity.Property(e => e.Role);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.FcmToken);
            entity.Property(e => e.DateOfBirth);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.SalesOrders)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);
            entity.HasMany(e => e.PurchaseOrders)
                .WithOne(e => e.Supplier)
                .HasForeignKey(e => e.SupplierId);
            entity.HasMany(e => e.Vehicles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);
            entity.HasMany(e => e.VerificationOtps)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);
        });
        
        modelBuilder.Entity<Vehicles>(entity =>
        {
            entity.ToTable("Vehicles");
            entity.HasKey(e => e.VehicleId);
            entity.Property(e => e.VehicleId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.LicensePlate);
            entity.Property(e => e.Year);
            entity.Property(e => e.Note);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.ServiceOrders)
                .WithOne(e => e.Vehicle)
                .HasForeignKey(e => e.VehicleId);
        });

        modelBuilder.Entity<VehicleTypes>(entity =>
        {
            entity.ToTable("VehicleTypes");
            entity.HasKey(e => e.VehicleTypeId);
            entity.Property(e => e.VehicleTypeId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name);
            entity.Property(e => e.EngineType);
            entity.Property(e => e.VehicleClass);
            entity.Property(e => e.Note);
            entity.Property(e => e.IsDeleted);

            entity.HasMany(e => e.Vehicles)
                .WithOne(e => e.VehicleType)
                .HasForeignKey(e => e.VehicleTypeId);
        });
        
        modelBuilder.Entity<VerificationOtp>(entity =>
        {
            entity.ToTable("VerificationOtps");
            entity.HasKey(e => e.VerificationOtpId);
            entity.Property(e => e.VerificationOtpId)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.OtpCode);
            entity.Property(e => e.ExpirationTime);
            entity.Property(e => e.IsTrue);
            entity.Property(e => e.IsDeleted);
            entity.Property(e => e.AttemptCount);
        });

    }
    
}
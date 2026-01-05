using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repository.Impl;

public class UnitOfWork : IUnitOfWork
{
    private readonly GarageStockDbContext _context;
    private bool _disposed;
    
    public UnitOfWork(GarageStockDbContext context, ICategoryRepository categoryUow, IDiscountRepository discountUow, IInventoryRepository inventoryUow, IManufacturerRepository manufacturerUow, IPriceHistoryRepository priceHistoryUow, IProductDiscountGroupsRepository productDiscountGroupsUow, IProductImageRepository productImageUow, IProductRepository productUow, IPurchaseOrderDetailsRepository purchaseOrderDetailsUow, IPurchaseOrdersRepository purchaseOrderUow, ISalesOrderDetailsRepository salesOrderDetailsUow, ISalesOrdersRepository salesOrderUow, IServiceOrderDetailsReposiotry serviceOrderDetailsUow, IServiceOrdersRepository serviceOrderUow, IUserRepository userUow, IVehiclesRepository vehiclesUow, IVehicleTypesRepository vehicleTypesUow, IVerificationOtpRepository verificationOtpUow, IMailTemplateRepository mailTemplateUow, ISendMailRepository sendMailUow)
    {
        _context = context;
        CategoryUOW = categoryUow ?? throw new ArgumentNullException(nameof(categoryUow));
        DiscountUOW = discountUow ?? throw new ArgumentNullException(nameof(discountUow));
        InventoryUOW = inventoryUow ?? throw new ArgumentNullException(nameof(inventoryUow));
        ManufacturerUOW = manufacturerUow ?? throw new ArgumentNullException(nameof(manufacturerUow));
        PriceHistoryUOW = priceHistoryUow ?? throw new ArgumentNullException(nameof(priceHistoryUow));
        ProductDiscountGroupsUOW = productDiscountGroupsUow ?? throw new ArgumentNullException(nameof(productDiscountGroupsUow));
        ProductImageUOW = productImageUow ?? throw new ArgumentNullException(nameof(productImageUow));
        ProductUOW = productUow ?? throw new ArgumentNullException(nameof(productUow));
        PurchaseOrderDetailsUOW = purchaseOrderDetailsUow ?? throw new ArgumentNullException(nameof(purchaseOrderDetailsUow));
        PurchaseOrderUOW = purchaseOrderUow ?? throw new ArgumentNullException(nameof(purchaseOrderUow));
        SalesOrderDetailsUOW = salesOrderDetailsUow ?? throw new ArgumentNullException(nameof(salesOrderDetailsUow));
        SalesOrderUOW = salesOrderUow ?? throw new ArgumentNullException(nameof(salesOrderUow));
        ServiceOrderDetailsUOW = serviceOrderDetailsUow ?? throw new ArgumentNullException(nameof(serviceOrderDetailsUow));
        ServiceOrderUOW = serviceOrderUow ?? throw new ArgumentNullException(nameof(serviceOrderUow));
        UserUOW = userUow ?? throw new ArgumentNullException(nameof(userUow));
        VehiclesUOW = vehiclesUow ?? throw new ArgumentNullException(nameof(vehiclesUow));
        VehicleTypesUOW = vehicleTypesUow ?? throw new ArgumentNullException(nameof(vehicleTypesUow));
        VerificationOtpUOW = verificationOtpUow ?? throw new ArgumentNullException(nameof(verificationOtpUow));
        MailTemplateUOW = mailTemplateUow;
        SendMailUOW = sendMailUow;
        _disposed = false;
    }
    
    public ICategoryRepository CategoryUOW { get; }
    public IDiscountRepository DiscountUOW { get; }
    public IInventoryRepository InventoryUOW { get; }
    public IMailTemplateRepository MailTemplateUOW { get; }
    public IManufacturerRepository ManufacturerUOW { get; }
    public IPriceHistoryRepository PriceHistoryUOW { get; }
    public IProductDiscountGroupsRepository ProductDiscountGroupsUOW { get; }
    public IProductImageRepository ProductImageUOW { get; }
    public IProductRepository ProductUOW { get; }
    public IPurchaseOrderDetailsRepository PurchaseOrderDetailsUOW { get; }
    public IPurchaseOrdersRepository PurchaseOrderUOW { get; }
    public ISalesOrderDetailsRepository SalesOrderDetailsUOW { get; }
    public ISalesOrdersRepository SalesOrderUOW { get; }
    public ISendMailRepository SendMailUOW { get; }
    public IServiceOrderDetailsReposiotry ServiceOrderDetailsUOW { get; }
    public IServiceOrdersRepository ServiceOrderUOW { get; }
    public IUserRepository UserUOW { get; }
    public IVehiclesRepository VehiclesUOW { get; }
    public IVehicleTypesRepository VehicleTypesUOW { get; }
    public IVerificationOtpRepository VerificationOtpUOW { get; }
    
    public async Task<int> SaveChangesAsync()
    {
        //using var context = new DocumentManagementSystemDbContext();
        //return await _context.SaveChangesAsync();
        if (_context.ChangeTracker.HasChanges())
            return await _context.SaveChangesAsync();
        // Không có thay đổi, có thể log hoặc trả về 0
        return 0;
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _context.Database.SetCommandTimeout(600);
        return await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    if (_context.Database.CurrentTransaction != null)
                    {
                        _context.Database.CurrentTransaction.Dispose();
                    }
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
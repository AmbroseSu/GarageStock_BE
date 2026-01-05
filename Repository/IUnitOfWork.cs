using Microsoft.EntityFrameworkCore.Storage;

namespace Repository;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository CategoryUOW { get; }
    IDiscountRepository DiscountUOW { get; }
    IInventoryRepository InventoryUOW { get; }
    IMailTemplateRepository MailTemplateUOW { get; }
    IManufacturerRepository ManufacturerUOW { get; }
    IPriceHistoryRepository PriceHistoryUOW { get; }
    IProductDiscountGroupsRepository ProductDiscountGroupsUOW { get; }
    IProductImageRepository ProductImageUOW { get; }
    IProductRepository ProductUOW { get; }
    IPurchaseOrderDetailsRepository PurchaseOrderDetailsUOW { get; }
    IPurchaseOrdersRepository PurchaseOrderUOW { get; }
    ISalesOrderDetailsRepository SalesOrderDetailsUOW { get; }
    ISalesOrdersRepository SalesOrderUOW { get; }
    ISendMailRepository SendMailUOW { get; }
    IServiceOrderDetailsReposiotry ServiceOrderDetailsUOW { get; }
    IServiceOrdersRepository ServiceOrderUOW { get; }
    IUserRepository UserUOW { get; }
    IVehiclesRepository VehiclesUOW { get; }
    IVehicleTypesRepository VehicleTypesUOW { get; }
    IVerificationOtpRepository VerificationOtpUOW { get; }
    
    Task<int> SaveChangesAsync();
    
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    
}
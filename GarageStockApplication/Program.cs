using System.Text;
using DataAccess;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Repository.Impl;
using Service;
using Service.Impl;
using Service.Jobs;
using Service.Utilities;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Your API", Version = "v1" });

    // Configure Swagger to use the Bearer token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false; // Cần bật true nếu chạy production
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Thêm kiểm tra thời gian sống của token
            ValidateIssuerSigningKey = true, // Đảm bảo kiểm tra khóa ký token
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key"))
            ),
            //RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero // Giảm độ trễ token xuống 0 để token hết hạn đúng thời điểm
        };
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                // Cho phép lấy access token qua query (dành cho SignalR)
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500"
            ) // địa chỉ chạy file HTML
            .AllowAnyHeader()
            .AllowAnyMethod() 
            .AllowCredentials(); // rất quan trọng cho SignalR
    });
});

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DB"));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(
        builder.Configuration["Redis:ConnectionString"]!
    )
);


builder.Services.AddHangfireServer();

builder.Services.AddDbContext<GarageStockDbContext>();
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<SendMailJob>();
/*builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
builder.Services.AddScoped<IProductDiscountGroupsRepository, ProductDiscountGroupsRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPurchaseOrderDetailsRepository, PurchaseOrderDetailsRepository>();
builder.Services.AddScoped<IPurchaseOrdersRepository, PurchaseOrdersRepository>();
builder.Services.AddScoped<ISalesOrderDetailsRepository, SalesOrderDetailsRepository>();
builder.Services.AddScoped<ISalesOrdersRepository, SalesOrdersRepository>();
builder.Services.AddScoped<IServiceOrderDetailsReposiotry, ServiceOrderDetailsRepository>();
builder.Services.AddScoped<IServiceOrdersRepository, ServiceOrdersRepository>();
builder.Services.AddScoped<IVehiclesRepository, VehiclesRepository>();
builder.Services.AddScoped<IVehicleTypesRepository, VehicleTypesRepository>();
builder.Services.AddScoped<IVerificationOtpRepository, VerificationOtpRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();*/

builder.Services.Scan(scan => scan
    .FromAssemblies(
        typeof(UserRepository).Assembly,
        typeof(AuthenticationService).Assembly
    )
    .AddClasses(classes => classes
        .Where(type =>
            type.Name.EndsWith("Repository") ||
            type.Name.EndsWith("Service")
        )
    )
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);



builder.WebHost.UseKestrel();
var app = builder.Build();

app.UseHangfireDashboard("/hangfire");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
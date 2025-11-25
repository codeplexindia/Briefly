using Briefly.Business.Interfaces;
using Briefly.Business.Services;
using Briefly.DataAccess.Interfaces;
using Briefly.DataAccess.Repositories;
using Briefly.Database;
using Briefly.Model.Entities;
using Briefly.Model.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BrieflyDbContext>(db => db.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Register dependencies
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Briefly API",
        Version = "v1"
    });

    // Add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT token below.\nExample: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
});


// Register AutoMapper services, scanning the current assembly for profiles
builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use authentication middleware
app.UseAuthentication();

// Use authorization middleware
app.UseAuthorization();

app.MapControllers();

// Perform database migrations and seed data
await MigrateDatabase(app);

app.Run();

// Method to handle migration and seeding
async Task MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BrieflyDbContext>();

    // Ensure the database is created
    await context.Database.MigrateAsync();

    // Seed Roles if they don't exist
    await SeedRoles(context);

    // Seed the Admin user if it doesn't exist
    await SeedAdminUser(context);
}

async Task SeedRoles(BrieflyDbContext context)
{
    if (await context.Roles.AnyAsync())
        return;

    var roles = new List<Role>
    {
        new() {
            Id = Guid.Parse("6f6533f0-dab1-4c96-b1a9-09d368b96366"),
            RoleName = "Admin",
            Permissions = "[\"CanManageUsers\", \"CanManageProducts\", \"CanViewReports\", \"CanManageRoles\", \"CanViewOrders\", \"CanManageInventory\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Administrator with full control over all system features."
        },
        new() {
            Id = Guid.Parse("e621bd67-380b-4119-951b-3e8394b750e7"),
            RoleName = "Guest",
            Permissions = "[\"CanViewProductCatalog\", \"CanMakePurchases\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Guests who can browse products and make purchases."
        },
        new() {
            Id = Guid.Parse("586fd14e-8280-43b8-9508-58a5aa31229d"),
            RoleName = "Supplier",
            Permissions = "[\"CanViewProductCatalog\", \"CanUpdateSupplyStatus\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Suppliers who view product details and update supply status."
        },
        new() {
            Id = Guid.Parse("fa6acfe5-527b-433d-b460-59573401d764"),
            RoleName = "Cashier",
            Permissions = "[\"CanProcessSales\", \"CanGenerateReceipts\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Cashier responsible for processing transactions and receipts."
        },
        new() {
            Id = Guid.Parse("23b1b520-f4d8-4c6e-bc82-be84d62c26a8"),
            RoleName = "Sales Associate",
            Permissions = "[\"CanProcessSales\", \"CanViewProductCatalog\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Sales associate assisting customers and processing sales."
        },
        new() {
            Id = Guid.Parse("24febd71-5261-4632-b963-c93fc1cc3b85"),
            RoleName = "Inventory Manager",
            Permissions = "[\"CanManageInventory\", \"CanViewProductCatalog\", \"CanManageSupplierOrders\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Manages stock levels, inventory, and supplier orders."
        },
        new() {
            Id = Guid.Parse("0cdaa7c7-3ccc-42f1-873c-d2054bd39ae2"),
            RoleName = "Customer Support",
            Permissions = "[\"CanProcessReturns\", \"CanViewCustomerOrderHistory\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Handles customer queries, returns, and support."
        },
        new() {
            Id = Guid.Parse("ece882c2-fe6e-45b5-81ae-d974a3bc86f5"),
            RoleName = "Marketing",
            Permissions = "[\"CanManageOffers\", \"CanViewSalesData\", \"CanViewCustomerData\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Manages promotions, offers, and analytics."
        },
        new() {
            Id = Guid.Parse("5533ae57-5640-484f-989c-f01b13b20b8e"),
            RoleName = "Manager",
            Permissions = "[\"CanManageProducts\", \"CanViewReports\", \"CanViewOrders\", \"CanManageInventory\"]",
            CreatedAt = DateTime.UtcNow,
            Description = "Manager overseeing product, report, order, and inventory processes."
        }
    };

    await context.Roles.AddRangeAsync(roles);
    await context.SaveChangesAsync();
}


// Method to seed Admin user if not exists
async Task SeedAdminUser(BrieflyDbContext context)
{
    var existingAdmin = await context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Username == "admin");

    if (existingAdmin != null)
        return;

    var adminRole = await context.Roles
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.RoleName == "Admin") ?? throw new Exception("Admin role not found. Make sure roles are seeded before creating admin user.");

    var adminUser = new User
    {
        Id = Guid.NewGuid(),
        Username = "admin",
        Email = "admin@store.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
        IsActive = true,
        RoleId = adminRole.Id,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    await context.Users.AddAsync(adminUser);
    await context.SaveChangesAsync();
}

using CinemaManagement.Common;
using CinemaManagement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure EF Core with SQLite
builder.Services.AddDbContext<CinemaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=cinema.db"));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<CinemaDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345!";
var issuer = jwtSettings["Issuer"] ?? "CinemaManagementAPI";
var audience = jwtSettings["Audience"] ?? "CinemaManagementClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Configure Authorization
builder.Services.AddAuthorization();

// Register repositories as Scoped
builder.Services.AddScoped<IRepository<Movie>, EfRepository<Movie>>();
builder.Services.AddScoped<IRepository<Cartoon>, EfRepository<Cartoon>>();

// Register CRUD services as Scoped
builder.Services.AddScoped<ICrudServiceAsync<Movie>, EfCrudServiceAsync<Movie>>();
builder.Services.AddScoped<ICrudServiceAsync<Cartoon>, EfCrudServiceAsync<Cartoon>>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cinema Management API v1");
    options.RoutePrefix = "swagger"; // Swagger UI at /swagger
});

app.UseHttpsRedirection();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database and seed roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CinemaDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Seed roles and users
        await SeedDataAsync(roleManager, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

// Method to seed roles and test users
static async Task SeedDataAsync(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
{
    // Seed roles
    string[] roles = { "Admin", "Manager", "User" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed test users
    // Admin user
    if (await userManager.FindByEmailAsync("admin@cinema.com") == null)
    {
        var adminUser = new User
        {
            UserName = "admin@cinema.com",
            Email = "admin@cinema.com",
            FullName = "Administrator",
            RegisteredAt = DateTime.UtcNow,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Manager user
    if (await userManager.FindByEmailAsync("manager@cinema.com") == null)
    {
        var managerUser = new User
        {
            UserName = "manager@cinema.com",
            Email = "manager@cinema.com",
            FullName = "Cinema Manager",
            RegisteredAt = DateTime.UtcNow,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(managerUser, "Manager123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(managerUser, "Manager");
        }
    }

    // Regular user
    if (await userManager.FindByEmailAsync("user@cinema.com") == null)
    {
        var regularUser = new User
        {
            UserName = "user@cinema.com",
            Email = "user@cinema.com",
            FullName = "Regular User",
            RegisteredAt = DateTime.UtcNow,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(regularUser, "User123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(regularUser, "User");
        }
    }
}

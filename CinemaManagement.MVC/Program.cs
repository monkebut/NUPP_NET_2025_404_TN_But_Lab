using CinemaManagement.Common;
using CinemaManagement.MongoDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// MongoDB Configuration
// Register MongoDbContext as Singleton
builder.Services.AddSingleton<MongoDbContext>();

// Register MongoDB repository for Customer as Scoped
builder.Services.AddScoped<IRepository<Customer>>(provider =>
{
    var context = provider.GetRequiredService<MongoDbContext>();
    return new MongoRepository<Customer>(context.Database);
});

// Register CRUD service for Customer as Scoped
builder.Services.AddScoped<ICrudServiceAsync<Customer>>(provider =>
{
    var repository = provider.GetRequiredService<IRepository<Customer>>();
    return new MongoCrudServiceAsync<Customer>(repository);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

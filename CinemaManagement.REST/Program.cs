using CinemaManagement.Common;
using CinemaManagement.MongoDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen();

// MongoDB Configuration
// Register MongoDbContext as Singleton
builder.Services.AddSingleton<MongoDbContext>();

// Register MongoDB repositories as Scoped
builder.Services.AddScoped<IRepository<Movie>>(provider =>
{
    var context = provider.GetRequiredService<MongoDbContext>();
    return new MongoRepository<Movie>(context.Database);
});

builder.Services.AddScoped<IRepository<Cartoon>>(provider =>
{
    var context = provider.GetRequiredService<MongoDbContext>();
    return new MongoRepository<Cartoon>(context.Database);
});

// Register CRUD services as Scoped
builder.Services.AddScoped<ICrudServiceAsync<Movie>>(provider =>
{
    var repository = provider.GetRequiredService<IRepository<Movie>>();
    return new MongoCrudServiceAsync<Movie>(repository);
});

builder.Services.AddScoped<ICrudServiceAsync<Cartoon>>(provider =>
{
    var repository = provider.GetRequiredService<IRepository<Cartoon>>();
    return new MongoCrudServiceAsync<Cartoon>(repository);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cinema Management API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

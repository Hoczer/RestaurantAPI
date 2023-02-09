using RestaurantAPI;
using NLog.Web;
using RestaurantAPI.Entities;
using System.Reflection;
using RestaurantAPI.Service;
using NLog;
using NLog.Web;
using RestaurantAPI.Middleware;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    logger.Debug("init main");

    // Add services to the container.
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddDbContext<RestaurantDbContext>();
    builder.Services.AddScoped<RestaurantSeeder>();
    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
    builder.Services.AddScoped<IRestaurantService, RestaurantService>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<RequestTimeMiddleware>();
    builder.Services.AddScoped<IDishService, DishService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    var scoope = app.Services.CreateScope();
    var seeder = scoope.ServiceProvider.GetService<RestaurantSeeder>();
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<RequestTimeMiddleware>();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
    });
    app.UseAuthorization();

    app.MapControllers();
    seeder.Seed();
    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}


using RestaurantAPI;
using NLog.Web;
using RestaurantAPI.Entities;
using System.Reflection;
using RestaurantAPI.Service;
using NLog;
using RestaurantAPI.Middleware;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RestaurantAPI.Authorization;
using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    var authenticationSettings = new AuthenticationSettings();
    builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

    logger.Debug("init main");

    // Add services to the container.
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddFluentValidationAutoValidation();
   builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
    builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
    builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();


    builder.Services.AddScoped<RestaurantSeeder>();
    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
    builder.Services.AddScoped<IRestaurantService, RestaurantService>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<RequestTimeMiddleware>();
    builder.Services.AddScoped<IDishService, DishService>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();
    builder.Services.AddScoped<IValidator<RegisterUserDto>,RegisterUserDtoValidator>();
    builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
    builder.Services.AddScoped<IUserContextService, UserContextService>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    }).AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = authenticationSettings.JwtIssuer,
            ValidAudience = authenticationSettings.JwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
        };
    });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish"));
        options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
        options.AddPolicy("CreatedAtleast2Restaurants",
            builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));
    });
    builder.Services.AddSingleton(authenticationSettings);
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontEndClient", policyBuilder =>

            policyBuilder.AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(builder.Configuration["AllowedOrigins"])

            );
    });

    builder.Services.AddDbContext<RestaurantDbContext>
        (options => options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection")));


    var app = builder.Build();

    // Configure the HTTP request pipeline.

    var scoope = app.Services.CreateScope();
    var seeder = scoope.ServiceProvider.GetService<RestaurantSeeder>();
    app.UseResponseCaching();
    app.UseStaticFiles();
    app.UseCors("FrontEndClient");
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<RequestTimeMiddleware>();
    app.UseAuthentication();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
    });
    app.UseRouting();

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


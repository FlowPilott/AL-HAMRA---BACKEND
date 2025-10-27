using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using WAS_Management.Data;
using WAS_Management.Middleware;
using WAS_Management.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/alhamra-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Register Serilog with the host builder
builder.Host.UseSerilog();

// Add CORS policy
//builder.Services.AddCors();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Add services to the container.
builder.Services.AddControllers();

// Add database context
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information("Application starting with connection string configured: {HasConnection}", !string.IsNullOrEmpty(conn));
builder.Services.AddDbContext<WAS_ManagementContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// Add authentication and JWT configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secret = jwtSettings["Secret"];

    if (string.IsNullOrWhiteSpace(secret))
    {
        Log.Fatal("JWT Secret is missing in configuration");
        throw new InvalidOperationException("JWT Secret is missing in configuration.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add authorization
builder.Services.AddAuthorization();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Version = "v1",
//        Title = "WAS Management API",
//        Description = "An ASP.NET Core Web API for WAS Management",
//        TermsOfService = new Uri("https://example.com/terms"),
//        Contact = new Microsoft.OpenApi.Models.OpenApiContact
//        {
//            Name = "Support Team",
//            Email = "support@example.com",
//            Url = new Uri("https://example.com/contact"),
//        },
//        License = new Microsoft.OpenApi.Models.OpenApiLicense
//        {
//            Name = "Use under LICX",
//            Url = new Uri("https://example.com/license"),
//        }
//    });
//});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
});
builder.WebHost.UseUrls("http://0.0.0.0:6000");
var app = builder.Build();

// Exception handling middleware (should be first)
app.UseMiddleware<ExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline.
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WAS Management API V1");
//});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    Log.Information("Application running in Development mode");
}
else
{
    Log.Information("Application running in Production mode");
}

app.UseStaticFiles();

// Add request logging middleware (add this early in the pipeline)
app.UseRequestLogging();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseMiddleware<CustomJwtSecurityMiddleware>();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting WAS Management web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "WAS Management application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WAS_Management.Data;
using WAS_Management.Middleware;
using WAS_Management.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors();

// Add services to the container.
builder.Services.AddControllers();

// Add database context
builder.Services.AddDbContext<WAS_ManagementContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "My API",
        Description = "An example ASP.NET Core Web API",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Support Team",
            Email = "support@example.com",
            Url = new Uri("https://example.com/contact"),
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });
});


//var uploadsDirectory = builder.Configuration.GetValue<string>("dllpaths:libwkhtmltox");
//var context = new CustomAssemblyLoadContext();
//context.LoadUnmanagedLibrary(uploadsDirectory);


builder.WebHost.UseUrls("http://0.0.0.0:6000");


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
//// Middleware to handle CORS issues
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
//    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
//    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
//    if (context.Request.Method == "OPTIONS")
//    {
//        context.Response.StatusCode = 200;
//        return;
//    }
//    await next();
//});

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseMiddleware<CustomJwtSecurityMiddleware>();
app.UseAuthorization();

app.MapControllers();





app.Run();

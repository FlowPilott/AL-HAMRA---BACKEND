using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WAS_Management.ViewModels;

namespace WAS_Management.Middleware
{
    public class CustomJwtSecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomJwtSecurityMiddleware> _logger;
        private readonly JwtSettings _jwtSettings;

        public CustomJwtSecurityMiddleware(RequestDelegate next, ILogger<CustomJwtSecurityMiddleware> logger, IOptions<JwtSettings> jwtSettings)
        {
            _next = next;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // Check if the endpoint allows anonymous access
            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>();
            if (allowAnonymous != null)
            {
                // Bypass JWT validation for endpoints marked with [AllowAnonymous]
                await _next(context);
                return;
            }


            var remoteIp = context.Connection.RemoteIpAddress;

            // Check if the IP is in the allowed list
            //if (IsAllowedIp(remoteIp))
            //{
            //    _logger.LogInformation("Bypassing JWT authentication for allowed IP: {IP}", remoteIp);
            //    await _next(context); // Skip JWT checks and continue
            //    return;
            //}
            // Bypass the middleware for token generation endpoint
            if (context.Request.Path.StartsWithSegments("/api/Security/Index"))
            {
                await _next(context); // Allow the token generation request to proceed
                return;
            }
            if (context.Request.Path.StartsWithSegments("/api/Securrity/Login"))
            {
                await _next(context); // Allow the token generation request to proceed
                return;
            }
            if (context.Request.Path.StartsWithSegments("/api/swagger/v1/swagger.json"))
            {
                await _next(context); // Allow the token generation request to proceed
                return;
            }

            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context); // Allow the token generation request to proceed
                return;
            }
            if (context.Request.Path.StartsWithSegments("/swagger/index.html"))
            {
                await _next(context); // Allow the token generation request to proceed
                return;
            }



            // Check if the Authorization header is present
            if (context.Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                var tokenValue = tokenHeader.ToString().Split(" ").Last(); // Get the token from the header

                // Validate the token
                var principal = ValidateToken(tokenValue);
                if (principal != null)
                {
                    context.User = principal;
                }
                else
                {
                    _logger.LogWarning("Invalid token.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token.");
                    return;
                }
            }
            else
            {
                _logger.LogWarning("Authorization header missing.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("You are not authorized to call. Authorization header missing.");
                return;
            }

            // Log user's identity and claims for auditing
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"User {userId} is accessing {context.Request.Path}.");

            // Additional claim validation
            //var hasRequiredClaim = context.User.HasClaim(c => c.Type == "scope" && c.Value == "admin");
            //if (!hasRequiredClaim)
            //{
            //    _logger.LogWarning($"User {userId} lacks the necessary scope for this request.");
            //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //    await context.Response.WriteAsync("Insufficient scope.");
            //    return;
            //}

            // Call the next middleware if all checks pass
            await _next(context);
        }
        //private bool IsAllowedIp(IPAddress remoteIp)
        //{
        //    if (remoteIp != null)
        //    {
        //        return _allowedIPs.LocalIPs.Any(ip => IPAddress.TryParse(ip, out var parsedIp).Equals(remoteIp));
        //    }
        //    return false;
        //}
      

        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret); // Use the key from configuration

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Remove delay of token when expire
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                return null; // Token is not valid
            }
        }
    }

}

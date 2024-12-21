using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WAS_Management.Data;
using WAS_Management.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WAS_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurrityController : ControllerBase
    {
        private readonly WAS_ManagementContext _context;
        private readonly IConfiguration _configuration;

        private readonly JwtSettings _jwtSettings;
        public SecurrityController(WAS_ManagementContext context, IConfiguration configuration, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
        }
        [HttpPost("Login")]
        public async Task<JsonResult> Login(string username, string password)
        {
            try
            {
                // Fetch user by username
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

                // If user is not found
                if (user == null)
                {
                    var errorData = new
                    {
                        Result = "",
                        ErrorCode = "405",
                        ErrorMessage = "Username not found.",
                        Data = ""
                    };
                    //return Json(errorDa.ta);
                    return new JsonResult(errorData);
                }

                // Verify password (assuming plaintext for simplicity; use proper hashing in production)
                if (user.Password != password)
                {
                    var errorData = new
                    {
                        Result = "",
                        ErrorCode = "403",
                        ErrorMessage = "Invalid password.",
                        Data = ""
                    };
                    return new JsonResult(errorData);
                }
                var token = await generateToken();
                // Success response
                var successData = new
                {
                    Result = "Login Successful",
                    ErrorCode = "200",
                    ErrorMessage = "",
                    Data = user,
                    Tokken = token
                };
                return new JsonResult(successData);
            }
            catch (Exception ex)
            {
                // Log the exception (replace with your logging mechanism)
                Console.WriteLine(ex);

                // Error response
                var errorData = new
                {
                    Result = "",
                    ErrorCode = "500",
                    ErrorMessage = "An unexpected error occurred.",
                    Data = ""
                };
                return new JsonResult(errorData);
            }
        }
        private async Task<string> generateToken()
        {
            // var jwtSettings = _configuration.GetSection("JwtSettings");
            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;
            var secretKey = _jwtSettings.Secret;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }
}

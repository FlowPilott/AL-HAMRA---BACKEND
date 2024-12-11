using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WAS_Management.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WAS_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurrityController : ControllerBase
    {
        private readonly WAS_ManagementContext _context;
        private readonly IConfiguration _configuration;

        public SecurrityController(WAS_ManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                    //return Json(errorData);
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

                // Success response
                var successData = new
                {
                    Result = "Login Successful",
                    ErrorCode = "200",
                    ErrorMessage = "",
                    Data = user
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

    }
}

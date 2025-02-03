using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAS_Management.Data;
using WAS_Management.Models;

namespace WAS_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetupController : Controller
    {
        private readonly WAS_ManagementContext _context;
        private readonly IConfiguration _configuration;


        public SetupController(WAS_ManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("units")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnits(
      string? searchTerm = null,
      int page = 1,
      int pageSize = 20)
        {
            // Start by getting all records
            IQueryable<Unit> query = _context.Units.AsQueryable();

            // If there's a search term, filter by it
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // For case-insensitive matching, convert both sides to lowercase
                var lowerTerm = searchTerm.ToLower();

                query = query.Where(u =>
                    (u.Slno != null && u.Slno.ToLower().Contains(lowerTerm)) ||
                    (u.Unitid != null && u.Unitid.ToLower().Contains(lowerTerm)) ||
                    (u.Sapno != null && u.Sapno.ToLower().Contains(lowerTerm)) ||
                    (u.Newnumber != null && u.Newnumber.ToLower().Contains(lowerTerm))
                );
            }

            // Order, skip, and take for pagination
            var skip = (page - 1) * pageSize;
            var results = await query
                .OrderBy(u => u.Id)  // Or any other ordering
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return results;
        }





        [HttpGet("GetInteractionTypes")]
        public async Task<ActionResult<IEnumerable<InteractionType>>> GetInteractionTypes()
        {
            // Start by getting all records
            var query = await _context.InteractionTypes.ToListAsync();

            return query;
        }



        [HttpPost("AddInteractionType/{name}")]
        public async Task<ActionResult<bool>> AddInteractionType(string name)
        {
            try
            {
                InteractionType intr = new InteractionType()
                {
                    Name = name
                };

                _context.InteractionTypes.Add(intr);
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}

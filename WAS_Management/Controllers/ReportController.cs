using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAS_Management.Data;
using WAS_Management.Models;

namespace WAS_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {

        private readonly WAS_ManagementContext _context;
        private readonly IConfiguration _configuration;


        public ReportController(WAS_ManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("interactions")]
        public async Task<ActionResult<IEnumerable<Interaction>>> GetInteractions(
       string? interactionType = null,
       string? customerName = null,
       string? unitNumber = null,
       int page = 1,
       int pageSize = 20)
        {
            IQueryable<Interaction> query = _context.Interactions.AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(interactionType))
            {
                query = query.Where(i => i.TypeOfInteraction != null &&
                                         i.TypeOfInteraction.ToLower().Contains(interactionType.ToLower()));
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(i => i.CustomerName != null &&
                                         i.CustomerName.ToLower().Contains(customerName.ToLower()));
            }

            if (!string.IsNullOrEmpty(unitNumber))
            {
                query = query.Where(i => i.UnitNumber != null &&
                                         i.UnitNumber.ToLower().Contains(unitNumber.ToLower()));
            }

            // Pagination
            var skip = (page - 1) * pageSize;
            var results = await query
                .OrderBy(i => i.Id) // Change ordering if needed
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return Ok(results);
        }
    }
}

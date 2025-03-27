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
       string? typeOfInteraction = null,
       int page = 1,
       int pageSize = 20)
        {

            
            IQueryable<Interaction> query = _context.Interactions.AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(interactionType))
            {
                query = query.Where(i => i.PurposeOfInteraction != null &&
                                         i.PurposeOfInteraction.Trim().ToLower().Contains(interactionType.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(i => i.CustomerName != null &&
                                         i.CustomerName.Trim().ToLower().Contains(customerName.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(unitNumber))
            {
                query = query.Where(i => i.UnitNumber != null &&
                                         i.UnitNumber.Trim().ToLower().Contains(unitNumber.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(typeOfInteraction))
            {
                query = query.Where(i => i.TypeOfInteraction != null &&
                                         i.TypeOfInteraction.Trim().ToLower().Contains(typeOfInteraction.Trim().ToLower()));
            }

            // Pagination
            var skip = (page - 1) * pageSize;
            var results = await query
                .OrderByDescending(i => i.Id) // Change ordering if needed
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return Ok(results);
        }
    }
}

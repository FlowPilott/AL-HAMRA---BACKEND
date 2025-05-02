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
        [HttpGet("resalenocs")]
        public async Task<ActionResult<IEnumerable<Resalenoc>>> GetResaleNOCs(
    string? mastercomm = null,
    string? projectname = null,
    string? subprojectname = null,
    string? unitno = null,
    string? customername = null,
    string? email = null,
    int page = 1,
    int pageSize = 20)
        {
            IQueryable<Resalenoc> query = _context.Resalenocs.AsQueryable();

            if (!string.IsNullOrEmpty(mastercomm))
            {
                query = query.Where(r => r.Mastercomm != null &&
                                         r.Mastercomm.Trim().ToLower().Contains(mastercomm.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(projectname))
            {
                query = query.Where(r => r.Projectname != null &&
                                         r.Projectname.Trim().ToLower().Contains(projectname.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(subprojectname))
            {
                query = query.Where(r => r.Subprojectname != null &&
                                         r.Subprojectname.Trim().ToLower().Contains(subprojectname.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(unitno))
            {
                query = query.Where(r => r.Unitno != null &&
                                         r.Unitno.Trim().ToLower().Contains(unitno.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(customername))
            {
                query = query.Where(r => r.Customername != null &&
                                         r.Customername.Trim().ToLower().Contains(customername.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(r => r.Email != null &&
                                         r.Email.Trim().ToLower().Contains(email.Trim().ToLower()));
            }

            // Pagination
            var skip = (page - 1) * pageSize;
            var results = await query
                .OrderByDescending(r => r.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return Ok(results);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
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

        [HttpGet("contractors")]
        public async Task<ActionResult<IEnumerable<Contractor>>> GetContractors(
    string? searchTerm = null,
    int page = 1,
    int pageSize = 20)
        {
            // Start by getting all records
            IQueryable<Contractor> query = _context.Contractors.AsQueryable();

            // If there's a search term, filter by it
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerTerm = searchTerm.ToLower();

                query = query.Where(c =>
                    (c.CompanyName != null && c.CompanyName.ToLower().Contains(lowerTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(lowerTerm)) ||
                    (c.Mobileno != null && c.Mobileno.ToLower().Contains(lowerTerm)) ||
                    (c.Landlineno != null && c.Landlineno.ToLower().Contains(lowerTerm)) ||
                    (c.Address != null && c.Address.ToLower().Contains(lowerTerm)) ||
                    (c.Bpnumber != null && c.Bpnumber.ToLower().Contains(lowerTerm)) ||
                    (c.VehicleReg != null && c.VehicleReg.ToLower().Contains(lowerTerm))
                );
            }

            // Order, skip, and take for pagination
            var skip = (page - 1) * pageSize;
            var results = await query
                .OrderByDescending(c => c.Id)  // Or any other ordering
                .Skip(skip)
                .Take(pageSize)
                .Where(x => x.RenewalDate != null)
                .ToListAsync();

            return results;
        }


        [HttpGet("GetAllContractors")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Contractor>>> GetContractors()
        {
            IQueryable<Contractor> query = _context.Contractors.AsQueryable();


            return await query.ToListAsync();
        }

        [HttpPost("UpdateContractorBpNumber/{id}/{newBpNumber}")]
        public async Task<IActionResult> UpdateContractorBpNumber(int id, string newBpNumber)
        {
            // Find the contractor by ID
            var contractor = await _context.Contractors.FindAsync(id);

            if (contractor == null)
            {
                return NotFound(new { message = "Contractor not found" });
            }

            // Update BP Number
            contractor.Bpnumber = newBpNumber;

            // Save changes to database
            await _context.SaveChangesAsync();

            return Ok(new { message = "BP Number updated successfully", contractor });
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

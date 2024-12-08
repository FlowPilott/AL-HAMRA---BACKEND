using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using WAS_Management.Models;
using WAS_Management.Data;


namespace WAS_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormsController : ControllerBase
    {
        private readonly WAS_ManagementContext _context;
        private readonly IConfiguration _configuration;

        public FormsController(WAS_ManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("interactions")]
        public async Task<ActionResult<IEnumerable<Interaction>>> GetInteractions()
        {
            return await _context.Interactions.ToListAsync();
        }

        [HttpPost("create")]
        public async Task<ActionResult<Interaction>> CreateInteraction([FromBody] Interaction interaction)
        {
            if (interaction.FollowUp == "yes" && interaction.FollowUpDate == null)
            {
                return BadRequest("Follow-up date is required when follow-up is yes.");
            }

            if (interaction.FollowUp == "no" && !string.IsNullOrEmpty(interaction.Remarks))
            {
                return BadRequest("Remarks should not be provided when follow-up is no.");
            }

            _context.Interactions.Add(interaction);
            await _context.SaveChangesAsync();

            if (interaction.PurposeOfInteraction == "Application for modification")
            {
                await SendModificationEmail(interaction);
            }

            return CreatedAtAction(nameof(GetInteractions), new { id = interaction.Id }, interaction);
        }

        [HttpPost("SubmitExternalForm/{id}")]
        public async Task<IActionResult> SubmitExternalForm(int id, [FromForm] IFormCollection formData)
        {
            var interaction = await _context.Interactions.FindAsync(id);
            if (interaction == null)
            {
                return NotFound();
            }

            // Extract form values
            DateTime? startDuration = null;
            DateTime? endDuration = null;
            if (DateTime.TryParse(formData["StartDuration"], out var start))
            {
                startDuration = start;
            }
            if (DateTime.TryParse(formData["EndDuration"], out var end))
            {
                endDuration = end;
            }

            if (startDuration.HasValue && endDuration.HasValue)
            {
                var durationDiff = (endDuration.Value - startDuration.Value).Days;
                if (durationDiff > 30)
                {
                    return BadRequest("The duration between start and end date should not exceed 1 month.");
                }
            }

            // Save file attachments
            var files = formData.Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Define a directory to save the uploaded files
                        //var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                        if (!Directory.Exists(uploadsDirectory))
                        {
                            Directory.CreateDirectory(uploadsDirectory);
                        }

                        // Create a unique file name to avoid conflicts
                        var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Save the file path or file metadata in the database
                        switch (file.Name)
                        {
                            case "contractAgreement":
                                interaction.ContractAgreement = filePath;
                                break;
                            case "scopeOfWork":
                                interaction.ScopeOfWork = filePath;
                                break;
                            case "tradeLicence":
                                interaction.TradeLicence = filePath;
                                break;
                            case "emirateId":
                                interaction.EmirateId = filePath;
                                break;
                            case "thirdPartyLiabilityCert":
                                interaction.ThirdPartyLiabilityCert = filePath;
                                break;
                            case "currentProposedLayout":
                                interaction.CurrentProposedLayout = filePath;
                                break;
                        }
                    }
                }
            }

            // Update interaction details
            interaction.StartDuration = startDuration;
            interaction.EndDuration = endDuration;
            await _context.SaveChangesAsync();

            return Ok("Form submitted successfully");
        }


        private async System.Threading.Tasks.Task SendModificationEmail(Interaction interaction)
        {
            var smtpClient = new SmtpClient(_configuration["Mail:Host"])
            {
                Port = int.Parse(_configuration["Mail:Port"]),
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Mail:From"]),
                Subject = "Application for Modification",
                Body = $"Dear Customer,\n\nFill out the detail on the link below http://example.com/api/forms/SubmitExternalForm/{interaction.Id}\n\nRegards,\nAlHamra Team",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(interaction.EmailAddress);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

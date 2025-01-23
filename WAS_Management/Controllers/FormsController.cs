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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;


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

            if (interaction.PurposeOfInteraction == "Modification Request")
            {
                await SendModificationEmail(interaction, "", "InternalForm");
            }

            return CreatedAtAction(nameof(GetInteractions), new { id = interaction.Id }, interaction);
        }

        [HttpPost("ExFormSubmitted/{id}")]
        [AllowAnonymous]
        public async Task<Interaction> ExFormSubmitted(int id)
        {

            var interaction = await _context.Interactions.FindAsync(id);

            if (interaction != null)
            {
                if (interaction.PropertyType != null)
                {
                    return interaction;
                }
                else
                {
                    return interaction;
                }
            }
            else
            {
                return null;
            }

        }

        [HttpPost("SubmitExternalForm/{id}")]
        [AllowAnonymous]
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

            // Validate duration difference
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
                        var uploadsDirectory = _configuration.GetValue<string>("upload:path");
                        if (!Directory.Exists(uploadsDirectory))
                        {
                            Directory.CreateDirectory(uploadsDirectory);
                        }

                        var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Map each file to the correct column in the database
                        switch (file.Name)
                        {
                            case "uploadContractAgreement":
                                interaction.ContractAgreement = filePath;
                                break;
                            case "detailedScopeofWork":
                                interaction.ScopeOfWork = filePath;
                                break;
                            case "contractorTradeLicense":
                                interaction.TradeLicence = filePath;
                                break;
                            case "emiratesId":
                                interaction.EmirateId = filePath;
                                break;
                            case "thirdPartyLiabilityInsuranceCertificate":
                                interaction.ThirdPartyLiabilityCert = filePath;
                                break;
                            case "Current&ProposedLayoutDrawing":
                                interaction.CurrentProposedLayout = filePath;
                                break;
                        }
                    }
                }
            }

         
            var intwork = "";
            if (!StringValues.IsNullOrEmpty(formData["InternalWork"]))
            {
                intwork = String.Join(", ", formData["InternalWork"]);
            }

            // Update interaction details
            interaction.StartDuration = startDuration;
            interaction.EndDuration = endDuration;
            interaction.PropertyType = formData["PropertyType"];
            interaction.OwnerName = formData["OwnerName"];
            interaction.ContactNo = formData["ContactNo"];
            interaction.Email = formData["Email"];
            interaction.WorkType = formData["WorkType"];
            interaction.InternalWork = intwork;
            interaction.ContractorRepName = formData["ContractorRepName"];
            interaction.ContractorName = formData["ContractorName"];
            interaction.ContractorCompName = formData["ContractorCompName"];
            interaction.ContractorContact = formData["ContractorContact"];
            interaction.ContractorEmail = formData["ContractorEmail"];
            interaction.ContractorComp = formData["ContractorComp"];
            interaction.CustomerName = formData["CustomerName"];


            await _context.SaveChangesAsync();

            // --------------------------------------------------------------------
            // Gather all form data into a dictionary (key/value), then convert to JSON
            // --------------------------------------------------------------------
            var formDataDictionary = new Dictionary<string, string>();
            foreach (var key in formData.Keys)
            {
                formDataDictionary[key] = formData[key];
            }
            string jsonInfo = JsonConvert.SerializeObject(formDataDictionary);

            // --------------------------------------------------------------------
            // Call your CreateInteractionWorkflow after saving everything
            // (You’ll need to figure out how to get or define 'initiator_id' and 'workflowname')
            // --------------------------------------------------------------------
            int initiator_id = 1;
            string workflowname = /* your logic to define the workflow name */ "INTERACTION RECORDING FORM";

            // Return or store the result from the workflow as needed
            var workflowResult = await new WorkflowController(_context, _configuration).CreateWorkFlow(initiator_id, workflowname, jsonInfo, interaction.Id);

            // You could return the workflow result directly or combine it with your interaction

            string customertoken = GenerateFormattedId(interaction.Id);
            await SendModificationEmail(interaction, customertoken, "ExternalForm");


            return Ok(new
            {
                Interaction = interaction,
                WorkflowResult = workflowResult
            });
        }

        private string GenerateFormattedId(int id)
        {
            string prefix = "PMMR";
            return $"{prefix}{id:D5}";
        }

        private async System.Threading.Tasks.Task SendModificationEmail(Interaction interaction, string customertoken, string emailtype)
        {
            var smtpClient = new SmtpClient(_configuration["Mail:Host"])
            {
                Port = int.Parse(_configuration["Mail:Port"]),
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                EnableSsl = true,
            };

            if (customertoken == "")
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Mail:From"]),
                    Subject = "Application for Modification",
                    Body = $"Dear Customer,\n\nFill out the detail on the link below {_configuration["AppBaseURL:URL"]}/Interaction/form/ExternalSubmissionForm/{interaction.Id}\n\nRegards,\nAlHamra Team",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(interaction.EmailAddress);

                await smtpClient.SendMailAsync(mailMessage);
            }
            else
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Mail:From"]),
                    Subject = "Application in progress",
                    Body = $"Dear Customer,\n\nYour request ID {customertoken} is under process, and you will be contacted\r\nsoon\n\nRegards,\nAlHamra Team",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(interaction.Email);

                await smtpClient.SendMailAsync(mailMessage);
            }


        }
    }
}

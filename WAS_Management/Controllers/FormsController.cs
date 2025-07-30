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
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Interaction = WAS_Management.Models.Interaction;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;


namespace WAS_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormsController : ControllerBase
    {
        private readonly WAS_ManagementContext _context;
        private readonly IConfiguration _configuration;

        private readonly ILogger<WorkflowController> _logger;
        public FormsController(WAS_ManagementContext context, IConfiguration configuration, ILogger<WorkflowController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("interactions")]
        public async Task<ActionResult<IEnumerable<Interaction>>> GetInteractions()
        {
            return await _context.Interactions.ToListAsync();
        }

        [HttpPost("create")]
        [AllowAnonymous]
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



            if (interaction.PurposeOfInteraction.Contains("Enquiry for Modification"))
            {
                await SendModificationEmail(interaction, "", "InternalForm");
            }

            if (interaction.TypeSel == "Third-Party Contractor" && interaction.PurposeOfInteraction == "Contractor Registration")
            {
                await SendContractorRegistrationEmail(interaction.EmailAddress, interaction.paymentOption);
            }


            return CreatedAtAction(nameof(GetInteractions), new { id = interaction.Id }, interaction);
        }



        [HttpPost("CreateMiniInteraction")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateMiniInteraction(InteractionVM interaction)
        {

            Interaction inct = new Interaction();
            var unitnos = await _context.Units.Where(x => x.Unitid.Contains(interaction.unitNumber)).FirstOrDefaultAsync();
            if (unitnos != null)
            {
                inct.UnitNumber = unitnos.Slno + " " + unitnos.Unitid;
            }
            else
            {
                inct.UnitNumber = interaction.unitNumber;
            }


            inct.Date = DateTime.Now;
            inct.CustomerType = "External";
            inct.TypeSel = interaction.customertype;
            inct.ProjectName = inct.UnitNumber;
            inct.Email = interaction.emailAddress;
            inct.ContactNo = interaction.contactNumber;
            inct.TypeOfInteraction = "AI Alhamra Agent";
            inct.PurposeOfInteraction = "Modification Request";
            inct.FollowUp = "false";

            _context.Interactions.Add(inct);
            await _context.SaveChangesAsync();

            InteractionResponseVM respvm = new InteractionResponseVM();
            respvm.TicketNo = GenerateFormattedId(inct.Id);
            respvm.URL = _configuration["AppBaseURL:URL"] + "/Interaction/form/ExternalSubmissionForm/" + inct.Id;
            var res = JsonConvert.SerializeObject(respvm);
            return Ok(new
            {
                Response = respvm
            });
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


        [HttpPost("ExCCFormSubmitted/{id}")]
        [AllowAnonymous]
        public async Task<Contractor> ExCCFormSubmitted(int id)
        {

            var interaction = await _context.Contractors.FindAsync(id);

            if (interaction != null)
            {
                if (interaction.CompanyName != null)
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

            DateTime? startDuration = null;
            DateTime? endDuration = null;

            // Extract the datetime part (removing the extra time zone name in parentheses)
            string ExtractDateTimeString(string input)
            {
                return Regex.Match(input, @"^[A-Za-z]{3} [A-Za-z]{3} \d{2} \d{4}").Value;
            }

            // Parse StartDate
            if (formData.TryGetValue("StartDate", out var startStr))
            {
                string cleanStartStr = ExtractDateTimeString(startStr);

                if (DateTimeOffset.TryParseExact(cleanStartStr, "ddd MMM dd yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
                {
                    startDuration = start.Date;
                }
            }

            // Parse EndDate
            if (formData.TryGetValue("EndDate", out var endStr))
            {
                string cleanEndStr = ExtractDateTimeString(endStr);

                if (DateTimeOffset.TryParseExact(cleanEndStr, "ddd MMM dd yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                {
                    endDuration = end.Date;
                }
            }

            // Validate duration difference
            if (startDuration.HasValue && endDuration.HasValue)
            {
                var durationDiff = (endDuration.Value - startDuration.Value).Days;
                if (durationDiff > 92)
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
                            case "CurrentLayoutDrawing":
                                interaction.CurrentLayout = filePath;
                                break;
                            case "ProposedLayoutDrawing":
                                interaction.ProposedLayout = filePath;
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

            if (intwork == "Others")
            {
                intwork = "others - " + formData["OtherWork"];
            }
            interaction.InternalWork = intwork;
            interaction.ContractorRepName = formData["ContractorRepName"];
            interaction.ContractorName = formData["ContractorName"];
            //interaction.ContractorCompName = formData["contractingCompName"];
            interaction.ContractorContact = formData["ContractorContact"];
            interaction.ContractorEmail = formData["ContractorEmail"];

            if (formData["contractingCompName"] == "Other")
            {
                interaction.ContractorCompName = formData["companyName"];
            }
            else
            {
                interaction.ContractorCompName = formData["contractingCompName"];
            }
            interaction.CustomerName = formData["CustomerName"];
            interaction.TradeLicenceNo = formData["trade_licence_no"];


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
            var workflowResult = await new WorkflowController(_context, _configuration,_logger).CreateWorkFlow(initiator_id, workflowname, jsonInfo, interaction.Id, "");

            // You could return the workflow result directly or combine it with your interaction

            string customertoken = GenerateFormattedId(interaction.Id);
            await SendModificationEmail(interaction, customertoken, "ExternalForm");


            return Ok(new
            {
                Interaction = interaction,
                WorkflowResult = workflowResult
            });
        }



        [HttpGet("GenerateFormattedId/{id}")]
        [AllowAnonymous]
        private string GenerateFormattedId(int id)
        {
            string prefix = "PMMR";
            return $"{prefix}{id:D5}";
        }

        [HttpGet("GenerateCCFormattedId/{id}")]
        [AllowAnonymous]
        private string GenerateCCFormattedId(int id)
        {
            string prefix = "PMCR";
            return $"{prefix}{id:D5}";
        }

        [HttpGet("GenerateRNFormattedId/{id}")]
        [AllowAnonymous]
        private string GenerateRNFormattedId(int id)
        {
            string prefix = "PRN";
            return $"{prefix}{id:D5}";
        }


        [HttpPost("Createresalenoc")]
        [AllowAnonymous]
        public async Task<bool> Createresalenoc([FromForm] IFormCollection formData)
        {
            try
            {
                Resalenoc resalenc = new Resalenoc();
                resalenc.Mastercomm = formData["mastercomm"];
                resalenc.Projectname = formData["projectName"];
                resalenc.Email = formData["email"];
                resalenc.Unitno = formData["unitNumber"];
                resalenc.Customername = formData["customerName"];
                resalenc.Contactno = formData["contactNumber"];
                resalenc.Intiatorname = formData["initiatorName"];


                _context.Resalenocs.Add(resalenc);
                await _context.SaveChangesAsync();

                // --------------------------------------------------------------------
                // Gather all form data into a dictionary (key/value), then convert to JSON
                // --------------------------------------------------------------------

                string jsonInfo = JsonConvert.SerializeObject(resalenc);

                // --------------------------------------------------------------------
                // Call your CreateInteractionWorkflow after saving everything
                // (You’ll need to figure out how to get or define 'initiator_id' and 'workflowname')
                // --------------------------------------------------------------------
                int initiator_id = 1;
                string workflowname = /* your logic to define the workflow name */ "Resale NOC";

                // Return or store the result from the workflow as needed
                var workflowResult = await new WorkflowController(_context, _configuration, _logger).CreateWorkFlow(initiator_id, workflowname, jsonInfo, resalenc.Id, "");

                // You could return the workflow result directly or combine it with your interaction

                //string customertoken = GenerateFormattedId(interaction.Id);
                //await SendModificationEmail(interaction, customertoken, "ExternalForm");

                return workflowResult;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPost("ContractorRegistration/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ContractorRegistration(int id, [FromForm] IFormCollection formData)
        {
            var interaction = await _context.Contractors.FindAsync(id);
            if (interaction == null)
            {
                return NotFound();
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
                            case "companyTradeLicense":
                                interaction.Companytradelicence = filePath;
                                break;
                            case "passportVisa":
                                interaction.Emirateid = filePath;
                                break;
                            case "stampedAuthorization":
                                interaction.Authorization = filePath;
                                break;
                            case "vehicleRegistration":
                                interaction.VehicleReg = filePath;
                                break;
                            case "brandedVehicle":
                                interaction.VehiclePic = filePath;
                                break;
                            case "managerPassport":
                                interaction.Passportid = filePath;
                                break;
                            case "previousPermit":
                                interaction.Previouspermit = filePath;
                                break;
                        }
                    }
                }
            }



            // Update interaction details
            interaction.CompanyName = formData["CompanyName"];
            interaction.Address = formData["Address"];
            interaction.Landlineno = formData["landlineNo"];
            interaction.Mobileno = formData["mobile"];
            interaction.Email = formData["Email"];
            interaction.Plateno = formData["plateNumber"];
            interaction.Platetype = formData["Platetype"];
            interaction.Placeofissuance = formData["Placeofissuance"];
            interaction.Color = formData["Color"];
            interaction.Typeofvehicle = formData["Typeofvehicle"];
            interaction.Paymentoption = formData["paymentOption"].ToString();
            if (interaction.Paymentoption == "monthly")
            {
                interaction.RenewalDate = DateTime.Now.AddMonths(1);
            }
            else if (interaction.Paymentoption == "yearly")
            {
                interaction.RenewalDate = DateTime.Now.AddYears(1);
            }
            else
            {
                interaction.Paymentoption = "yearly";
                interaction.RenewalDate = DateTime.Now.AddYears(1);
            }


            interaction.Isactive = false;

            await _context.SaveChangesAsync();

            string formtype = formData["type"];
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
            string workflowname = formtype == "new" ? "CONTRACTOR REGISTRATION" : "CONTRACTOR RENEWAL";


            // Return or store the result from the workflow as needed
            var workflowResult = await new WorkflowController(_context, _configuration,_logger).CreateWorkFlow(initiator_id, workflowname, jsonInfo, interaction.Id, formtype);


            // You could return the workflow result directly or combine it with your interaction

            string customertoken = GenerateCCFormattedId(interaction.Id);
            await SendContractorServiceRequestEmail(interaction, customertoken);


            return Ok(new
            {
                Interaction = interaction,
                WorkflowResult = workflowResult
            });



        }

        [HttpPost("CheckAndSendRenewalEmails")]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task CheckAndSendRenewalEmails()
        {
            var contractorsDueForRenewal = await _context.Contractors
                .Where(c => c.RenewalDate.HasValue && c.RenewalDate.Value.Date == DateTime.Today)
                .ToListAsync();



            foreach (var contractor in contractorsDueForRenewal)
            {
                await SendRenewalEmail(contractor);
            }
        }

        private async System.Threading.Tasks.Task SendRenewalEmail(Contractor contractor)
        {
            var smtpClient = new SmtpClient(_configuration["Mail:Host"])
            {
                Port = int.Parse(_configuration["Mail:Port"]),
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
            };

            string emailSubject = $"Contractor Renewal";

            string emailBody = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            color: #333333;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #ffffff;
            padding: 20px;
            border-radius: 8px;
            border: 1px solid #e0e0e0;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 1px solid #e0e0e0;
        }}
        .header img {{
            max-width: 180px;
        }}
        .content {{
            padding: 20px;
            font-size: 16px;
            color: #333333;
            line-height: 1.5;
        }}
        .button {{
            display: inline-block;
            background-color: #007bff;
            color: #ffffff !important;
            padding: 10px 20px;
            text-align: center;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            margin: 10px 0;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 14px;
            color: #c70e0e;
            border-top: 1px solid #e0e0e0;
            margin-top: 20px;
        }}
        a {{
            color: #007bff;
            text-decoration: none;
            font-weight: bold;
        }}
        a:hover {{
            text-decoration: underline;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Al Hamra Logo'>
        </div>
        <div class='content'>
            <p>Dear {contractor.CompanyName},</p>
            <p>Please complete your renewal by clicking the button below:</p>
            <a style='background-color:#a6272e;color:white;' class='button' href='{_configuration["AppBaseURL:URL"]}/Interaction/form/ContractorRegistration/{contractor.Id}?type=renewal'>
                Complete Your Renewal
            </a>
            <p>If you have any questions, please contact us at 
                <a href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.
            </p>
        </div>
        <div class='footer'>
            <p style='color:#a6272e;'>Best regards,</p>
            <p style='color:#a6272e;'><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
        </div>
    </div>
</body>
</html>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Mail:From"]),
                Subject = emailSubject,
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(contractor.Email);

            await smtpClient.SendMailAsync(mailMessage);
        }



        [HttpGet("SendContractorRegistrationEmail/{email}")]
        [AllowAnonymous]
        public async Task<bool> SendContractorRegistrationEmail(string email,string paymentoption)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                };

                Contractor cr = new Contractor();
                cr.Paymentoption = paymentoption;
                cr.Email = email;
                _context.Contractors.Add(cr);
                await _context.SaveChangesAsync();

                string emailBody;
                string emailSubject;


                emailSubject = $"Contractor Registration";

                emailBody = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            color: #333333;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #ffffff;
            padding: 20px;
            border-radius: 8px;
            border: 1px solid #e0e0e0;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 1px solid #e0e0e0;
        }}
        .header img {{
            max-width: 180px;
        }}
        .content {{
            padding: 20px;
            font-size: 16px;
            color: #333333;
            line-height: 1.5;
        }}
        .button {{
            display: inline-block;
            background-color: #007bff;
            color: #ffffff !important;
            padding: 10px 20px;
            text-align: center;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            margin: 10px 0;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 14px;
            color: #c70e0e;
            border-top: 1px solid #e0e0e0;
            margin-top: 20px;
        }}
        a {{
            color: #007bff;
            text-decoration: none;
            font-weight: bold;
        }}
        a:hover {{
            text-decoration: underline;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Al Hamra Logo'>
        </div>
        <div class='content'>
            <p>Dear Sir,</p>
            <p>Please complete your registration by clicking the button below:</p>
            <a class='button' style='background-color:#a6272e;color:white;' href='{_configuration["AppBaseURL:URL"]}/Interaction/form/ContractorRegistration/{cr.Id}?type=new'>
                Complete Your Request
            </a>
            <p>If you have any questions, please contact us at 
                <a href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.
            </p>
        </div>
        <div class='footer'>
            <p style='color:#a6272e;'>Best regards,</p>
            <p style='color:#a6272e;'><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
        </div>
    </div>
</body>
</html>";


                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Mail:From"]),
                    Subject = emailSubject,
                    Body = emailBody,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPost("SendServiceRequestEmailAsync/{email}")]
        public async System.Threading.Tasks.Task SendServiceRequestEmailAsync(
    [FromForm] string email,
    [FromForm] string customername,
    [FromForm] string subject,
    [FromForm] string issue,
    [FromForm] IFormFile attachmentFile)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                };
                string TicketNo = Guid.NewGuid()
                       .ToString("N")             // 32 hex chars, no hyphens
                       .Substring(0, 10);         // take first 10
                string emailBody = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #ffffff; color: #000000; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: auto; background: #ffffff; padding: 20px; border-radius: 8px; border: 1px solid #000000; }}
        .header {{ text-align: center; padding-bottom: 10px; border-bottom: 1px solid #000000; }}
        .header img {{ max-width: 180px; }}
        .content {{ padding: 20px; font-size: 16px; color: #000000; line-height: 1.5; }}
        .footer {{ text-align: center; padding: 10px; font-size: 14px; color: #000000; border-top: 1px solid #000000; margin-top: 20px; }}
        a {{ color: #000000; text-decoration: none; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Logo'>
        </div>
        <div class='content'>
            <p>Ticket No: {TicketNo}</p>
            <p>Issue Reported By {customername},</p>
            <p>{issue}</p>
        </div>
    </div>
</body>
</html>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Mail:From"]),
                    Subject = subject,
                    Body = emailBody,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add("muhammadhassannaeem@gmail.com");
                mailMessage.To.Add("connect@flowpilot.ae");

                if (attachmentFile != null && attachmentFile.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await attachmentFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var fileName = string.IsNullOrWhiteSpace(attachmentFile.FileName) ? "attachment" : attachmentFile.FileName;
                    var contentType = string.IsNullOrWhiteSpace(attachmentFile.ContentType) ? "application/octet-stream" : attachmentFile.ContentType;

                    var attachment = new Attachment(memoryStream, fileName, contentType);
                    mailMessage.Attachments.Add(attachment);
                }



                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email Error: {ex.Message}");
                throw; // Optionally rethrow or handle gracefully
            }
        }


        [HttpGet("SendContractorServiceRequestEmail/{email}")]
        public async System.Threading.Tasks.Task SendContractorServiceRequestEmail(Contractor interaction, string customertoken)
        {
            var smtpClient = new SmtpClient(_configuration["Mail:Host"])
            {
                Port = int.Parse(_configuration["Mail:Port"]),
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
            };

            string emailSubject = $"Update on Your Service Request - ID: {customertoken}";

            string emailBody = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; background-color: #ffffff; color: #000000; margin: 0; padding: 20px; }}
            .container {{ max-width: 600px; margin: auto; background: #ffffff; padding: 20px; border-radius: 8px; border: 1px solid #000000; }}
            .header {{ text-align: center; padding-bottom: 10px; border-bottom: 1px solid #000000; }}
            .header img {{ max-width: 180px; }}
            .content {{ padding: 20px; font-size: 16px; color: #000000; line-height: 1.5; }}
            .footer {{ text-align: center; padding: 10px; font-size: 14px; color: #c70e0e; border-top: 1px solid #000000; margin-top: 20px; }}
            a {{ color: #000000; text-decoration: none; font-weight: bold; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Al Hamra Logo'>
            </div>
            <div class='content'>
                <p>Dear {interaction.CompanyName},</p>
                <p>Thank you for reaching out to the Property Management Team at Al Hamra Real Estate Development.</p>
                <p>We are pleased to confirm that your request (ID: <strong>{customertoken}</strong>) is under process, and our team will provide an update shortly.</p>
                <p>Kindly quote the reference number in future communications regarding this request.</p>
                <p>We appreciate your patience and look forward to assisting you.</p>
                <p>If you have any questions, please contact us at <a href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
            </div>
            <div class='footer'>
                <p style='color:#a6272e;'>Best regards,</p>
                <p style='color:#a6272e;'><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
            </div>
        </div>
    </body>
    </html>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Mail:From"]),
                Subject = emailSubject,
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(interaction.Email);

            await smtpClient.SendMailAsync(mailMessage);
        }




        [HttpGet("SendModificationEmail/{id}")]
        [AllowAnonymous]
        private async System.Threading.Tasks.Task SendModificationEmail(Interaction interaction, string customertoken, string emailtype)
        {
            var smtpClient = new SmtpClient(_configuration["Mail:Host"])
            {
                Port = int.Parse(_configuration["Mail:Port"]),
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
            };

            string emailBody;
            string emailSubject;

            if (string.IsNullOrEmpty(customertoken))
            {
                emailSubject = "Complete Your Request for Modification at Al Hamra Village";

                emailBody = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #ffffff;
                    color: #000000;
                    margin: 0;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: auto;
                    background: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    border: 1px solid #000000;
                }}
                .header {{
                    text-align: center;
                    padding-bottom: 10px;
                    border-bottom: 1px solid #000000;
                }}
                .header img {{
                    max-width: 180px;
                }}
                .content {{
                    padding: 20px;
                    font-size: 16px;
                    color: #000000;
                    line-height: 1.5;
                }}
                .button {{
                    display: block;
                    width: 220px;
                    text-align: center;
                    background-color: #000000;
                    color: #ffffff;
                    padding: 12px;
                    border-radius: 5px;
                    text-decoration: none;
                    font-size: 16px;
                    margin: 20px auto;
                    font-weight: bold;
                }}
                .footer {{
                    text-align: center;
                    padding: 10px;
                    font-size: 14px;
                    color: #c70e0e;
                    border-top: 1px solid #000000;
                    margin-top: 20px;
                }}
                a {{
                    color: #000000;
                    text-decoration: none;
                    font-weight: bold;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Al Hamra Logo'>
                </div>
                <div class='content'>
                    <p>Dear Valued Customer,</p>
                    <p>You are receiving this notification based on your recent interaction with Al Hamra Real Estate Development.</p>
                    <p>To complete your request for modification and accept the terms and conditions, please click the button below:</p>
                    <a style='background-color:#a6272e;color:white;' class='button'  href='{_configuration["AppBaseURL:URL"]}/Interaction/form/ExternalSubmissionForm/{interaction.Id}'>Complete Your Request</a>
                    <p>If you have any questions or need further assistance, please feel free to contact us at 
                    <a   href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
                </div>
                <div class='footer'>
                    <p style='color:#a6272e;'>Best regards,</p>
                    <p style='color:#a6272e;'><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
                </div>
            </div>
        </body>
        </html>";
            }
            else
            {
                emailSubject = $"Update on Your Service Request - ID: {customertoken}";

                emailBody = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #ffffff;
                    color: #000000;
                    margin: 0;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: auto;
                    background: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    border: 1px solid #000000;
                }}
                .header {{
                    text-align: center;
                    padding-bottom: 10px;
                    border-bottom: 1px solid #000000;
                }}
                .header img {{
                    max-width: 180px;
                }}
                .content {{
                    padding: 20px;
                    font-size: 16px;
                    color: #000000;
                    line-height: 1.5;
                }}
                .footer {{
                    text-align: center;
                    padding: 10px;
                    font-size: 14px;
                    color: #000000;
                    border-top: 1px solid #000000;
                    margin-top: 20px;
                }}
                a {{
                    color: #000000;
                    text-decoration: none;
                    font-weight: bold;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Al Hamra Logo'>
                </div>
                <div class='content'>
                    <p>Dear {interaction.OwnerName},</p>
                    <p>Thank you for reaching out to the Property Management Team at Al Hamra Real Estate Development.</p>
                    <p>We are pleased to confirm that your request (ID: <strong>{customertoken}</strong>) is under process, and our team will provide an update shortly.</p>
                    <p>Kindly quote the reference number in future communications regarding this request.</p>
                    <p>We appreciate your patience and look forward to assisting you.</p>
                    <p>If you have any questions, please contact us at <a style='color:black' href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
                </div>
                <div class='footer'>
                    <p style='color:#a6272e;'>Best regards,</p>
                    <p style='color:#a6272e;'><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
                </div>
            </div>
        </body>
        </html>";
            }

            // Create the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Mail:From"]),
                Subject = emailSubject,
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(interaction.EmailAddress);

            await smtpClient.SendMailAsync(mailMessage);
        }




    }
}

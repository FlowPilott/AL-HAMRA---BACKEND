using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using WAS_Management.Data;
using WAS_Management.Models;
using WAS_Management.ViewModels;
using Task = WAS_Management.Models.Task;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Linq;
using PdfSharp.Pdf;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using DinkToPdf;
using DinkToPdf.Contracts;
using Interaction = WAS_Management.Models.Interaction;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Json.Nodes;



namespace WAS_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly WAS_ManagementContext _context;
        private readonly IConfiguration _configuration;

        public WorkflowController(WAS_ManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("CreateWorkFlow")]
        public async Task<bool> CreateWorkFlow(int initiator_id, string workflowname, string jsonInfo, int interactionid, string type)
        {
            if (workflowname == "INTERACTION RECORDING FORM")
            {
                return await CreateInteractionWorkflow(initiator_id, workflowname, jsonInfo, interactionid);
            }
            if (workflowname == "CONTRACTOR REGISTRATION")
            {
                return await CreateContractorWorkflow(initiator_id, workflowname, jsonInfo, interactionid, "CONTRACTOR REGISTRATION");
            }
            if (workflowname == "CONTRACTOR RENEWAL" && type == "Renewal")
            {
                return await CreateContractorWorkflow(initiator_id, workflowname, jsonInfo, interactionid, "CONTRACTOR RENEWAL");
            }
            if (workflowname == "Resale NOC")
            {
                return await CreateResaleNOCWorkflow(initiator_id, workflowname, jsonInfo, interactionid);
            }



            return true;
        }



        private string GenerateNmeFormattedId(int id, string template)
        {
            if (template == "Modification Request")
            {
                return GenerateFormattedId(id);
            }
            else if (template == "CONTRACTOR REGISTRATION")
            {
                return GenerateCCFormattedId(id);
            }
            else if (template == "Resale NOC")
            {
                return GenerateRNFormattedId(id);
            }
            return "";
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
            string prefix = "PMRN";
            return $"{prefix}{id:D5}";
        }




        [HttpPost("CreateInteractionWorkflow")]
        [AllowAnonymous]
        private async Task<bool> CreateInteractionWorkflow(int initiator_id, string workflowname, string jsonInfo, int interactionid)
        {
            Workflow workflow = new Workflow();
            try
            {

                var userid1 = await _context.Users.Where(x => x.Username == "Bejoy").Select(x => x.Id).FirstOrDefaultAsync();
                var userid2 = await _context.Users.Where(x => x.Username == "Jinu").Select(x => x.Id).FirstOrDefaultAsync();
                workflow.InitiatorId = initiator_id;
                var workflowtypeid = await _context.WorkflowTypes.Where(x => x.Name == "INTERACTION RECORDING FORM").Select(x => x.Id).FirstOrDefaultAsync();
                workflow.WorkflowTypeId = workflowtypeid;
                workflow.Status = "In Progress";
                workflow.Subject = "Interaction Recording Form";
                workflow.ProcessOwner = initiator_id;
                workflow.StartedOn = DateTime.Now;
                workflow.Progress = "Active";
                workflow.InteractionId = interactionid.ToString();
                // string jsonString = JsonSerializer.Serialize(workflow);
                workflow.Details = jsonInfo;
                await _context.AddAsync(workflow);
                await _context.SaveChangesAsync();
                #region workflow step 
                WorkflowStep step = new WorkflowStep();
                step.WorkflowId = workflow.Id;
                step.StepName = "Review Scope and Site Requirements";
                step.StepDescription = "Review Scope and Site Requirements";
                step.Type = "Any";

                step.Status = "In Progress";
                step.ReceivedOn = DateTime.Now;
                step.DueOn = DateTime.Now.AddDays(2);
                await _context.AddAsync(step);
                await _context.SaveChangesAsync();

                WorkflowStep step2 = new WorkflowStep();
                step2.WorkflowId = workflow.Id;
                step2.StepName = "Review Scope";
                step2.StepDescription = "Review Scope";
                step2.Type = "All";
                userid1 = await _context.Users.Where(x => x.Username == "Abubaker").Select(x => x.Id).FirstOrDefaultAsync();
                //userid2 = await _context.Users.Where(x => x.Email == "jinu.joy@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                var data2 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" }

};
                string jsonStringUsers2 = JsonSerializer.Serialize(data2, new JsonSerializerOptions { WriteIndented = true });
                step2.AssignedTo = jsonStringUsers2;
                step2.Status = "Not Started";
                // step2.ReceivedOn = DateTime.Now;
                // step2.DueOn = DateTime.Now.AddDays(4);
                await _context.AddAsync(step2);
                await _context.SaveChangesAsync();

                WorkflowStep step3 = new WorkflowStep();
                step3.WorkflowId = workflow.Id;
                step3.StepName = "Review Scope2";
                step3.StepDescription = "Review Scope2";
                step3.Type = "All";
                userid1 = await _context.Users.Where(x => x.Username == "Suhail").Select(x => x.Id).FirstOrDefaultAsync();
                // var userid2 = await _context.Users.Where(x => x.Email == "jinu.joy@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                var data3 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" }
};
                string jsonStringUsers3 = JsonSerializer.Serialize(data3, new JsonSerializerOptions { WriteIndented = true });
                step3.AssignedTo = jsonStringUsers3;
                step3.Status = "Not Started";
                //  step3.ReceivedOn = DateTime.Now;
                // step3.DueOn = DateTime.Now.AddDays(6);
                await _context.AddAsync(step3);
                await _context.SaveChangesAsync();

                WorkflowStep step4 = new WorkflowStep();
                step4.WorkflowId = workflow.Id;
                step4.StepName = "Review Scope and Fees Calculation";
                step4.StepDescription = "Review Scope and Fees Calculation";
                step4.Type = "All";
                userid1 = await _context.Users.Where(x => x.Username == "Dilin").Select(x => x.Id).FirstOrDefaultAsync();
                // var userid2 = await _context.Users.Where(x => x.Email == "jinu.joy@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                var data4 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" }
};
                string jsonStringUsers4 = JsonSerializer.Serialize(data4, new JsonSerializerOptions { WriteIndented = true });
                step4.AssignedTo = jsonStringUsers4;
                step4.Status = "Not Started";
                //   step4.ReceivedOn = DateTime.Now;
                //   step4.DueOn = DateTime.Now.AddDays(8);
                await _context.AddAsync(step4);
                await _context.SaveChangesAsync();

                WorkflowStep step5 = new WorkflowStep();
                step5.WorkflowId = workflow.Id;
                step5.StepName = "Upload the Invoice";
                step5.StepDescription = "Upload the Invoice";
                step5.Type = "All";
                userid1 = await _context.Users.Where(x => x.Username == "Cashier").Select(x => x.Id).FirstOrDefaultAsync();
                // var userid2 = await _context.Users.Where(x => x.Email == "jinu.joy@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                var data5 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" }
};
                string jsonStringUsers5 = JsonSerializer.Serialize(data5, new JsonSerializerOptions { WriteIndented = true });
                step5.AssignedTo = jsonStringUsers5;
                step5.Status = "Not Started";
                //  step5.ReceivedOn = DateTime.Now;
                //  step5.DueOn = DateTime.Now.AddDays(10);
                await _context.AddAsync(step5);
                await _context.SaveChangesAsync();

                WorkflowStep step6 = new WorkflowStep();
                step6.WorkflowId = workflow.Id;
                step6.StepName = "Confirm Payment Received";
                step6.StepDescription = "Confirm Payment Received";
                step6.Type = "Any";
                userid1 = await _context.Users.Where(x => x.Username == "Bejoy").Select(x => x.Id).FirstOrDefaultAsync();
                userid2 = await _context.Users.Where(x => x.Username == "Jinu").Select(x => x.Id).FirstOrDefaultAsync();
                var data6 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" },
     new { Id = userid2.ToString(), Status = "Not Approved", Rights = "Edit" },
};
                string jsonStringUsers6 = JsonSerializer.Serialize(data6, new JsonSerializerOptions { WriteIndented = true });
                step6.AssignedTo = jsonStringUsers6;
                step6.Status = "Not Started";
                //   step6.ReceivedOn = DateTime.Now;
                //   step6.DueOn = DateTime.Now.AddDays(12);
                await _context.AddAsync(step6);
                await _context.SaveChangesAsync();
                #endregion

                var taskid_1 = await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid1, "Modification Request", initiator_id);
                var taskid_2 = await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid2, "Modification Request", initiator_id);

                var data = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" , TaskId = taskid_1 },
    new { Id = userid2.ToString(), Status = "Not Approved", Rights = "Edit" , TaskId = taskid_2},
};
                string jsonStringUsers = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                step.AssignedTo = jsonStringUsers;


                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


        [HttpPost("CreateContractorWorkflow")]
        private async Task<bool> CreateContractorWorkflow(int initiator_id, string workflowname, string jsonInfo, int interactionid, string type)
        {
            Workflow workflow = new Workflow();
            try
            {

                var userid1 = await _context.Users.Where(x => x.Username == "Bejoy").Select(x => x.Id).FirstOrDefaultAsync();
                var userid2 = await _context.Users.Where(x => x.Username == "Jinu").Select(x => x.Id).FirstOrDefaultAsync();
                workflow.InitiatorId = initiator_id;
                var workflowtypeid = await _context.WorkflowTypes.Where(x => x.Name == type).Select(x => x.Id).FirstOrDefaultAsync();
                workflow.WorkflowTypeId = workflowtypeid;
                workflow.Status = "In Progress";
                workflow.Subject = type;
                workflow.ProcessOwner = initiator_id;
                workflow.StartedOn = DateTime.Now;
                workflow.Progress = "Active";
                workflow.InteractionId = interactionid.ToString();
                // string jsonString = JsonSerializer.Serialize(workflow);
                workflow.Details = jsonInfo;
                await _context.AddAsync(workflow);
                await _context.SaveChangesAsync();
                #region workflow step 
                WorkflowStep step = new WorkflowStep();
                step.WorkflowId = workflow.Id;
                step.StepName = "REVIEW DOCS";
                step.StepDescription = "REVIEW DOCS";
                step.Type = "Any";

                step.Status = "In Progress";
                step.ReceivedOn = DateTime.Now;
                step.DueOn = DateTime.Now.AddDays(2);
                await _context.AddAsync(step);
                await _context.SaveChangesAsync();

                WorkflowStep step2 = new WorkflowStep();
                step2.WorkflowId = workflow.Id;
                step2.StepName = "APPROVE";
                step2.StepDescription = "APPROVE";
                step2.Type = "All";
                userid1 = await _context.Users.Where(x => x.Username == "Abubaker").Select(x => x.Id).FirstOrDefaultAsync();
                userid2 = await _context.Users.Where(x => x.Username == "Suhail").Select(x => x.Id).FirstOrDefaultAsync();
                var data2 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" },
    new { Id = userid2.ToString(), Status = "Not Approved", Rights = "Edit" }

};
                string jsonStringUsers2 = JsonSerializer.Serialize(data2, new JsonSerializerOptions { WriteIndented = true });
                step2.AssignedTo = jsonStringUsers2;
                step2.Status = "Not Started";
                // step2.ReceivedOn = DateTime.Now;
                // step2.DueOn = DateTime.Now.AddDays(4);
                await _context.AddAsync(step2);
                await _context.SaveChangesAsync();

                WorkflowStep step3 = new WorkflowStep();
                step3.WorkflowId = workflow.Id;
                step3.StepName = "UPLOAD INVOICE";
                step3.StepDescription = "UPLOAD INVOICE";
                step3.Type = "All";
                userid1 = await _context.Users.Where(x => x.Username == "Cashier").Select(x => x.Id).FirstOrDefaultAsync();
                // var userid2 = await _context.Users.Where(x => x.Email == "jinu.joy@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                var data3 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" }
};
                string jsonStringUsers3 = JsonSerializer.Serialize(data3, new JsonSerializerOptions { WriteIndented = true });
                step3.AssignedTo = jsonStringUsers3;
                step3.Status = "Not Started";
                //  step3.ReceivedOn = DateTime.Now;
                // step3.DueOn = DateTime.Now.AddDays(6);
                await _context.AddAsync(step3);
                await _context.SaveChangesAsync();

                WorkflowStep step4 = new WorkflowStep();
                step4.WorkflowId = workflow.Id;
                step4.StepName = "FILE CLOSURE";
                step4.StepDescription = "FILE CLOSURE";
                step4.Type = "Any";
                userid1 = await _context.Users.Where(x => x.Username == "Bejoy").Select(x => x.Id).FirstOrDefaultAsync();
                userid2 = await _context.Users.Where(x => x.Username == "Jinu").Select(x => x.Id).FirstOrDefaultAsync();
                var data4 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" },
     new { Id = userid2.ToString(), Status = "Not Approved", Rights = "Edit" },
};
                string jsonStringUsers4 = JsonSerializer.Serialize(data4, new JsonSerializerOptions { WriteIndented = true });
                step4.AssignedTo = jsonStringUsers4;
                step4.Status = "Not Started";
                //   step4.ReceivedOn = DateTime.Now;
                //   step4.DueOn = DateTime.Now.AddDays(8);
                await _context.AddAsync(step4);
                await _context.SaveChangesAsync();
                #endregion

                var taskid_1 = await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid1, type, initiator_id);
                var taskid_2 = await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid2, type, initiator_id);

                var data = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" , TaskId = taskid_1 },
    new { Id = userid2.ToString(), Status = "Not Approved", Rights = "Edit" , TaskId = taskid_2},
};
                string jsonStringUsers = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                step.AssignedTo = jsonStringUsers;
                await _context.SaveChangesAsync();


                var contractor = _context.Contractors.Where(x => x.Id == interactionid).FirstOrDefault();
                contractor.Isactive = false;
                await _context.SaveChangesAsync();


                return true;

            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }



        [HttpPost("CreateResaleNOCWorkflow")]
        private async Task<bool> CreateResaleNOCWorkflow(int initiator_id, string workflowname, string jsonInfo, int interactionid)
        {
            Workflow workflow = new Workflow();
            try
            {

                var userid1_one = await _context.Users.Where(x => x.Username == "Bejoy").Select(x => x.Id).FirstOrDefaultAsync();
                var userid2_one = await _context.Users.Where(x => x.Username == "Jinu").Select(x => x.Id).FirstOrDefaultAsync();
                workflow.InitiatorId = initiator_id;
                var workflowtypeid = await _context.WorkflowTypes.Where(x => x.Name == "Resale NOC").Select(x => x.Id).FirstOrDefaultAsync();
                workflow.WorkflowTypeId = workflowtypeid;
                workflow.Status = "In Progress";
                workflow.Subject = "Resale NOC";
                workflow.ProcessOwner = initiator_id;
                workflow.StartedOn = DateTime.Now;
                workflow.Progress = "Active";
                workflow.InteractionId = interactionid.ToString();
                // string jsonString = JsonSerializer.Serialize(workflow);
                workflow.Details = jsonInfo;
                await _context.AddAsync(workflow);
                await _context.SaveChangesAsync();
                #region workflow step 
                WorkflowStep step = new WorkflowStep();
                step.WorkflowId = workflow.Id;
                step.StepName = "INSPECT PROPERTY";
                step.StepDescription = "INSPECT PROPERTY";
                step.Type = "Any";

                step.Status = "In Progress";
                step.ReceivedOn = DateTime.Now;
                step.DueOn = DateTime.Now.AddDays(2);
                await _context.AddAsync(step);
                await _context.SaveChangesAsync();

                WorkflowStep step2 = new WorkflowStep();
                step2.WorkflowId = workflow.Id;
                step2.StepName = "APPROVE";
                step2.StepDescription = "APPROVE";
                step2.Type = "Any";
                var userid1 = await _context.Users.Where(x => x.Username == "Abubaker").Select(x => x.Id).FirstOrDefaultAsync();
                var userid2 = await _context.Users.Where(x => x.Username == "Suhail").Select(x => x.Id).FirstOrDefaultAsync();
                var data2 = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" },
    new { Id = userid2.ToString(), Status = "Not Approved", Rights = "Edit" }

};
                string jsonStringUsers2 = JsonSerializer.Serialize(data2, new JsonSerializerOptions { WriteIndented = true });
                step2.AssignedTo = jsonStringUsers2;
                step2.Status = "Not Started";
                // step2.ReceivedOn = DateTime.Now;
                // step2.DueOn = DateTime.Now.AddDays(4);
                await _context.AddAsync(step2);
                await _context.SaveChangesAsync();

                #endregion

                var taskid_1 = await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid1_one, "Resale NOC", initiator_id);
                var taskid_2 = await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid2_one, "Resale NOC", initiator_id);

                var data = new[]
{
    new { Id = userid1_one.ToString(), Status = "Not Approved", Rights = "Edit" , TaskId = taskid_1 },
    new { Id = userid2_one.ToString(), Status = "Not Approved", Rights = "Edit" , TaskId = taskid_2},
};
                string jsonStringUsers = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                step.AssignedTo = jsonStringUsers;


                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }




        [HttpGet("downloadall/{workflowId}/{stepid}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadAllFiles(int workflowId, int stepid)
        {
            // Retrieve workflow steps for the given workflowId and stepid
            var stepEntities = await _context.WorkflowSteps
                .Where(x => x.Id == stepid && x.WorkflowId == workflowId)
                .ToListAsync();

            // Map the step entities to DTOs
            var workflowStepDtos = Mapper.MapToDtos<WorkflowStep, WorkFlowStepVM>(stepEntities);

            // Extract step IDs for filtering documents
            var stepIds = workflowStepDtos.Select(x => x.Id).ToList();

            // Retrieve documents associated with the workflow steps
            var documents = await _context.WorkflowDocuments
                .Where(x => stepIds.Contains(Convert.ToInt32(x.WorkflowId)))
                .ToListAsync();

            // Check if any documents were found
            if (documents == null || !documents.Any())
            {
                return NotFound("No documents found for the given workflow.");
            }

            // Create a memory stream for the ZIP archive (DO NOT use `using`)
            var memoryStream = new MemoryStream();

            try
            {
                // Create the ZIP archive
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var document in documents)
                    {
                        var filePath = document.DocumentPath;

                        // Skip files that don't exist
                        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                        {
                            continue;
                        }

                        // Add each existing file to the ZIP archive
                        var entryName = string.IsNullOrEmpty(document.DocumentName)
                            ? Path.GetFileName(filePath)
                            : document.DocumentName;

                        var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);

                        using (var entryStream = entry.Open())
                        using (var fileStream = System.IO.File.OpenRead(filePath))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }

                // Reset the memory stream position to the beginning before returning it
                memoryStream.Position = 0;

                // Define a filename for the ZIP file
                var zipFileName = $"Workflow_{workflowId}_Documents.zip";

                // Return the ZIP file as a downloadable file (DO NOT wrap memoryStream in `using`)
                return File(memoryStream, "application/zip", zipFileName);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"An error occurred while creating the ZIP archive: {ex.Message}");
            }
        }





        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadFile(int documentId)
        {
            // 1. Retrieve the WorkflowDocument from the database
            var workflowDocument = await _context.WorkflowDocuments
                .FirstOrDefaultAsync(doc => doc.Id == documentId);

            // 2. Check if the document exists
            if (workflowDocument == null)
            {
                return NotFound("Document not found.");
            }

            // 3. Verify the file path
            var filePath = workflowDocument.DocumentPath;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File does not exist on the server.");
            }

            // Generate the full URL including the base URL and uploads path
            var baseUrl = _configuration.GetValue<string>("AppBaseURL:URL");
            var fileUrl = $"{baseUrl}/uploads/{Path.GetFileName(filePath)}";

            // Return the file URL as a response
            return Ok(new { FileUrl = fileUrl });
        }


        [HttpGet("downloadinteractionfile/{interactionid}/{docname}")]
        [AllowAnonymous]
        public async Task<string> DownloadInteractionFile(int interactionid, string docname)
        {
            // 1. Retrieve the WorkflowDocument from the database
            var interactions = await _context.Interactions
                .Where(x => x.Id == interactionid).FirstOrDefaultAsync();

            string docppath = "";

            if (docname == "contractAgreement")
            {
                docppath = interactions.ContractAgreement;
            }
            else if (docname == "scopeOfWork")
            {
                docppath = interactions.ScopeOfWork;
            }
            else if (docname == "tradeLicence")
            {
                docppath = interactions.TradeLicence;
            }
            else if (docname == "emirateId")
            {
                docppath = interactions.EmirateId;
            }
            else if (docname == "thirdPartyLiabilityCert")
            {
                docppath = interactions.ThirdPartyLiabilityCert;
            }
            else if (docname == "currentLayout")
            {
                docppath = interactions.CurrentLayout;
            }
            else if (docname == "proposedLayout")
            {
                docppath = interactions.ProposedLayout;
            }

            // 2. Check if the document path exists
            if (string.IsNullOrEmpty(docppath))
            {
                return "Document not found.";
            }

            // 3. Verify the file existence
            if (!System.IO.File.Exists(docppath))
            {
                return "File does not exist on the server.";
            }

            // 4. Generate the full URL including the base URL and uploads path
            var baseUrl = _configuration.GetValue<string>("AppBaseURL:URL");
            var fileUrl = $"{baseUrl}/uploads/{Path.GetFileName(docppath)}";

            return fileUrl;

        }


        [HttpGet("DownloadContractorFile/{interactionid}/{docname}")]
        [AllowAnonymous]
        public async Task<string> DownloadContractorFile(int interactionid, string docname)
        {
            // 1. Retrieve the WorkflowDocument from the database
            var interactions = await _context.Contractors
                .Where(x => x.Id == interactionid).FirstOrDefaultAsync();

            string docppath = "";


            if (docname == "companytradelicence")
            {
                docppath = interactions.Companytradelicence;
            }
            else if (docname == "emirateid")
            {
                docppath = interactions.Emirateid;
            }
            else if (docname == "authorization")
            {
                docppath = interactions.Authorization;
            }
            else if (docname == "vehicleReg")
            {
                docppath = interactions.VehicleReg;
            }
            else if (docname == "vehiclePic")
            {
                docppath = interactions.VehiclePic;
            }
            else if (docname == "passportid")
            {
                docppath = interactions.Passportid;
            }
            else if (docname == "previouspermit")
            {
                docppath = interactions.Previouspermit;
            }

            // 2. Check if the document path exists
            if (string.IsNullOrEmpty(docppath))
            {
                return "Document not found.";
            }

            // 3. Verify the file existence
            if (!System.IO.File.Exists(docppath))
            {
                return "File does not exist on the server.";
            }

            // 4. Generate the full URL including the base URL and uploads path
            var baseUrl = _configuration.GetValue<string>("AppBaseURL:URL");
            var fileUrl = $"{baseUrl}/uploads/{Path.GetFileName(docppath)}";

            return fileUrl;

        }


        [HttpPost("CreateWorkFlowStepAction")]
        public async Task<JsonResult> CreateWorkFlowStepAction(int WorkflowStepId, [FromForm] IFormCollection formData)
        {
            try
            {
                var stepAction = await this.ConvertformDataToStepAction(formData);
                string GetStringValue(string key) => formData.ContainsKey(key) ? formData[key].ToString() : null;
                int? GetNullableIntValue(string key) => int.TryParse(GetStringValue(key), out int value) ? value : null;


                int? approverid = GetNullableIntValue("ApproverId");
                int? tskid = GetNullableIntValue("TaskId");
                string? typeofWork = GetStringValue("typeofWork");
                var approvalstatus = formData.ContainsKey("approvalStatus") ? formData["approvalStatus"].ToString() : null;
                stepAction.approvalstatus = approvalstatus;


                if (formData.ContainsKey("checkboxes"))
                {
                    var checkboxesJson = formData["checkboxes"].ToString();
                    // Deserialize JSON to Dictionary
                    var checkboxesDict = JsonSerializer.Deserialize<Dictionary<string, object>>(checkboxesJson);

                    if (checkboxesDict != null)
                    {
                        // Extract keys where values are 'true'
                        var selectedItems = checkboxesDict
     .Select(kv => new { Key = kv.Key, Value = kv.Value }) // Use proper casing
     .ToList();


                        List<string> chkboxes = new List<string>();

                        foreach (var item in selectedItems)
                        {
                            if (item.Key == "unauthorizedGarageDoor" && item.Value.ToString() == "True")
                            {
                                chkboxes.Add("Unauthorized Garage door installation");
                            }

                            if (item.Key == "unauthorizedAreaExtension" && item.Value.ToString() == "True")
                            {
                                chkboxes.Add("Unauthorized area extension");
                            }

                            if (item.Key == "unauthorizedStructureEquipment" && item.Value.ToString() == "True")
                            {
                                chkboxes.Add("Unauthorized installation of structure / equipment within the unit");
                            }


                            if (item.Key == "other")
                            {
                                chkboxes.Add(item.Value.ToString());
                            }
                        }


                        // Map selectedItems to StepAction Checkboxes property
                        stepAction.Checkboxes = chkboxes;


                        // Print the selected checkboxes

                    }
                }


                if (stepAction.ActionType == "RFI")
                {
                    int stepid = Convert.ToInt32(stepAction.PrevStepId);

                    var stepaction = await _context.StepActions.Where(x => x.Id == stepid).FirstOrDefaultAsync();
                    // Retrieve the previous workflow step
                    var prevstep = await _context.WorkflowSteps
                        .Where(x => x.Id == stepaction.StepId)
                        .FirstOrDefaultAsync();



                    var pathData = new List<dynamic>();


                    var files = formData.Files;
                    if (files != null && files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            if (file.Length > 0)
                            {
                                // 2. Get the upload path from appsettings.json
                                var uploadPath = _configuration.GetValue<string>("upload:path");


                                // 3. Combine with the current directory to get the full path
                                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);


                                // 4. Ensure the directory exists
                                if (!Directory.Exists(fullPath))
                                {
                                    Directory.CreateDirectory(fullPath);
                                }

                                // 5. Generate a unique file name to avoid overwriting
                                var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(fullPath, fileName);

                                // 6. Save the file to the specified path
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                // Use the original file name
                                var originalFileName = Path.GetFileName(file.FileName);


                                WorkflowDocument workflowDocument = new WorkflowDocument();
                                workflowDocument.StepId = stepAction.Id;
                                workflowDocument.WorkflowId = WorkflowStepId;
                                workflowDocument.DocumentPath = filePath;
                                workflowDocument.DocumentName = originalFileName;
                                workflowDocument.UploadedOn = DateTime.Now;
                                await _context.AddAsync(workflowDocument);
                                await _context.SaveChangesAsync();
                                pathData.Add(new
                                {
                                    Name = originalFileName,
                                    Path = filePath,
                                    Id = workflowDocument.Id
                                });
                            }
                        }
                    }




                    var task = await _context.UserTasks.Where(x => x.Id == tskid).FirstOrDefaultAsync();

                    task.Status = "Approved";
                    await _context.SaveChangesAsync();


                    // Safely parse the existing JSON details
                    var detailsList = JsonConvert.DeserializeObject<List<DetailModelVM>>(prevstep.Details)
                                      ?? new List<DetailModelVM>();

                    // Option A: Add/Update the 'Answer' field for all items in the JSON
                    foreach (var detail in detailsList)
                    {
                        if (detail.StepId != null)
                        {
                            int stpid = Convert.ToInt32(detail.StepId);
                            int prvstpid = Convert.ToInt32(stepAction.PrevStepId);
                            if (stpid == prvstpid)
                            {
                                detail.Answer = stepAction.Comments == null ? "" : stepAction.Comments;
                                detail.Files = JsonConvert.SerializeObject(pathData);
                            }
                        }
                    }







                    var combinedData = new
                    {
                        Details = detailsList,
                        Files = pathData, // Embed pathData (list of files)

                    };

                    // string jsonString = JsonSerializer.Serialize(combinedData, new JsonSerializerOptions { WriteIndented = true });
                    //prevstep.Actiondetails = jsonString;


                    // Serialize the updated list back to JSON and store it
                    //prevstep.Status = "Approved";
                    prevstep.Details = JsonConvert.SerializeObject(detailsList);

                    // Save changes
                    await _context.SaveChangesAsync();







                    var successData = new
                    {
                        Result = "Success",
                        ErrorCode = "200",
                        ErrorMessage = "",
                        Data = true,
                    };
                    return new JsonResult(successData);
                }


                if (stepAction != null)
                {
                    stepAction.StepId = WorkflowStepId;
                    stepAction.PerformedOn = DateTime.Now;
                    await _context.AddAsync(stepAction);
                    await _context.SaveChangesAsync();
                    var pathData = new List<dynamic>();
                    var files = formData.Files;
                    if (files != null && files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            if (file.Length > 0)
                            {


                                // 2. Get the upload path from appsettings.json
                                var uploadPath = _configuration.GetValue<string>("upload:path");


                                // 3. Combine with the current directory to get the full path
                                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);


                                // 4. Ensure the directory exists
                                if (!Directory.Exists(fullPath))
                                {
                                    Directory.CreateDirectory(fullPath);
                                }

                                // 5. Generate a unique file name to avoid overwriting
                                var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(fullPath, fileName);

                                // 6. Save the file to the specified path
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }



                                // Use the original file name
                                var originalFileName = Path.GetFileName(file.FileName);




                                WorkflowDocument workflowDocument = new WorkflowDocument();
                                workflowDocument.StepId = stepAction.Id;
                                workflowDocument.WorkflowId = WorkflowStepId;
                                workflowDocument.DocumentPath = filePath;
                                workflowDocument.DocumentName = originalFileName;
                                workflowDocument.UploadedOn = DateTime.Now;
                                await _context.AddAsync(workflowDocument);
                                await _context.SaveChangesAsync();
                                pathData.Add(new
                                {
                                    Name = originalFileName,
                                    Path = filePath,
                                    Id = workflowDocument.Id
                                });
                            }
                        }
                    }

                    // Find the workflow step
                    var workflowstep = await _context.WorkflowSteps.FindAsync(WorkflowStepId);
                    if (workflowstep != null)
                    {
                        var existingList = string.IsNullOrWhiteSpace(workflowstep.Actiondetails)
     ? new List<WorkflowStepActionDetails>()
     : JsonSerializer.Deserialize<List<WorkflowStepActionDetails>>(workflowstep.Actiondetails);

                        //var entry = existingList.FirstOrDefault(x => x.PerformedBy == stepAction.PerformedBy);

                        if (existingList == null)
                        {
                            existingList = new List<WorkflowStepActionDetails>();
                        }


                        WorkflowStepActionDetails entry = new WorkflowStepActionDetails
                        {
                            PerformedBy = stepAction.PerformedBy ?? 0,
                            ReceivedOn = DateTime.Now,
                            DueOn = DateTime.Now.AddDays(3),
                            Rights = "Edit",
                            StepAction = stepAction,
                            ExecutedOn = DateTime.Now,
                            Status = stepAction.approvalstatus == "reject" ? "Rejected" : "Approved",
                            Files = pathData
                        };
                        existingList.Add(entry);

                        // Create a combined JSON object with stepAction and pathData
                        //var combinedData = new
                        //{
                        //    StepAction = stepAction,
                        //    Files = pathData, // Embed pathData (list of files)
                        //};



                        // Serialize the combined object to JSON
                        string jsonString = JsonSerializer.Serialize(existingList, new JsonSerializerOptions { WriteIndented = true });
                        if (stepAction.ActionType == "Submit")
                        {
                            workflowstep.Actiondetails = jsonString;
                            var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(workflowstep.AssignedTo);

                            // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.AssignedTo);
                            bool all = true;
                            foreach (var item in deserializedData)
                            {
                                if (item["Id"].ToString() == stepAction.PerformedBy.ToString())
                                {
                                    if (approvalstatus == "reject")
                                    {
                                        item["Status"] = "Rejected"; // Modify the property
                                        item["Rights"] = "Edit";    // Modify another property

                                    }
                                    else
                                    {
                                        item["Status"] = "Approved"; // Modify the property
                                        item["Rights"] = "Edit";    // Modify another property
                                    }
                                }
                                if (item["Rights"].ToString() == "Edit" && item["Status"].ToString() != "Approved")
                                {
                                    all = false;
                                }
                            }
                            if (workflowstep.Type == "Any")
                            {
                                if (approvalstatus == "reject")
                                {
                                    workflowstep.Status = "Rejected";
                                }
                                else
                                {
                                    workflowstep.Status = "Approved";
                                }
                            }
                            else if (all == true)
                            {
                                if (approvalstatus == "reject")
                                {
                                    workflowstep.Status = "Rejected";
                                }
                                else
                                {
                                    workflowstep.Status = "Approved";
                                }
                            }
                            string jsonString2 = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                            workflowstep.AssignedTo = jsonString2;

                        }
                        else
                        {

                            workflowstep.Actiondetails = jsonString;
                        }
                        workflowstep.ExecutedOn = DateTime.Now;
                        workflowstep.ApprovedBy = approverid;
                        await _context.SaveChangesAsync();

                        var workflow = _context.Workflows.FirstOrDefault(w => w.Id == workflowstep.WorkflowId);


                        //if (workflowstep.Status == "Approved")
                        //{
                        //    var tasklst = await _context.UserTasks.Where(x => x.Id == tskid).FirstOrDefaultAsync();
                        //    tasklst.Status = "Approved";
                        //    await _context.SaveChangesAsync();
                        //}

                        #region Modifiction Request
                        if (workflowstep.Status == "Approved" && stepAction.ActionType == "Submit" && workflow.Subject == "Interaction Recording Form")
                        {
                            var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                            string nextstepname = string.Empty;
                            if (workflowstep.StepName == "Review Scope and Site Requirements") nextstepname = "Review Scope";
                            else if (workflowstep.StepName == "Review Scope") nextstepname = "Review Scope2";
                            else if (workflowstep.StepName == "Review Scope2") nextstepname = "Review Scope and Fees Calculation";
                            else if (workflowstep.StepName == "Review Scope and Fees Calculation") nextstepname = "Upload the Invoice";
                            else if (workflowstep.StepName == "Upload the Invoice") nextstepname = "Confirm Payment Received";

                            if (workflowstep.StepName == "Review Scope and Site Requirements" && (stepAction.Category == "Minor Work"
                                && stepAction.SubCat == "Without Charges"))
                            {
                                //  workflows.Status = "Approved";
                                //workflowstep.Status = "In Progress";

                                workflows.Status = "Approved";
                                workflows.Worktype = typeofWork;
                                workflows.ApprovalStartDate = stepAction.ApprovalStartDate;
                                workflows.ApprovalEndDate = stepAction.ApprovalEndDate;
                                await _context.SaveChangesAsync();

                                var tasklst = await _context.UserTasks.Where(x => x.WorkflowId == workflows.Id).ToListAsync();
                                foreach (var task in tasklst)
                                {
                                    task.Status = "Approved";
                                    await _context.SaveChangesAsync();
                                }

                                // Assuming uploadsDirectory was defined earlier in your code:
                                // var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");


                                // 2. Get the upload path from appsettings.json
                                var uploadsDirectory = _configuration.GetValue<string>("upload:path");


                                // Create a blank PDF document
                                string pdfFileName = "workpermit_" + WorkflowStepId + ".pdf";
                                string pdfPath = Path.Combine(uploadsDirectory, pdfFileName);

                                // Initialize a new PDF document
                                PdfDocument document = new PdfDocument();
                                document.Info.Title = "Work Permit";

                                // Add an empty page (optional: you can add content later)
                                PdfPage page = document.AddPage();

                                // Save the PDF to the specified path
                                document.Save(pdfPath);

                                // Optionally, you can add a record for the created PDF similar to other uploads
                                WorkflowDocument workflowDocument = new WorkflowDocument();
                                workflowDocument.StepId = stepAction.Id;
                                workflowDocument.WorkflowId = WorkflowStepId;
                                workflowDocument.DocumentPath = pdfPath;
                                workflowDocument.DocumentName = pdfFileName;
                                workflowDocument.UploadedOn = DateTime.Now;
                                await _context.AddAsync(workflowDocument);
                                await _context.SaveChangesAsync();

                                // Optionally add this document info to pathData if needed
                                pathData.Add(new
                                {
                                    Name = pdfFileName,
                                    Path = pdfPath
                                });


                                await _context.SaveChangesAsync();



                                // Step 1: Get the workflow step with ID = 161

                                int interactionid = Convert.ToInt32(workflow.InteractionId);
                                // Step 3: Get the email address from interaction with ID = 42
                                var emailDetails = _context.Interactions
                               .Where(i => i.Id == interactionid)
                               .FirstOrDefault();



                                var workpermitpath = await SaveWorkPermitAsPdfAsync(WorkflowStepId, emailDetails, "Bejoy", stepAction.Comments, typeofWork);

                                await SendPaymentConfirmationEmail(emailDetails.EmailAddress, stepAction.Comments, workpermitpath);


                                //await SendModificationEmail2("");
                                var successData = new
                                {
                                    Result = "Success",
                                    ErrorCode = "200",
                                    ErrorMessage = "",
                                    Data = true,
                                };
                                return new JsonResult(successData);
                            }

                            var nextstepflow = await _context.WorkflowSteps.Where(x => x.StepName == nextstepname && x.WorkflowId == workflowstep.WorkflowId).FirstOrDefaultAsync();
                            // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(nextstepflow.AssignedTo);
                            if (nextstepflow != null)
                            {

                                var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextstepflow.AssignedTo);

                                foreach (var item in deserializedData)
                                {
                                    if (item["Rights"].ToString() == "Edit")
                                    {
                                        var taskid = await CreateTask(nextstepflow.WorkflowId.Value, nextstepflow.Id.ToString(), "Workflow", Convert.ToInt32(item["Id"].ToString()), "Modification Request", workflows.InitiatorId.Value);
                                        item["TaskId"] = taskid;

                                    }
                                }

                                string json = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });

                                await _context.SaveChangesAsync();

                                nextstepflow.AssignedTo = json;
                                nextstepflow.Status = "In Progress";
                                nextstepflow.ReceivedOn = DateTime.Now;
                                nextstepflow.DueOn = DateTime.Now.AddDays(2);
                                await _context.SaveChangesAsync();
                            }

                            if (workflowstep.StepName == "Review Scope and Site Requirements")
                            {
                                workflows.ApprovalStartDate = stepAction.ApprovalStartDate;
                                workflows.ApprovalEndDate = stepAction.ApprovalEndDate;
                                workflows.Worktype = typeofWork;
                                await _context.SaveChangesAsync();

                            }

                            if (workflowstep.StepName == "Review Scope and Fees Calculation")
                            {
                                workflows.Amount = stepAction.Total;
                                await _context.SaveChangesAsync();





                                // Step 2: Get the workflow with ID = 26

                                int intractionid = Convert.ToInt32(workflow.InteractionId);
                                // Step 3: Get the email address from interaction with ID = 42
                                var emailDetails = _context.Interactions
                               .Where(i => i.Id == intractionid)
                               .Select(i => new
                               {
                                   i.EmailAddress,
                                   i.OwnerName,
                               })
                               .FirstOrDefault();


                                await SendPaymentEmailToCustomer(emailDetails.OwnerName, workflows.Amount.ToString(), emailDetails.EmailAddress);
                            }


                            if (workflowstep.StepName == "Upload the Invoice")
                            {
                                workflows.ReceiptDate = DateTime.Now;
                                workflows.ReceiptBy = "6";
                                workflows.ReceiptNo = GenerateFormattedId(Convert.ToInt32(workflows.InteractionId));
                                //workflows.Amount = stepAction.Total;
                                workflows.PaidBy = stepAction.PaidBy;
                                workflows.VendorName = stepAction.VendorName;
                                await _context.SaveChangesAsync();

                                var invoiceemail = _configuration.GetValue<string>("Emails:InvoiceEmail");
                                await SendInvoiceAndReceiptEmail(invoiceemail, files);
                            }

                            if (workflowstep.StepName == "Confirm Payment Received")
                            {
                                // 2. Get the upload path from appsettings.json
                                var uploadsDirectory = _configuration.GetValue<string>("upload:path");

                                //   var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                                if (!Directory.Exists(uploadsDirectory))
                                {
                                    Directory.CreateDirectory(uploadsDirectory);
                                }


                                workflows.Status = "Approved";
                                workflows.ApprovalStartDate = stepAction.ApprovalStartDate;
                                workflows.ApprovalEndDate = stepAction.ApprovalEndDate;
                                await _context.SaveChangesAsync();

                                var tasklst = await _context.UserTasks.Where(x => x.WorkflowId == workflows.Id).ToListAsync();
                                foreach (var task in tasklst)
                                {
                                    task.Status = "Approved";
                                    await _context.SaveChangesAsync();
                                }

                                // Assuming uploadsDirectory was defined earlier in your code:
                                // var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");


                                // Create a blank PDF document
                                string pdfFileName = "workpermit_" + WorkflowStepId + ".pdf";
                                string pdfPath = Path.Combine(uploadsDirectory, pdfFileName);

                                // Initialize a new PDF document
                                PdfDocument document = new PdfDocument();
                                document.Info.Title = "Work Permit";

                                // Add an empty page (optional: you can add content later)
                                PdfPage page = document.AddPage();

                                // Save the PDF to the specified path
                                document.Save(pdfPath);

                                // Optionally, you can add a record for the created PDF similar to other uploads
                                WorkflowDocument workflowDocument = new WorkflowDocument();
                                workflowDocument.StepId = stepAction.Id;
                                workflowDocument.WorkflowId = WorkflowStepId;
                                workflowDocument.DocumentPath = pdfPath;
                                workflowDocument.DocumentName = pdfFileName;
                                workflowDocument.UploadedOn = DateTime.Now;
                                await _context.AddAsync(workflowDocument);
                                await _context.SaveChangesAsync();

                                // Optionally add this document info to pathData if needed
                                pathData.Add(new
                                {
                                    Name = pdfFileName,
                                    Path = pdfPath
                                });

                                // Step 1: Get the workflow step with ID = 161
                                var ws = _context.WorkflowSteps.FirstOrDefault(w => w.Id == workflowstep.Id);

                                // Step 2: Get the workflow with ID = 26

                                int interactionid = Convert.ToInt32(workflow.InteractionId);
                                // Step 3: Get the email address from interaction with ID = 42
                                var emailDetails = _context.Interactions
                               .Where(i => i.Id == interactionid)
                               .FirstOrDefault();



                                var workpermitpath = await SaveWorkPermitAsPdfAsync(WorkflowStepId, emailDetails, "Bejoy", stepAction.Comments, workflow.Worktype);

                                await SendPaymentConfirmationEmail(emailDetails.EmailAddress, stepAction.Comments, workpermitpath);
                            }
                        }
                        #endregion


                        #region Contractor Registration
                        string upperCaseSubject = workflow.Subject.ToUpper();
                        if (workflowstep.Status == "Approved" && stepAction.ActionType == "Submit" && (upperCaseSubject == "CONTRACTOR REGISTRATION" || upperCaseSubject == "CONTRACTOR RENEWAL"))
                        {
                            var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                            string nextstepname = string.Empty;
                            if (workflowstep.StepName == "REVIEW DOCS") nextstepname = "APPROVE";
                            else if (workflowstep.StepName == "APPROVE") nextstepname = "UPLOAD INVOICE";
                            else if (workflowstep.StepName == "UPLOAD INVOICE") nextstepname = "FILE CLOSURE";



                            var nextstepflow = await _context.WorkflowSteps.Where(x => x.StepName == nextstepname && x.WorkflowId == workflowstep.WorkflowId).FirstOrDefaultAsync();
                            // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(nextstepflow.AssignedTo);
                            if (nextstepflow != null)
                            {

                                var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextstepflow.AssignedTo);

                                foreach (var item in deserializedData)
                                {
                                    if (item["Rights"].ToString() == "Edit")
                                    {
                                        var taskid = await CreateTask(nextstepflow.WorkflowId.Value, nextstepflow.Id.ToString(), "Workflow", Convert.ToInt32(item["Id"].ToString()), workflow.Subject, workflows.InitiatorId.Value);
                                        item["TaskId"] = taskid;

                                    }
                                }

                                string json = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });

                                await _context.SaveChangesAsync();

                                nextstepflow.AssignedTo = json;
                                nextstepflow.Status = "In Progress";
                                nextstepflow.ReceivedOn = DateTime.Now;
                                nextstepflow.DueOn = DateTime.Now.AddDays(2);
                                await _context.SaveChangesAsync();
                            }


                            if (workflowstep.StepName == "APPROVE")
                            {
                                int intid = Convert.ToInt32(workflow.InteractionId);
                                var contractor = await _context.Contractors.Where(x => x.Id == intid).FirstOrDefaultAsync();


                                if (contractor.Paymentoption == "monthly")
                                {
                                    workflow.Amount = 525;

                                }
                                else if (contractor.Paymentoption == "yearly")
                                {
                                    workflow.Amount = 5250;
                                }
                                await _context.SaveChangesAsync();

                                string requestno = GenerateCCFormattedId(intid);

                                await SendPaymentNotificationToContractor(contractor.CompanyName, workflows.Amount.ToString(), requestno, contractor.Email);

                            }

                            if (workflowstep.StepName == "UPLOAD INVOICE")
                            {

                                int intid = Convert.ToInt32(workflow.InteractionId);
                                var contractor = await _context.Contractors.Where(x => x.Id == intid).FirstOrDefaultAsync();
                                contractor.Bpnumber = formData["bpNumber"].ToString();
                                contractor.RenewalDate = DateTime.Now.AddYears(1);
                                await _context.SaveChangesAsync();

                                workflow.ReceiptDate = DateTime.Now;
                                workflow.ReceiptBy = "6";
                                workflow.ReceiptNo = GenerateCCFormattedId(Convert.ToInt32(workflows.InteractionId));
                                await _context.SaveChangesAsync();

                                string requestno = GenerateCCFormattedId(intid);

                                await SendPaymentReceiptToContractor(contractor.CompanyName, workflows.Amount.ToString(), DateTime.Now.ToShortDateString(), requestno, contractor.Email, formData["paidBy"].ToString(), formData["paymentMethod"].ToString());
                            }


                            if (workflowstep.StepName == "FILE CLOSURE")
                            {

                                var tasklst = await _context.UserTasks.Where(x => x.WorkflowId == workflows.Id).ToListAsync();
                                foreach (var task in tasklst)
                                {
                                    task.Status = "Approved";
                                    await _context.SaveChangesAsync();
                                }


                                workflows.Status = "Approved";
                                await _context.SaveChangesAsync();

                                int intid = Convert.ToInt32(workflow.InteractionId);
                                var contractor = await _context.Contractors.Where(x => x.Id == intid).FirstOrDefaultAsync();
                                contractor.Isactive = true;
                                await _context.SaveChangesAsync();

                            }

                            //if (workflowstep.StepName == "APPROVE")
                            //{
                            //    workflows.Amount = stepAction.Total;
                            //    await _context.SaveChangesAsync();





                            //    // Step 2: Get the workflow with ID = 26
                            //    int contid = Convert.ToInt32(workflow.InteractionId);

                            //    // Step 3: Get the email address from interaction with ID = 42
                            //    var emailDetails = _context.Contractors
                            //   .Where(i => i.Id == contid)
                            //   .Select(i => new
                            //   {
                            //       i.Email,
                            //       i.CompanyName
                            //   })
                            //   .FirstOrDefault();


                            //    await SendPaymentEmailToContractor(emailDetails.CompanyName, workflows.Amount.ToString(), emailDetails.Email);
                            //}


                            //if (workflowstep.StepName == "UPLOAD INVOICE")
                            //{
                            //    workflows.ReceiptDate = DateTime.Now;
                            //    workflows.ReceiptBy = "6";
                            //    workflows.ReceiptNo = GenerateFormattedId(Convert.ToInt32(workflows.InteractionId));
                            //    workflows.Amount = stepAction.Total;
                            //    workflows.PaidBy = stepAction.PaidBy;
                            //    workflows.VendorName = stepAction.VendorName;
                            //    await _context.SaveChangesAsync();

                            //    var invoiceemail = _configuration.GetValue<string>("Emails:InvoiceEmail");
                            //    await SendInvoiceAndReceiptEmail(invoiceemail, files);
                            //}


                        }
                        #endregion


                        #region Resale NOC

                        if ((workflowstep.Status == "Approved" || workflowstep.Status == "Rejected") && stepAction.ActionType == "Submit" && (upperCaseSubject == "RESALE NOC" || upperCaseSubject == "RESALE NOC"))
                        {


                            var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                            string nextstepname = string.Empty;
                            if (workflowstep.StepName == "INSPECT PROPERTY") nextstepname = "APPROVE";


                            if (approvalstatus != "reject")
                            {

                                var nextstepflow = await _context.WorkflowSteps.Where(x => x.StepName == nextstepname && x.WorkflowId == workflowstep.WorkflowId).FirstOrDefaultAsync();
                                // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(nextstepflow.AssignedTo);
                                if (nextstepflow != null)
                                {

                                    var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextstepflow.AssignedTo);

                                    foreach (var item in deserializedData)
                                    {
                                        if (item["Rights"].ToString() == "Edit")
                                        {
                                            var taskid = await CreateTask(nextstepflow.WorkflowId.Value, nextstepflow.Id.ToString(), "Workflow", Convert.ToInt32(item["Id"].ToString()), workflow.Subject, workflows.InitiatorId.Value);
                                            item["TaskId"] = taskid;

                                        }
                                    }

                                    string json = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });

                                    await _context.SaveChangesAsync();

                                    nextstepflow.AssignedTo = json;
                                    nextstepflow.Status = "In Progress";
                                    nextstepflow.ReceivedOn = DateTime.Now;
                                    nextstepflow.DueOn = DateTime.Now.AddDays(2);
                                    await _context.SaveChangesAsync();
                                }

                                if (workflowstep.StepName == "APPROVE")
                                {

                                    var tasklst = await _context.UserTasks.Where(x => x.WorkflowId == workflows.Id).ToListAsync();
                                    foreach (var task in tasklst)
                                    {
                                        task.Status = "Approved";
                                        await _context.SaveChangesAsync();
                                    }


                                    workflows.Status = "Approved";
                                    await _context.SaveChangesAsync();


                                    int resalenocid = Convert.ToInt32(workflow.InteractionId);
                                    // Step 3: Get the email address from interaction with ID = 42
                                    var resalenoc = _context.Resalenocs
                                   .Where(i => i.Id == resalenocid)
                                   .FirstOrDefault();

                                    var currentstep = await _context.Users.Where(x => x.Id == approverid).FirstOrDefaultAsync();
                                    var user = await _context.Users.Where(x => x.Username == resalenoc.Intiatorname).FirstOrDefaultAsync();

                                    approveresalenoc(user.Email, "Sales Team", resalenoc.Unitno, stepAction.Checkboxes, files, currentstep.Username);
                                }

                            }
                            else
                            {
                                int resalenocid = Convert.ToInt32(workflow.InteractionId);

                                // Step 1: Retrieve Resale NOC by ID
                                var resalenoc = await _context.Resalenocs.FirstOrDefaultAsync(i => i.Id == resalenocid);
                                if (resalenoc == null)
                                {
                                    throw new Exception($"Resalenoc with ID {resalenocid} not found.");
                                }

                                // Step 2: Get current approver and initiator users
                                var currentStepUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == approverid);
                                if (currentStepUser == null)
                                {
                                    throw new Exception($"Approver with ID {approverid} not found.");
                                }

                                if (string.IsNullOrEmpty(resalenoc.Intiatorname))
                                {
                                    throw new Exception("Resalenoc.Intiatorname is null or empty.");
                                }

                                var initiatorUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == resalenoc.Intiatorname);
                                if (initiatorUser == null)
                                {
                                    throw new Exception($"Initiator user with username '{resalenoc.Intiatorname}' not found.");
                                }

                                // Step 3: Send rejection email
                                rejectresalenoc(
                                    initiatorUser.Email,
                                    "Sales Team",
                                    resalenoc.Unitno,
                                    stepAction.Checkboxes,
                                    files,
                                    currentStepUser.Username
                                );

                                // Step 4: Reject all tasks
                                var taskList = await _context.UserTasks.Where(x => x.WorkflowId == workflows.Id).ToListAsync();
                                foreach (var task in taskList)
                                {
                                    task.Status = "Rejected";
                                }

                                // Step 5: Reject workflow and save
                                workflows.Status = "Rejected";
                                await _context.SaveChangesAsync();

                            }

                        }
                        #endregion



                        if (workflowstep.Type == "Any")
                        {
                            var tasklst = await _context.UserTasks.Where(x => x.StepId == workflowstep.Id).ToListAsync();
                            foreach (var item in tasklst)
                            {
                                item.Status = "Approved";
                                await _context.SaveChangesAsync();
                            }


                        }

                        if (workflowstep.Type == "All")
                        {
                            var tasklst = await _context.UserTasks.Where(x => x.StepId == workflowstep.Id).ToListAsync();
                            if (tasklst.Count() == 1)
                            {
                                foreach (var item in tasklst)
                                {
                                    item.Status = "Approved";
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                var tsklst = await _context.UserTasks.Where(x => x.Id == tskid).FirstOrDefaultAsync();
                                tsklst.Status = "Approved";
                                await _context.SaveChangesAsync();

                            }


                        }

                        if (approvalstatus == "reject")
                        {
                            var tasklst = await _context.UserTasks.Where(x => x.StepId == workflowstep.Id).ToListAsync();
                            foreach (var item in tasklst)
                            {
                                item.Status = "Rejected";
                                await _context.SaveChangesAsync();
                            }
                        }



                        var Data = new
                        {
                            Result = "Success",
                            ErrorCode = "200",
                            ErrorMessage = "",
                            Data = true,
                        };
                        return new JsonResult(Data);
                    }
                    else
                    {
                        var successData = new
                        {
                            Result = "workflowstep is not found",
                            ErrorCode = "404",
                            ErrorMessage = "",
                            Data = false,
                        };
                        return new JsonResult(successData);
                    }
                }
                else
                {
                    var successData = new
                    {
                        Result = "Object Conversion Error",
                        ErrorCode = "500",
                        ErrorMessage = "",
                        Data = false,
                    };
                    return new JsonResult(successData);
                }
            }
            catch (Exception ex)
            {
                var errorData = new
                {
                    Result = "Exception Occurred",
                    ErrorCode = "500",
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace, // Optional, remove if not needed
                    InnerException = ex.InnerException?.Message // Optional, remove if not needed
                };
                return new JsonResult(errorData);
            }
        }



        [HttpPost("CreateWorkFlowStepActionRFI")]
        public async Task<bool> CreateWorkFlowStepActionRFI(int WorkflowStepId, [FromForm] IFormCollection formData)
        {
            var stepAction = await this.ConvertformDataToStepAction(formData);
            stepAction.StepId = WorkflowStepId;
            stepAction.PerformedOn = DateTime.Now;
            await _context.AddAsync(stepAction);
            await _context.SaveChangesAsync();

            var taskname = "";



            // Find the workflow step
            var workflowstep = await _context.WorkflowSteps.FindAsync(WorkflowStepId);
            if (workflowstep != null)
            {
                var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);

                if (workflows.Subject == "Interaction Recording Form")
                {
                    taskname = "Modification Request";
                }
                else if (workflows.Subject == "CONTRACTOR REGISTRATION")
                {
                    taskname = "CONTRACTOR REGISTRATION";
                }
                else if (workflows.Subject == "Resale NOC")
                {
                    taskname = "Resale NOC";
                }

                var taskId = await CreateTask(workflowstep.WorkflowId.Value, WorkflowStepId.ToString(), "RFI", stepAction.AssignTo.Value, taskname, workflows.InitiatorId.Value);
                // Deserialize the AssignedTo and Details fields
                var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.AssignedTo);
                List<dynamic>? deserializedData2 = new List<dynamic>();

                if (workflowstep.Details != null)
                    deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.Details);

                // Add the new data to the deserialized lists
                deserializedData.Add(new
                {
                    StepId = stepAction.Id,
                    Id = stepAction.AssignTo.ToString(),
                    Status = "Not Approved",
                    Rights = "RFI",
                    TaskId = taskId
                });

                //deserializedData2.Add(new
                //{
                //    StepId = stepAction.Id,
                //    Id = stepAction.AssignTo.ToString(),
                //    Status = "Not Approved",
                //    Rights = "RFI",
                //    Comment = stepAction.Comments,
                //    RequestedBy = stepAction.PerformedBy.ToString(),
                //    PerformedOn = DateTime.Now,
                //    IterationType = "RFI"
                //});

                // Serialize the updated data back to JSON
                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });

                workflowstep.AssignedTo = jsonString;
                workflowstep.Details = jsonString2;

                var pathData = new List<dynamic>();

                var files = formData.Files;
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // 2. Get the upload path from appsettings.json
                            var uploadPath = _configuration.GetValue<string>("upload:path");


                            // 3. Combine with the current directory to get the full path
                            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);


                            // 4. Ensure the directory exists
                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }

                            // 5. Generate a unique file name to avoid overwriting
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(fullPath, fileName);

                            // 6. Save the file to the specified path
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Use the original file name
                            var originalFileName = Path.GetFileName(file.FileName);


                            WorkflowDocument workflowDocument = new WorkflowDocument();
                            workflowDocument.StepId = stepAction.Id;
                            workflowDocument.WorkflowId = WorkflowStepId;
                            workflowDocument.DocumentPath = filePath;
                            workflowDocument.DocumentName = originalFileName;
                            workflowDocument.UploadedOn = DateTime.Now;
                            await _context.AddAsync(workflowDocument);
                            await _context.SaveChangesAsync();
                            pathData.Add(new
                            {
                                Name = originalFileName,
                                Path = filePath,
                                Id = workflowDocument.Id
                            });
                        }
                    }
                }


                await _context.SaveChangesAsync();



                // Call CreateTask and wait for the TaskId


                // Update deserializedData2 with the returned TaskId
                deserializedData2.Add(new
                {
                    StepId = stepAction.Id,
                    Id = stepAction.AssignTo.ToString(),
                    Status = "Not Approved",
                    Rights = "RFI",
                    Comment = stepAction.Comments,
                    RequestedBy = stepAction.PerformedBy.ToString(),
                    PerformedOn = DateTime.Now,
                    IterationType = "RFI",
                    TaskId = taskId,  // Add TaskId here
                    Files = pathData
                });

                // Serialize the updated data again
                jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                workflowstep.Details = jsonString2;

                // Save the updated workflow step
                await _context.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }



        [HttpPost("CreateWorkFlowStepActionReassign")]
        public async Task<bool> CreateWorkFlowStepActionReassign(int WorkflowStepId, [FromForm] IFormCollection formData)
        {
            var stepAction = await this.ConvertformDataToStepAction(formData);
            stepAction.StepId = WorkflowStepId;
            stepAction.PerformedOn = DateTime.Now;
            await _context.AddAsync(stepAction);
            await _context.SaveChangesAsync();
            var taskname = "";
            // Find the workflow step
            var workflowstep = await _context.WorkflowSteps.FindAsync(WorkflowStepId);
            if (workflowstep != null)
            {
                var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                if (workflows.Subject == "Interaction Recording Form")
                {
                    taskname = "Modification Request";
                }
                else if (workflows.Subject == "CONTRACTOR REGISTRATION")
                {
                    taskname = "CONTRACTOR REGISTRATION";
                }
                else if (workflows.Subject == "Resale NOC")
                {
                    taskname = "Resale NOC";
                }
                var taskId = await CreateTask(workflowstep.WorkflowId.Value, WorkflowStepId.ToString(), "Reassigned", stepAction.AssignTo.Value, taskname, workflows.InitiatorId.Value);
                // Serialize the combined object to JSON
                // string jsonString = JsonSerializer.Serialize(combinedData, new JsonSerializerOptions { WriteIndented = true });
                var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(workflowstep.AssignedTo);

                var pathData = new List<dynamic>();

                var files = formData.Files;
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // 2. Get the upload path from appsettings.json
                            var uploadPath = _configuration.GetValue<string>("upload:path");


                            // 3. Combine with the current directory to get the full path
                            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);


                            // 4. Ensure the directory exists
                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }

                            // 5. Generate a unique file name to avoid overwriting
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(fullPath, fileName);

                            // 6. Save the file to the specified path
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Use the original file name
                            var originalFileName = Path.GetFileName(file.FileName);


                            WorkflowDocument workflowDocument = new WorkflowDocument();
                            workflowDocument.StepId = stepAction.Id;
                            workflowDocument.WorkflowId = WorkflowStepId;
                            workflowDocument.DocumentPath = filePath;
                            workflowDocument.DocumentName = originalFileName;
                            workflowDocument.UploadedOn = DateTime.Now;
                            await _context.AddAsync(workflowDocument);
                            await _context.SaveChangesAsync();
                            pathData.Add(new
                            {
                                Name = originalFileName,
                                Path = filePath,
                                Id = workflowDocument.Id
                            });
                        }
                    }
                }
                //List<dynamic>? deserializedData2 = new List<dynamic>();
                //if (workflowstep.Details != null)
                //    deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.Details);
                foreach (var item in deserializedData)
                {
                    if (item["Rights"].ToString() == "Edit" && item["Id"].ToString() == stepAction.PerformedBy.ToString())
                    {
                        item["Status"] = "Not Approved";
                        item["Rights"] = "View";
                        item["TaskId"] = taskId;

                    }
                }
                //foreach (var item in deserializedData2)
                //{
                //    //  item["Files"] = pathData;
                //    var jsonObj = (JsonObject)item;
                //    jsonObj["Files"] = JsonSerializer.SerializeToNode(pathData); // or JsonArray if already JSON

                //}
                JsonArray deserializedData2 = null;
                if (workflowstep.Details != null)
                {
                     deserializedData2 = JsonNode.Parse(workflowstep.Details).AsArray();

                    foreach (JsonNode item in deserializedData2)
                    {
                        // Add or update "Files"
                        item["Files"] = JsonSerializer.SerializeToNode(pathData);
                    }
                }
                // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.AssignedTo);

                var data = new Dictionary<string, object> {
                    { "Id" , stepAction.AssignTo.ToString() },{ "Status" , "Not Approved" },{ "Rights" , "Edit" } , { "TaskId" , taskId }, { "IterationType" , "Reassigned" }, {"PerformedOn" , DateTime.Now }, { "RequestedBy" , "" } , {"RequestedTo" , ""}
                };

                deserializedData.Add(data);
                //deserializedData2.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "Edit", Comment = stepAction.Comments, RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "Reassigned" });

                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                workflowstep.Details = jsonString2;
                workflowstep.AssignedTo = jsonString;
                // workflowstep.ExecutedOn = DateTime.Now;
                await _context.SaveChangesAsync();



                deserializedData2.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "Edit", Comment = stepAction.Comments, RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "Reassigned", TaskId = taskId });


                // Serialize the updated data again
                jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                workflowstep.Details = jsonString2;


                // Save the updated workflow step
                await _context.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }


        //public async Task<string> SaveWorkPermitAsPdfAsync(int workflowStepId)
        //{
        //    // Retrieve the uploads directory from configuration
        //    var uploadsDirectory = _configuration.GetValue<string>("upload:path");

        //    // Define the output PDF file name and path
        //    string pdfFileName = $"workpermit_{workflowStepId}.pdf";
        //    string pdfPath = Path.Combine(uploadsDirectory, pdfFileName);

        //    // Load the HTML template content
        //    var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        //    var htmlTemplatePath = Path.Combine(wwwRootPath, "Templates", "PDF.html");
        //    string htmlContent = await System.IO.File.ReadAllTextAsync(htmlTemplatePath);

        //    // Replace placeholders in the HTML content with dynamic values
        //    htmlContent = htmlContent
        //        .Replace("MM/DD/YYYY", DateTime.Now.ToString("MM/dd/yyyy")) // Replace date placeholders
        //        .Replace("Enter Permit Code", "PERMIT12345")               // Replace permit code
        //        .Replace("123 Anywhere St., Any City, ST 12345", "Dynamic Address")
        //        .Replace("afdsgfhdhgfjgfjghgkgkughhlhlkg;sdghgjigkjl", "Dynamic Work Description");

        //    // Create a DinkToPdf converter instanceReview Scope and Fees Calculation
        //    var converter = new SynchronizedConverter(new PdfTools());

        //    // Configure the PDF generation settings
        //    var pdfDocument = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = new GlobalSettings
        //        {
        //            ColorMode = ColorMode.Color,
        //            Orientation = Orientation.Portrait,
        //            PaperSize = PaperKind.A4,
        //            Out = pdfPath // Specify the output path for the PDF file
        //        }
        //    };

        //    // Add the HTML content to the document
        //    pdfDocument.Objects.Add(new ObjectSettings
        //    {
        //        HtmlContent = htmlContent, // HTML content to be converted to PDF
        //        WebSettings = { DefaultEncoding = "utf-8" }
        //    });

        //    // Generate the PDF
        //    converter.Convert(pdfDocument);

        //    // Return the file path of the saved PDF
        //    return pdfPath;
        //}

        [HttpPost("SaveWorkPermitAsPdfAsync")]
        [AllowAnonymous]
        public async Task<string> SaveWorkPermitAsPdfAsync(int workflowStepId, Interaction intobj, string approvalby, string comments, string typeofwork)
        {
            // Retrieve the uploads directory from configuration
            var uploadsDirectory = _configuration.GetValue<string>("upload:path");

            if (string.IsNullOrEmpty(uploadsDirectory))
            {
                throw new InvalidOperationException("The upload path is not configured.");
            }

            // Ensure the uploads directory exists
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            // Define the output PDF file name and path
            string pdfFileName = $"workpermit_{workflowStepId}.pdf";
            string pdfPath = Path.Combine(uploadsDirectory, pdfFileName);

            // Load the HTML template content
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var htmlTemplatePath = Path.Combine(wwwRootPath, "Templates", "PDF.html");

            if (!System.IO.File.Exists(htmlTemplatePath))
            {
                throw new FileNotFoundException($"The HTML template was not found at {htmlTemplatePath}");
            }

            string htmlContent = await System.IO.File.ReadAllTextAsync(htmlTemplatePath);

            var contractorid = await _context.Contractors.Where(x => x.CompanyName == intobj.ContractorCompName).FirstOrDefaultAsync();

            string contid = "";
            if (contractorid != null)
            {
                contid = contractorid.Id.ToString();
            }

            htmlContent = htmlContent
             .Replace("STARTDATE", Convert.ToDateTime(intobj.Date).ToString("dd/MM/yyyy"))
             .Replace("ENDDATE", Convert.ToDateTime(intobj.EndDuration).ToString("dd/MM/yyyy"))
                .Replace("UNIT#", intobj.UnitNumber)
                .Replace("PERMIT#", "PERMIT" + intobj.Id.ToString("D5"))
                .Replace("CONTRACTORNAME", intobj.ContractorCompName)
                .Replace("TRADELICENCE", intobj.TradeLicenceNo)
                .Replace("CONTRACTORID", contid)
                .Replace("WORKDESCRIPTION", intobj.InternalWork)
                .Replace("VILLA#", intobj.UnitNumber)
                .Replace("TYPEOFWORK", typeofwork)
                .Replace("APPROVALBY", approvalby)
                .Replace("AREAOFWORK", intobj.InternalWork)
                .Replace("SPECIALCOMMENTS", comments);

            // Save the updated HTML content to a temporary file
            string tempHtmlFilePath = Path.Combine(uploadsDirectory, $"workpermit_{workflowStepId}.html");
            await System.IO.File.WriteAllTextAsync(tempHtmlFilePath, htmlContent);

            try
            {
                // Generate PDF using PdfGenerator
                var pdfGenerator = new PdfGenerator();
                pdfGenerator.GeneratePdf(tempHtmlFilePath, pdfPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                throw new Exception($"Failed to generate the PDF for workflowStepId {workflowStepId}", ex);
            }
            finally
            {
                // Clean up temporary HTML file
                if (System.IO.File.Exists(tempHtmlFilePath))
                {
                    System.IO.File.Delete(tempHtmlFilePath);
                }
            }

            // Return the file path of the saved PDF
            return pdfPath;
        }


        [HttpPost("ReturnWorkFlowStepAction")]
        public async Task<bool> ReturnWorkFlowStepAction(int WorkflowStepId, StepAction stepAction)
        {
            stepAction.StepId = WorkflowStepId;
            stepAction.PerformedOn = DateTime.Now;
            await _context.AddAsync(stepAction);
            await _context.SaveChangesAsync();



            // Find the workflow step
            var workflowstep = await _context.WorkflowSteps.FindAsync(WorkflowStepId);
            if (workflowstep != null)
            {

                var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                if (workflows.Subject == "CONTRACTOR REGISTRATION" || workflows.Subject == "CONTRACTOR RENEWAL")
                {
                    string prestepname = string.Empty;

                    if (workflowstep.StepName == "APPROVE") prestepname = "REVIEW DOCS";
                    else if (workflowstep.StepName == "UPLOAD INVOICE") prestepname = "APPROVE";
                    else if (workflowstep.StepName == "FILE CLOSURE") prestepname = "UPLOAD INVOICE";

                    var prevstepflow = await _context.WorkflowSteps.Where(x => x.StepName == prestepname && x.WorkflowId == workflowstep.WorkflowId).FirstOrDefaultAsync();
                    var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(prevstepflow.AssignedTo);


                    var currentstepjson = JArray.Parse(workflowstep.AssignedTo);

                    foreach (var obj in currentstepjson)
                    {
                        if (obj["Status"]?.ToString() == "Approved")
                        {
                            obj["Status"] = "Not Approved";
                        }
                    }
                    // Current Step Json
                    string u_currentstepjson = JsonConvert.SerializeObject(currentstepjson, Formatting.Indented);



                    var previousstepjson = JArray.Parse(prevstepflow.AssignedTo);

                    foreach (var obj in previousstepjson)
                    {
                        if (obj["Status"]?.ToString() == "Approved")
                        {
                            obj["Status"] = "Not Approved";
                        }
                    }
                    //Previous Step Json
                    string p_currentstepjson = JsonConvert.SerializeObject(previousstepjson, Formatting.Indented);


                    foreach (var item in deserializedData)
                    {
                        if (item["Rights"].ToString() == "Edit")
                        {
                            var taskid = await CreateTask(workflowstep.WorkflowId.Value, prevstepflow.Id.ToString(), "Return Step", Convert.ToInt32(item["Id"].ToString()), "CONTRACTOR REGISTRATION", workflows.InitiatorId.Value);
                            prevstepflow.Status = "In Progress";
                            prevstepflow.AssignedTo = p_currentstepjson;
                            await _context.SaveChangesAsync();
                            item["Status"] = "Not Approved";
                            item["IterationType"] = "Return Step";
                            item["PerformedOn"] = DateTime.Now;
                            item["TaskId"] = taskid;
                        }
                    }
                    List<dynamic>? deserializedData2 = new List<dynamic>();
                    if (workflowstep.Details != null)
                        deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.Details);
                    deserializedData2.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "Edit", Comment = "", RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "Return Step" });

                    string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                    string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                    workflowstep.AssignedTo = u_currentstepjson;
                    workflowstep.Details = jsonString2;
                    // workflowstep.ExecutedOn = DateTime.Now;
                    workflowstep.Status = "Not Started";
                    await _context.SaveChangesAsync();




                }
                else if (workflows.Subject == "Interaction Recording Form")
                {
                    string prestepname = string.Empty;

                    if (workflowstep.StepName == "Confirm Payment Received") prestepname = "Upload the Invoice";
                    else if (workflowstep.StepName == "Upload the Invoice") prestepname = "Review Scope and Fees Calculation";
                    else if (workflowstep.StepName == "Review Scope and Fees Calculation") prestepname = "Review Scope2";
                    else if (workflowstep.StepName == "Review Scope2") prestepname = "Review Scope";
                    else if (workflowstep.StepName == "Review Scope") prestepname = "Review Scope and Site Requirements";
                    var prevstepflow = await _context.WorkflowSteps.Where(x => x.StepName == prestepname && x.WorkflowId == workflowstep.WorkflowId).FirstOrDefaultAsync();
                    var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(prevstepflow.AssignedTo);


                    var currentstepjson = JArray.Parse(workflowstep.AssignedTo);

                    foreach (var obj in currentstepjson)
                    {
                        if (obj["Status"]?.ToString() == "Approved")
                        {
                            obj["Status"] = "Not Approved";
                        }
                    }
                    // Current Step Json
                    string u_currentstepjson = JsonConvert.SerializeObject(currentstepjson, Formatting.Indented);



                    var previousstepjson = JArray.Parse(prevstepflow.AssignedTo);

                    foreach (var obj in previousstepjson)
                    {
                        if (obj["Status"]?.ToString() == "Approved")
                        {
                            obj["Status"] = "Not Approved";
                        }
                    }
                    //Previous Step Json
                    string p_currentstepjson = JsonConvert.SerializeObject(previousstepjson, Formatting.Indented);


                    foreach (var item in deserializedData)
                    {
                        if (item["Rights"].ToString() == "Edit")
                        {
                            var taskid = await CreateTask(workflowstep.WorkflowId.Value, prevstepflow.Id.ToString(), "Return Step", Convert.ToInt32(item["Id"].ToString()), "Modification Request", workflows.InitiatorId.Value);
                            prevstepflow.Status = "In Progress";
                            prevstepflow.AssignedTo = p_currentstepjson;
                            await _context.SaveChangesAsync();
                            item["Status"] = "Not Approved";
                            item["IterationType"] = "Return Step";
                            item["PerformedOn"] = DateTime.Now;
                            item["TaskId"] = taskid;
                        }
                    }
                    List<dynamic>? deserializedData2 = new List<dynamic>();
                    if (workflowstep.Details != null)
                        deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.Details);
                    deserializedData2.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "Edit", Comment = "", RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "Return Step" });

                    string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                    string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                    workflowstep.AssignedTo = u_currentstepjson;
                    workflowstep.Details = jsonString2;
                    // workflowstep.ExecutedOn = DateTime.Now;
                    workflowstep.Status = "Not Started";
                    await _context.SaveChangesAsync();



                }
                else if (workflows.Subject == "Resale NOC")
                {
                    string prestepname = string.Empty;

                    if (workflowstep.StepName == "APPROVE") prestepname = "INSPECT PROPERTY";

                    var prevstepflow = await _context.WorkflowSteps.Where(x => x.StepName == prestepname && x.WorkflowId == workflowstep.WorkflowId).FirstOrDefaultAsync();
                    var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(prevstepflow.AssignedTo);


                    var currentstepjson = JArray.Parse(workflowstep.AssignedTo);

                    foreach (var obj in currentstepjson)
                    {
                        if (obj["Status"]?.ToString() == "Approved")
                        {
                            obj["Status"] = "Not Approved";
                        }
                    }
                    // Current Step Json
                    string u_currentstepjson = JsonConvert.SerializeObject(currentstepjson, Formatting.Indented);



                    var previousstepjson = JArray.Parse(prevstepflow.AssignedTo);

                    foreach (var obj in previousstepjson)
                    {
                        if (obj["Status"]?.ToString() == "Approved")
                        {
                            obj["Status"] = "Not Approved";
                        }
                    }
                    //Previous Step Json
                    string p_currentstepjson = JsonConvert.SerializeObject(previousstepjson, Formatting.Indented);


                    foreach (var item in deserializedData)
                    {
                        if (item["Rights"].ToString() == "Edit")
                        {
                            var taskid = await CreateTask(workflowstep.WorkflowId.Value, prevstepflow.Id.ToString(), "Return Step", Convert.ToInt32(item["Id"].ToString()), "Resale NOC", workflows.InitiatorId.Value);
                            prevstepflow.Status = "In Progress";
                            prevstepflow.AssignedTo = p_currentstepjson;
                            await _context.SaveChangesAsync();
                            item["Status"] = "Not Approved";
                            item["IterationType"] = "Return Step";
                            item["PerformedOn"] = DateTime.Now;
                            item["TaskId"] = taskid;
                        }
                    }
                    List<dynamic>? deserializedData2 = new List<dynamic>();
                    if (workflowstep.Details != null)
                        deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.Details);
                    deserializedData2.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "Edit", Comment = "", RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "Return Step" });

                    string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                    string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                    workflowstep.AssignedTo = u_currentstepjson;
                    workflowstep.Details = jsonString2;
                    // workflowstep.ExecutedOn = DateTime.Now;
                    workflowstep.Status = "Not Started";
                    await _context.SaveChangesAsync();



                }


                //await CreateTask(workflowstep.WorkflowId.Value, WorkflowStepId.ToString(), "RFI", stepAction.AssignTo.Value, "Modification Request", workflows.InitiatorId.Value);



                return true;
            }
            else
            {
                return false;
            }
        }


        [HttpPost("CreateTask")]
        [AllowAnonymous]
        private async Task<int> CreateTask(int WorkflowId, string WorkflowStepId, string type, int assignto, string template, int initiatorid)
        {
            var user = await _context.Users.FindAsync(initiatorid);
            Models.UserTask task = new Models.UserTask();
            task.WorkflowId = WorkflowId;
            task.StepId = Convert.ToInt32(WorkflowStepId);
            task.TaskType = type;
            task.Status = "In Progress";
            task.AssignedTo = assignto;
            task.DueDate = DateTime.Now.AddDays(2);
            task.Template = template;
            task.Department = user.Department;
            await _context.AddAsync(task);
            await _context.SaveChangesAsync();
            await SendModificationEmail(user);
            return task.Id;
        }
        [HttpPost("GetTaks")]
        [AllowAnonymous]
        public async Task<IEnumerable<TaksVM>> GetTaks(int userid)
        {
            try
            {

                var query = from ut in _context.UserTasks
                            join wf in _context.Workflows on ut.WorkflowId equals wf.Id
                            join ws in _context.WorkflowSteps on ut.StepId equals ws.Id
                            where ut.AssignedTo == userid
                            select new
                            {
                                ut.Id,
                                ut.WorkflowId,
                                ut.StepId,
                                ut.TaskType,
                                ut.Department,
                                ut.Template,
                                ut.DueDate,
                                ws.ReceivedOn,
                                ws.DueOn,
                                ut.Ageing,
                                ut.Status,
                                ut.AssignedTo,
                                WorkflowData = wf,
                                IsViewed = ut.Isviewed,
                                InteractionId = wf.InteractionId
                            };

                var result = await query.ToListAsync();


                var tasksList = result.Select(item => new TaksVM
                {
                    Id = item.Id,
                    WorkflowId = item.WorkflowId,
                    StepId = item.StepId,
                    TaskType = item.TaskType,
                    Department = item.Department,
                    Template = item.Template,
                    DueDate = item.DueDate,
                    ReceivedOn = item.ReceivedOn,
                    DueOn = item.DueOn,
                    Ageing = CalculateAgeing(item.DueDate),
                    Status = item.Status,
                    AssignedTo = item.AssignedTo,
                    Isviewed = item.IsViewed,
                    Unique_Id = item.InteractionId != null ? GenerateNmeFormattedId(Convert.ToInt32(item.InteractionId), item.Template) : ""
                }).OrderByDescending(x => x.Id).ToList();

                return tasksList;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return new List<TaksVM>();
            }
        }

        [HttpGet("CalculateAgeing")]
        [AllowAnonymous]
        private int CalculateAgeing(DateTime? dueDate)
        {
            if (!dueDate.HasValue)
                return 0;

            // Consider using UtcNow or standardized time if needed.
            var now = DateTime.Now.Date;
            var target = dueDate.Value.Date;

            // Days passed since due date (if in the past) 
            // or days remaining until due date (if in the future, result will be negative).
            int difference = (now - target).Days;

            return difference > 0 ? difference : 0;
        }


        [HttpPost("TaskViewed")]
        public async Task<bool> TaskViewed(int taskid)
        {
            try
            {
                var lst = await _context.UserTasks.Where(x => x.Id == taskid).FirstOrDefaultAsync();
                lst.Isviewed = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        //[HttpPost("GetWorkFlow")]
        //public async Task<WorkFlowVM> GetWorkFlow(int workflowid)
        //{
        //    try
        //    {
        //        var wf = await _context.Workflows.FindAsync(workflowid);
        //        var WorkFlowVMobj = Mapper.MapToDto<Workflow, WorkFlowVM>(wf);
        //        var lst = await _context.WorkflowSteps.Where(x => x.WorkflowId == workflowid).ToListAsync();
        //        var WFSslst = Mapper.MapToDtos<WorkflowStep, WorkFlowStepVM>(lst);
        //        WorkFlowVMobj.workFlowStepVMs = WFSslst;
        //        // Calculate progress based on the status of workflow steps
        //        int totalSteps = WFSslst.Count();
        //        int completedSteps = WFSslst.Count(x => x.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase));

        //        int progressPercentage = totalSteps > 0
        //     ? (completedSteps * 100) / totalSteps
        //     : 0;

        //        // Convert progress to string
        //        WorkFlowVMobj.Progress = progressPercentage.ToString();

        //        WorkFlowVMobj.WorkflowTypeName = (await _context.WorkflowTypes.FindAsync(WorkFlowVMobj.WorkflowTypeId)).Name;
        //        WorkFlowVMobj.InitiatorName = (await _context.Users.FindAsync(WorkFlowVMobj.InitiatorId)).Username;
        //        WorkFlowVMobj.Department = (await _context.Users.FindAsync(WorkFlowVMobj.InitiatorId)).Department;
        //        return WorkFlowVMobj;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        [HttpPost("GetWorkFlow")]
        public async Task<WorkFlowVM> GetWorkFlow(int workflowid)
        {
            try
            {
                // 1. Fetch the workflow
                var wf = await _context.Workflows.FindAsync(workflowid);
                if (wf == null)
                {
                    return null; // or handle not found as you wish
                }

                // 2. Map workflow entity to WorkFlowVM
                var WorkFlowVMobj = Mapper.MapToDto<Workflow, WorkFlowVM>(wf);

                // 3. Fetch the workflow steps
                var stepEntities = await _context.WorkflowSteps
                    .Where(x => x.WorkflowId == workflowid)
                    .ToListAsync();

                var WFSslst = Mapper.MapToDtos<WorkflowStep, WorkFlowStepVM>(stepEntities);

                List<WorkFlowStepVM> wfm = new List<WorkFlowStepVM>();
                wfm = WFSslst.ToList();


                // Reassign the modified list back to the main object
                WorkFlowVMobj.workFlowStepVMs = wfm;

                // 5. Calculate progress
                int totalSteps = wfm.Count();


                int completedSteps = wfm
                    .Count(x => x.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase));

                int progressPercentage = (totalSteps > 0)
                    ? (completedSteps * 100) / totalSteps
                    : 0;

                WorkFlowVMobj.Progress = progressPercentage.ToString();

                // 6. Populate additional details
                var workflowType = await _context.WorkflowTypes.FindAsync(WorkFlowVMobj.WorkflowTypeId);
                if (workflowType != null)
                {
                    WorkFlowVMobj.WorkflowTypeName = workflowType.Name;
                }

                var initiator = await _context.Users.FindAsync(WorkFlowVMobj.InitiatorId);
                if (initiator != null)
                {
                    WorkFlowVMobj.InitiatorName = initiator.Username;
                    WorkFlowVMobj.Department = initiator.Department;
                }


                var stepslist = wfm.Select(x => x.Id).ToList();

                var documents = await _context.WorkflowDocuments
                    .Where(x => stepslist.Contains(Convert.ToInt32(x.WorkflowId)))
                    .ToListAsync();




                WorkFlowVMobj.Documents = documents;

                // 10. Return the completed workflow object with steps + documents
                int interactionid = Convert.ToInt32(WorkFlowVMobj.InteractionId);

                if (wf.Subject == "Interaction Recording Form")
                {
                    var tasklst = await _context.Interactions.Where(x => x.Id == interactionid).ToListAsync();

                    WorkFlowVMobj.InterationData = tasklst;
                }
                else if (wf.Subject == "CONTRACTOR REGISTRATION")
                {
                    var tasklst = await _context.Contractors.Where(x => x.Id == interactionid).ToListAsync();
                    WorkFlowVMobj.Details = wf.Details;
                    WorkFlowVMobj.InterationData = tasklst;
                }
                else if (wf.Subject == "Resale NOC")
                {
                    var tasklst = await _context.Resalenocs.Where(x => x.Id == interactionid).Select(x => new ResaleNOCVM()
                    {
                        contactNumber = x.Contactno,
                        customerName = x.Customername,
                        email = x.Email,
                        mastercomm = x.Mastercomm,
                        projectName = x.Projectname,
                        unitNumber = x.Unitno,
                        Intiatorname = x.Intiatorname

                    }).ToListAsync();

                    WorkFlowVMobj.InterationData = tasklst;
                }



                if (wf.Subject == "Interaction Recording Form")
                {
                    WorkFlowVMobj.Identifier = GenerateFormattedId(interactionid);
                }
                else if (wf.Subject == "CONTRACTOR REGISTRATION")
                {
                    WorkFlowVMobj.Identifier = GenerateCCFormattedId(interactionid);
                }
                else if (wf.Subject == "Resale NOC")
                {
                    WorkFlowVMobj.Identifier = GenerateRNFormattedId(interactionid);
                }

                return WorkFlowVMobj;
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return null;
            }
        }



        [HttpGet("GetUsers")]
        [AllowAnonymous]
        public async Task<List<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }



        [HttpGet("SendPaymentConfirmationEmail")]
        public async System.Threading.Tasks.Task SendPaymentConfirmationEmail(string email, string comment, string? pdfFilePath = null)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = "Work Permit Approval";

                // Email Body with Updated Black & White Theme and Logo
                string emailBody = $@"
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
                .highlight {{
                    font-weight: bold;
                    color: #000000;
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
                    <p>Dear Customer,</p>
                    <p>{comment}</p>
                    <p>If you require further assistance, feel free to contact us at <a style='color:black' href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
                </div>
                <div class='footer'>
                    <p>Best Regards,</p>
                    <p><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
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

                // Add recipient
                mailMessage.To.Add(email);

                // Attach the PDF file if provided
                if (!string.IsNullOrEmpty(pdfFilePath) && System.IO.File.Exists(pdfFilePath))
                {
                    var pdfAttachment = new Attachment(pdfFilePath, "application/pdf");
                    mailMessage.Attachments.Add(pdfAttachment);
                }

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }


        [HttpGet("resalenocother")]
        public async System.Threading.Tasks.Task resalenocother(string email, string salesOpsStaffName, string unitCode, string other, IFormFileCollection? files, string personname)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = "NOC Inspection Rejection Notice";

                // Email Body
                string emailBody = $@"
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
        .content {{
            padding: 20px;
            font-size: 16px;
            line-height: 1.5;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 14px;
            border-top: 1px solid #000000;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>NOC Inspection Rejection</h2>
        </div>
        <div class='content'>
            <p>Dear {salesOpsStaffName},</p>
            <p>Following our review of the property <strong>{unitCode}</strong> as requested for NOC inspection, we regret to inform you that we cannot proceed with this request at this time due to the following non-compliance issue(s):</p>
            <p><strong>{other}</strong></p>
            {(files != null && files.Count > 0 ? "<p>For your reference, please find attached the relevant picture(s) highlighting the identified issue(s).</p>" : "")}
            <p>We request you to inform the unit owner of the non-compliance and advise them to address the matter. Should you or the customer require any clarification or assistance, please feel free to reach out to us.</p>
        </div>
        <div class='footer'>
            <p>Best Regards,</p>
            <p><strong>{personname}</strong></p>
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

                // Add recipient
                mailMessage.To.Add(email);

                // Attach files if available
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Get the upload path from appsettings.json
                            var uploadPath = _configuration.GetValue<string>("upload:path");
                            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);

                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }

                            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(fullPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            mailMessage.Attachments.Add(new Attachment(filePath, file.ContentType));
                        }
                    }
                }

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }



        [HttpGet("rejectresalenoc")]
        public async System.Threading.Tasks.Task rejectresalenoc(string email, string salesOpsStaffName, string unitCode, List<string> nonComplianceIssues, IFormFileCollection? files, string personname)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = "NOC Inspection Rejection Notice";

                // Convert nonComplianceIssues list to bullet points
                string nonComplianceIssuesHtml = "<ul>" + string.Join("", nonComplianceIssues.Select(issue => $"<li>{issue}</li>")) + "</ul>";

                // Email Body
                string emailBody = $@"
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
        .content {{
            padding: 20px;
            font-size: 16px;
            line-height: 1.5;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 14px;
            border-top: 1px solid #000000;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>NOC Inspection Rejection</h2>
        </div>
        <div class='content'>
            <p>Dear {salesOpsStaffName},</p>
            <p>Following our review of the property <strong>{unitCode}</strong> as requested for NOC inspection, we regret to inform you that we cannot proceed with this request at this time due to the following non-compliance issue(s):</p>
            {nonComplianceIssuesHtml}
            {(files != null && files.Count > 0 ? "<p>For your reference, please find attached the relevant picture(s) highlighting the identified issue(s).</p>" : "")}
            <p>We request you to inform the unit owner of the non-compliance and advise them to address the matter. Should you or the customer require any clarification or assistance, please feel free to reach out to us.</p>
        </div>
        <div class='footer'>
            <p>Best Regards,</p>
            <p><strong>{personname}</strong></p>
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

                // Add recipient
                mailMessage.To.Add(email);

                // Attach files if available
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Get the upload path from appsettings.json
                            var uploadPath = _configuration.GetValue<string>("upload:path");
                            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);

                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }

                            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(fullPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            mailMessage.Attachments.Add(new Attachment(filePath, file.ContentType));
                        }
                    }
                }

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        [HttpGet("approveresalenoc")]
        public async System.Threading.Tasks.Task approveresalenoc(string email, string salesOpsStaffName, string unitCode, List<string> nonComplianceIssues, IFormFileCollection? files, string personname)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = "NOC Inspection Approval";

                // Convert nonComplianceIssues list to bullet points
                string nonComplianceIssuesHtml = "<ul>" + string.Join("", nonComplianceIssues.Select(issue => $"<li>{issue}</li>")) + "</ul>";

                // Email Body
                string emailBody = $@"
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
        .content {{
            padding: 20px;
            font-size: 16px;
            line-height: 1.5;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 14px;
            border-top: 1px solid #000000;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>NOC Inspection Approval</h2>
        </div>
        <div class='content'>
            <p>Dear {salesOpsStaffName},</p>
            <p>We are pleased to inform you that the NOC inspection for the property <strong>{unitCode}</strong> has been successfully completed and approved.</p>
            <p>All necessary compliance checks have been met, and the request is now ready for further processing.</p>          
            <p>If you require any further details or assistance, please do not hesitate to contact us.</p>
        </div>
        <div class='footer'>
            <p>Best Regards,</p>
            <p><strong>{personname}</strong></p>
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

                // Add recipient
                mailMessage.To.Add(email);



                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }




        [HttpGet("SendInvoiceAndReceiptEmail")]
        public async System.Threading.Tasks.Task SendInvoiceAndReceiptEmail(string email, IFormFileCollection? files)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = "Invoice and Payment Receipt - Al Hamra";

                // Email Body with Updated Black & White Theme and Logo
                string emailBody = $@"
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
                .highlight {{
                    font-weight: bold;
                    color: #000000;
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
                    <p>Dear Customer,</p>
                    <p>We hope this email finds you well. Please find attached the <strong>Invoice and Payment Receipt</strong> for your recent transaction with Al Hamra Property Management.</p>
                    <p>If you have any questions or require further assistance, please feel free to contact us at 
                    <a style='color:black' href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
                    <p>Thank you for your continued trust in Al Hamra.</p>
                </div>
                <div class='footer'>
                    <p>Best Regards,</p>
                    <p><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
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

                // Add recipient
                mailMessage.To.Add(email);

                // Attach files if available
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Get the upload path from appsettings.json
                            var uploadPath = _configuration.GetValue<string>("upload:path");

                            // Combine with the current directory to get the full path
                            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);

                            // Ensure the directory exists
                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }

                            // Generate a unique file name to avoid overwriting
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                            var originalFileName = Path.GetFileName(file.FileName);
                            var filePath = Path.Combine(fullPath, originalFileName);

                            // Save the file to the specified path
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Determine the MIME type of the file
                            var mimeType = file.ContentType; // Use the MIME type from the uploaded file

                            // Attach the file to the email with a valid MIME type
                            mailMessage.Attachments.Add(new Attachment(filePath, mimeType));
                        }
                    }
                }

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }


        [HttpGet("SendPaymentEmailToCustomer")]
        public async System.Threading.Tasks.Task SendPaymentEmailToCustomer(string customerName, string modificationFee, string customerEmail)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = "Payment Request for Modification";

                // Email Body with Black & White Theme and Logo
                string emailBody = $@"
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
                .highlight {{
                    font-weight: bold;
                    color: #000000;
                }}
                .note {{
                    background-color: #eeeeee;
                    padding: 10px;
                    border-left: 4px solid #000000;
                    margin-top: 10px;
                    font-size: 14px;
                }}
                .footer {{
                    text-align: center;
                    padding: 10px;
                    font-size: 14px;
                    color: #000000;
                    border-top: 1px solid #000000;
                    margin-top: 20px;
                }}
                ul {{
                    list-style-type: none;
                    padding: 0;
                }}
                li {{
                    background: #f2f2f2;
                    padding: 10px;
                    margin: 5px 0;
                    border-radius: 5px;
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
                    <p>Dear <span class='highlight'>{customerName}</span>,</p>
                    <p>We are pleased to inform you that your modification request is being reviewed and finalized for approval. Below, you will find the payment details for the associated fees:</p>
                    <ul>
                        <li><strong>Modification Fee: AED {modificationFee}/-</strong></li>
                    </ul>
                    <p>Kindly make the payment at the <strong>Al Hamra Community Office</strong>.</p>

                    <div class='note'>
                        <p><strong>Important Notice:</strong></p>
                        <p>All payments towards the unit (utilities, community fees, etc.) must be cleared and up to date at all times, in order to process approvals and work permits.</p>
                    </div>

                    <div class='note'>
                        <p><strong>Attention:</strong></p>
                        <p>No changes to <strong>fire, safety, or HVAC systems</strong> are allowed without approval from AMC contractors. Any damage caused by work done without approval or by unapproved contractors will be penalized.</p>
                        <p>Notify neighbors in advance if noisy work is expected.</p>
                    </div>

                    <p>For any inquiries, please feel free to contact us at <a style='color:black' href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
                </div>
                <div class='footer'>
                    <p>Best Regards,</p>
                    <p><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
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

                // Add recipient
                mailMessage.To.Add(customerEmail);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }



        [HttpGet("SendPaymentNotificationToContractor")]
        public async System.Threading.Tasks.Task SendPaymentNotificationToContractor(string customerName, string amountPaid, string requestno, string customerEmail)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = $"Your Request {requestno} Has Been Processed – Proceed to Cashier";

                // Email Body
                string emailBody = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; color: #000000; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: auto; background: #ffffff; padding: 20px; border-radius: 8px; border: 1px solid #000000; }}
        .header {{ text-align: center; padding-bottom: 10px; border-bottom: 1px solid #000000; }}
        .content {{ padding: 20px; font-size: 16px; line-height: 1.5; }}
        .footer {{ text-align: center; padding: 10px; font-size: 14px; border-top: 1px solid #000000; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
         <div class='header'>
                <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Al Hamra Logo'>
            </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>We are pleased to inform you that your request no: <strong>{requestno}</strong> has been successfully processed.</p>
            <p>You may now proceed to the cashier located at Royal Breeze 4 to complete the next step.</p>
            <p>To assist you in reaching the correct location, please use the following link:</p>
            <p><a href='https://maps.google.com?q=Royal+Breeze+4' style='color: black;'>Royal Breeze 4 Location</a></p>
        </div>
        <div class='footer'>
            <p>Best Regards,</p>
            <p><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
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

                // Add recipient
                mailMessage.To.Add(customerEmail);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }





        [HttpGet("SendPaymentReceiptToCustomer")]
        public async System.Threading.Tasks.Task SendPaymentReceiptToContractor(string customerName, string amountPaid, string paymentDate, string workflowId, string customerEmail, string paidBy, string paymentMethod)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = $"Payment Confirmation reference no {workflowId} – Validity & Next Steps";

                // Email Body
                string emailBody = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; color: #000000; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: auto; background: #ffffff; padding: 20px; border-radius: 8px; border: 1px solid #000000; }}
        .header {{ text-align: center; padding-bottom: 10px; border-bottom: 1px solid #000000; }}
        .content {{ padding: 20px; font-size: 16px; line-height: 1.5; }}
        .footer {{ text-align: center; padding: 10px; font-size: 14px; border-top: 1px solid #000000; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
         <div class='header'>
                    <img src='{_configuration["AppBaseURL:EmailLogo"]}' alt='Al Hamra Logo'>
                </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>We are pleased to confirm that we have received your payment with respect to reference no <strong>{workflowId}</strong>. Kindly find enclosed payment receipt and below payment details for your reference:</p>
            <p><strong>Payment Details:</strong></p>
            <table>
                <tr>
                    <th>Amount Paid:</th>
                    <td>AED {amountPaid}/-</td>
                </tr>
                <tr>
                    <th>Date of Payment:</th>
                    <td>{paymentDate}</td>
                </tr>
                <tr>
                    <th>Paid By:</th>
                    <td>{paidBy}</td>
                </tr>
                <tr>
                    <th>Payment Method:</th>
                    <td>{paymentMethod}</td>
                </tr>
            </table>
            <p>This payment is valid for one year, provided that all dues for the year are settled.</p>
            <p>Should you have any questions or require further assistance, please feel free to reach out to us at 8002542672 or alternatively at <a href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
        </div>
        <div class='footer'>
            <p>Best Regards,</p>
            <p><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
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

                // Add recipient
                mailMessage.To.Add(customerEmail);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }


        [HttpGet("SendPaymentEmailToContractor")]
        public async System.Threading.Tasks.Task SendPaymentEmailToContractor(string contractorname, string regfee, string contractoremail)
        {
            try
            {
                // Initialize SMTP client with configuration
                var smtpClient = new SmtpClient(_configuration["Mail:Host"])
                {
                    Port = int.Parse(_configuration["Mail:Port"]),
                    Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                    EnableSsl = true,
                };

                // Email Subject
                string emailSubject = "Payment Request for Contractor Registration";

                // Email Body with Black & White Theme and Logo
                string emailBody = $@"
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
                .highlight {{
                    font-weight: bold;
                    color: #000000;
                }}
                .note {{
                    background-color: #eeeeee;
                    padding: 10px;
                    border-left: 4px solid #000000;
                    margin-top: 10px;
                    font-size: 14px;
                }}
                .footer {{
                    text-align: center;
                    padding: 10px;
                    font-size: 14px;
                    color: #000000;
                    border-top: 1px solid #000000;
                    margin-top: 20px;
                }}
                ul {{
                    list-style-type: none;
                    padding: 0;
                }}
                li {{
                    background: #f2f2f2;
                    padding: 10px;
                    margin: 5px 0;
                    border-radius: 5px;
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
                    <p>Dear <span class='highlight'>{contractorname}</span>,</p>
                    <p>We are pleased to inform you that your modification request is being reviewed and finalized for approval. Below, you will find the payment details for the associated fees:</p>
                    <ul>
                        <li><strong> Fee: AED {regfee}/-</strong></li>
                    </ul>
                    <p>Kindly make the payment at the <strong>Al Hamra Community Office</strong>.</p>

                 

                    <p>For any inquiries, please feel free to contact us at <a style='color:black' href='mailto:propertymanagement@alhamra.ae'>propertymanagement@alhamra.ae</a>.</p>
                </div>
                <div class='footer'>
                    <p>Best Regards,</p>
                    <p><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
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

                // Add recipient
                mailMessage.To.Add(contractoremail);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }


        [HttpGet("SendModificationEmail")]
        private async System.Threading.Tasks.Task SendModificationEmail(User user)
        {
            var smtpClient = new SmtpClient(_configuration["Mail:Host"])
            {
                Port = int.Parse(_configuration["Mail:Port"]),
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                EnableSsl = true,
            };

            string emailSubject = "New Task Assigned";

            string emailBody = $@"
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
                width: 200px;
                text-align: center;
                background-color: #000000;
                color: #ffffff;
                padding: 10px;
                border-radius: 5px;
                text-decoration: none;
                font-size: 16px;
                margin: 20px auto;
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
                <p>Dear {user.Username},</p>
                <p>You have been assigned a new task.</p>
                <p>Please click the button below to view the task details:</p>
                <a class='button' href='{_configuration["AppBaseURL:URL"]}/signin'>View Task</a>
                <p>If you have any questions or need further assistance, please feel free to contact us.</p>
            </div>
            <div class='footer'>
                <p>Best regards,</p>
                <p><strong>PROPERTY MANAGEMENT</strong><br>AL HAMRA</p>
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

            mailMessage.To.Add(user.Email);

            await smtpClient.SendMailAsync(mailMessage);
        }






        [AllowAnonymous]
        private string ExtractDateTimeString(string input)
        {
            return Regex.Match(input, @"^[A-Za-z]{3} [A-Za-z]{3} \d{2} \d{4}").Value;
        }


        [HttpGet("GetNullableDateTimeValue")]
        [AllowAnonymous]
        public DateTime? GetNullableDateTimeValue(string dateString)
        {


            if (string.IsNullOrWhiteSpace(dateString)) return null;

            // Extract the datetime part (removing extra time zone name in parentheses)
            string cleanedDate = ExtractDateTimeString(dateString);

            if (DateTimeOffset.TryParseExact(cleanedDate, "ddd MMM dd yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate.Date;
            }

            return null;
        }


        [AllowAnonymous]
        private async Task<StepAction> ConvertformDataToStepAction(IFormCollection formData)
        {
            StepAction stepAction = new StepAction();
            try
            {
                string GetStringValue(string key) => formData.ContainsKey(key) ? formData[key].ToString() : null;

                int? GetNullableIntValue(string key) => int.TryParse(GetStringValue(key), out int value) ? value : null;

                decimal? GetNullableDecimalValue(string key) => decimal.TryParse(GetStringValue(key), out decimal value) ? value : null;




                stepAction.StepId = Convert.ToInt32(GetStringValue("stepId"));
                stepAction.ActionType = GetStringValue("actionType");
                stepAction.PerformedBy = GetNullableIntValue("PerformedBy");
                stepAction.PerformedOn = GetNullableDateTimeValue(GetStringValue("PerformedOn"));
                stepAction.Comments = GetStringValue("Comments");
                stepAction.Category = GetStringValue("Category");
                stepAction.SubCat = GetStringValue("SubCat");
                stepAction.ModificationFee = GetNullableDecimalValue("ModificationFee");
                stepAction.UnlistedContractorFee = GetNullableDecimalValue("UnlistedContractorFee");
                stepAction.BuiltupExtensionFee = GetNullableDecimalValue("BuiltupExtensionFee");
                stepAction.Total = GetNullableDecimalValue("Total");
                stepAction.InvNuumber = GetStringValue("InvNuumber");
                stepAction.Name = GetStringValue("Name");
                stepAction.Email = GetStringValue("Email");
                stepAction.ContactNumber = GetStringValue("ContactNumber");
                stepAction.AssignTo = GetStringValue("AssignTo") != null ? Convert.ToInt32(GetStringValue("AssignTo")) : 0;
                stepAction.SubCategory = GetStringValue("SubCategory");
                stepAction.ModificationRequest = GetStringValue("ModificationRequest");
                stepAction.PaidBy = GetStringValue("PaidBy");
                stepAction.ApprovalStartDate = GetNullableDateTimeValue(GetStringValue("ApprovalStartDate"));
                stepAction.ApprovalEndDate = GetNullableDateTimeValue(GetStringValue("ApprovalEndDate"));
                stepAction.PrevStepId = GetStringValue("PrevStepId");

            }
            catch (Exception ex)
            {
                return null;
            }

            return stepAction;
        }

    }
    public static class Mapper
    {
        public static TDto MapToDto<TEntity, TDto>(TEntity entity)
        {
            // Ensure you have a mapping from TEntity to TDto
            if (entity == null) return default;

            var dto = Activator.CreateInstance<TDto>();

            // Get properties of TDto and TEntity
            var dtoProperties = typeof(TDto).GetProperties();
            var entityProperties = typeof(TEntity).GetProperties();

            // Use reflection to match properties case-insensitively
            foreach (var dtoProperty in dtoProperties)
            {
                // Find the corresponding entity property with case-insensitive matching
                var entityProperty = entityProperties
                    .FirstOrDefault(ep => string.Equals(ep.Name, dtoProperty.Name, StringComparison.OrdinalIgnoreCase));

                if (entityProperty != null && dtoProperty.CanWrite)
                {
                    var value = entityProperty.GetValue(entity);
                    dtoProperty.SetValue(dto, value);
                }
            }

            return dto;
        }
        public static IEnumerable<TDto> MapToDtos<TEntity, TDto>(this IEnumerable<TEntity> entities)
       where TDto : new()
        {
            if (entities == null)
                return Enumerable.Empty<TDto>();

            return entities.Select(entity => MapToDto<TEntity, TDto>(entity));
        }
    }
}

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
        public async Task<bool> CreateWorkFlow(int initiator_id, string workflowname, string jsonInfo, int interactionid)
        {
            if (workflowname == "INTERACTION RECORDING FORM")
            {
                return await CreateInteractionWorkflow(initiator_id, workflowname, jsonInfo, interactionid);
            }
            return true;
        }

        private string GenerateFormattedId(int id)
        {
            string prefix = "PMMR";
            return $"{prefix}{id:D5}";
        }


        private async Task<bool> CreateInteractionWorkflow(int initiator_id, string workflowname, string jsonInfo, int interactionid)
        {
            Workflow workflow = new Workflow();
            try
            {

                var userid1 = await _context.Users.Where(x => x.Email == "bejoy.george@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                var userid2 = await _context.Users.Where(x => x.Email == "jinu.joy@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                workflow.InitiatorId = initiator_id;
                var workflowtypeid = await _context.WorkflowTypes.Where(x => x.Name == "INTERACTION RECORDING FORM").Select(x => x.Id).FirstOrDefaultAsync();
                workflow.WorkflowTypeId = workflowtypeid;
                workflow.Status = "In Progress";
                workflow.Subject = "INTERACTION RECORDING FORM";
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
                var data = new[]
{
    new { Id = userid1.ToString(), Status = "Not Approved", Rights = "Edit" },
    new { Id = userid2.ToString(), Status = "Not Approved", Rights = "Edit" },
};
                string jsonStringUsers = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                step.AssignedTo = jsonStringUsers;
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
                userid1 = await _context.Users.Where(x => x.Email == "abubaker.yafai@email.com").Select(x => x.Id).FirstOrDefaultAsync();
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
                userid1 = await _context.Users.Where(x => x.Email == "suhail.abdullah@email.com").Select(x => x.Id).FirstOrDefaultAsync();
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
                userid1 = await _context.Users.Where(x => x.Email == "dilin.varkey@email.com").Select(x => x.Id).FirstOrDefaultAsync();
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
                userid1 = await _context.Users.Where(x => x.Email == "cashir.varkey@email.com").Select(x => x.Id).FirstOrDefaultAsync();
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
                userid1 = await _context.Users.Where(x => x.Email == "bejoy.george@email.com").Select(x => x.Id).FirstOrDefaultAsync();
                userid2 = await _context.Users.Where(x => x.Email == "jinu.joy@email.com").Select(x => x.Id).FirstOrDefaultAsync();
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

                await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid1, "Modification Request", initiator_id);
                await CreateTask(workflow.Id, step.Id.ToString(), "Workflow", userid2, "Modification Request", initiator_id);

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

            // Create a memory stream for the ZIP archive
            using (var memoryStream = new MemoryStream())
            {
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

                    // Return the ZIP file as a downloadable file
                    return File(memoryStream, "application/zip", zipFileName);
                }
                catch (Exception ex)
                {
                    // Log the exception as needed
                    // Example: _logger.LogError(ex, "Error while creating ZIP archive");
                    return StatusCode(500, $"An error occurred while creating the ZIP archive: {ex.Message}");
                }
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
        public async Task<IActionResult> DownloadInteractionFile(int interactionid, string docname)
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
            else if (docname == "currentProposedLayout")
            {
                docppath = interactions.CurrentProposedLayout;
            }

            // 2. Check if the document exists
            if (docppath == null)
            {
                return NotFound("Document not found.");
            }

            // 3. Verify the file path
            var filePath = docppath;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File does not exist on the server.");
            }

            // 4. Ensure the file is a PDF
            if (!filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are supported.");
            }

            // 5. Set the content type for PDF
            var contentType = "application/pdf";

            // 6. Return the file as an attachment (download)
            return PhysicalFile(filePath, contentType, docname);
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

                if (stepAction.ActionType == "RFI")
                {
                    int stepid = Convert.ToInt32(stepAction.PrevStepId);

                    var stepaction = await _context.StepActions.Where(x => x.Id == stepid).FirstOrDefaultAsync();
                    // Retrieve the previous workflow step
                    var prevstep = await _context.WorkflowSteps
                        .Where(x => x.Id == stepaction.StepId)
                        .FirstOrDefaultAsync();

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
                            }
                        }
                    }


                    // Serialize the updated list back to JSON and store it
                    prevstep.Details = JsonConvert.SerializeObject(detailsList);

                    // Save changes
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

                        // Create a combined JSON object with stepAction and pathData
                        var combinedData = new
                        {
                            StepAction = stepAction,
                            Files = pathData, // Embed pathData (list of files)

                        };

                        // Serialize the combined object to JSON
                        string jsonString = JsonSerializer.Serialize(combinedData, new JsonSerializerOptions { WriteIndented = true });
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
                                    item["Status"] = "Approved"; // Modify the property
                                    item["Rights"] = "Edit";    // Modify another property
                                }
                                if (item["Rights"].ToString() == "Edit" && item["Status"].ToString() != "Approved")
                                {
                                    all = false;
                                }
                            }
                            if (workflowstep.Type == "Any")
                            {
                                workflowstep.Status = "Approved";
                            }
                            else if (all == true)
                            {
                                workflowstep.Status = "Approved";
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
                        if (workflowstep.Status == "Approved" && stepAction.ActionType == "Submit")
                        {
                            var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                            string nextstepname = string.Empty;
                            if (workflowstep.StepName == "Review Scope and Site Requirements") nextstepname = "Review Scope";
                            else if (workflowstep.StepName == "Review Scope") nextstepname = "Review Scope2";
                            else if (workflowstep.StepName == "Review Scope2") nextstepname = "Review Scope and Fees Calculation";
                            else if (workflowstep.StepName == "Review Scope and Fees Calculation") nextstepname = "Upload the Invoice";
                            else if (workflowstep.StepName == "Upload the Invoice") nextstepname = "Confirm Payment Received";

                            if (workflowstep.StepName == "Review Scope and Site Requirements" && (stepAction.Category == "Minor Work"
                                && stepAction.SubCat == "Without Charges") || (stepAction.Category == "Major Work"))
                            {
                                //  workflows.Status = "Approved";
                                //workflowstep.Status = "In Progress";

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
                                        await CreateTask(nextstepflow.WorkflowId.Value, nextstepflow.Id.ToString(), "Workflow", Convert.ToInt32(item["Id"].ToString()), "Modification Request", workflows.InitiatorId.Value);
                                    }
                                }

                                nextstepflow.Status = "In Progress";
                                nextstepflow.ReceivedOn = DateTime.Now;
                                nextstepflow.DueOn = DateTime.Now.AddDays(2);
                                await _context.SaveChangesAsync();
                            }

                            if (workflowstep.StepName == "Review Scope and Site Requirements")
                            {
                                workflows.ApprovalStartDate = stepAction.ApprovalStartDate;
                                workflows.ApprovalEndDate = stepAction.ApprovalEndDate;
                                await _context.SaveChangesAsync();
                            }

                            if (workflowstep.StepName == "Review Scope and Fees Calculation")
                            {
                                workflows.Amount = stepAction.Total;
                                await _context.SaveChangesAsync();
                            }

                            if (workflowstep.StepName == "Upload the Invoice")
                            {
                                workflows.ReceiptDate = DateTime.Now;
                                workflows.ReceiptBy = "6";
                                workflows.ReceiptNo = GenerateFormattedId(Convert.ToInt32(workflows.InteractionId));
                                workflows.Amount = stepAction.Total;
                                workflows.PaidBy = stepAction.PaidBy;
                                workflows.VendorName = stepAction.VendorName;
                                await _context.SaveChangesAsync();
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
                                string pdfFileName = "workpermit.pdf";
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
        public async Task<bool> CreateWorkFlowStepActionRFI(int WorkflowStepId, StepAction stepAction)
        {
            stepAction.StepId = WorkflowStepId;
            stepAction.PerformedOn = DateTime.Now;
            await _context.AddAsync(stepAction);
            await _context.SaveChangesAsync();

            // Find the workflow step
            var workflowstep = await _context.WorkflowSteps.FindAsync(WorkflowStepId);
            if (workflowstep != null)
            {
                // Serialize the combined object to JSON
                // string jsonString = JsonSerializer.Serialize(combinedData, new JsonSerializerOptions { WriteIndented = true });
                var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.AssignedTo);
                List<dynamic>? deserializedData2 = new List<dynamic>();
                if (workflowstep.Details != null)
                    deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.Details);
                deserializedData.Add(new { StepId = stepAction.Id, Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "RFI" });
                deserializedData2.Add(new { StepId = stepAction.Id, Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "RFI", Comment = stepAction.Comments, RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "RFI" });
                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                workflowstep.AssignedTo = jsonString;
                workflowstep.Details = jsonString2;
                // workflowstep.ExecutedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                await CreateTask(workflowstep.WorkflowId.Value, WorkflowStepId.ToString(), "RFI", stepAction.AssignTo.Value, "Modification Request", workflows.InitiatorId.Value);

                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost("CreateWorkFlowStepActionReassign")]
        public async Task<bool> CreateWorkFlowStepActionReassign(int WorkflowStepId, StepAction stepAction)
        {
            stepAction.StepId = WorkflowStepId;
            stepAction.PerformedOn = DateTime.Now;
            await _context.AddAsync(stepAction);
            await _context.SaveChangesAsync();

            // Find the workflow step
            var workflowstep = await _context.WorkflowSteps.FindAsync(WorkflowStepId);
            if (workflowstep != null)
            {
                // Serialize the combined object to JSON
                // string jsonString = JsonSerializer.Serialize(combinedData, new JsonSerializerOptions { WriteIndented = true });
                var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(workflowstep.AssignedTo);
                List<dynamic>? deserializedData2 = new List<dynamic>();
                if (workflowstep.Details != null)
                    deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.Details);
                foreach (var item in deserializedData)
                {
                    if (item["Rights"].ToString() == "Edit" && item["Id"].ToString() == stepAction.PerformedBy.ToString())
                    {
                        item["Status"] = "Not Approved";
                        item["Rights"] = "View";
                    }
                }

                // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.AssignedTo);

                var data = new Dictionary<string, object> {
                    { "Id" , stepAction.AssignTo.ToString() },{ "Status" , "Not Approved" },{ "Rights" , "Edit" } , { "IterationType" , "Reassigned" }, {"PerformedOn" , DateTime.Now }, { "RequestedBy" , "" } , {"RequestedTo" , ""}
                };

                deserializedData.Add(data);
                deserializedData2.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "Edit", Comment = stepAction.Comments, RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "Reassigned" });

                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                workflowstep.Details = jsonString2;
                workflowstep.AssignedTo = jsonString;
                // workflowstep.ExecutedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                var workflows = await _context.Workflows.FindAsync(workflowstep.WorkflowId);
                await CreateTask(workflowstep.WorkflowId.Value, WorkflowStepId.ToString(), "Reassigned", stepAction.AssignTo.Value, "Modification Request", workflows.InitiatorId.Value);

                return true;
            }
            else
            {
                return false;
            }
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
                // Serialize the combined object to JSON
                // string jsonString = JsonSerializer.Serialize(combinedData, new JsonSerializerOptions { WriteIndented = true });
                //var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.AssignedTo);
                //deserializedData.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "RFI" });
                //string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                //workflowstep.Details = jsonString;
                //// workflowstep.ExecutedOn = DateTime.Now;
                //await _context.SaveChangesAsync();
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
                        await CreateTask(workflowstep.WorkflowId.Value, prevstepflow.Id.ToString(), "Return Step", Convert.ToInt32(item["Id"].ToString()), "Modification Request", workflows.InitiatorId.Value);
                        prevstepflow.Status = "In Progress";
                        prevstepflow.AssignedTo = p_currentstepjson;
                        await _context.SaveChangesAsync();
                        item["Status"] = "Not Approved";
                        item["IterationType"] = "Return Step";
                        item["PerformedOn"] = DateTime.Now;
                    }
                }
                List<dynamic>? deserializedData2 = new List<dynamic>();
                if (prevstepflow.Details != null)
                    deserializedData2 = JsonSerializer.Deserialize<List<dynamic>>(prevstepflow.Details);
                deserializedData2.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "Edit", Comment = "", RequestedBy = stepAction.PerformedBy.ToString(), PerformedOn = DateTime.Now, IterationType = "Return Step" });

                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                string jsonString2 = JsonSerializer.Serialize(deserializedData2, new JsonSerializerOptions { WriteIndented = true });
                workflowstep.AssignedTo = u_currentstepjson;
                //workflowstep.Details = jsonString2;
                // workflowstep.ExecutedOn = DateTime.Now;
                workflowstep.Status = "Not Started";
                await _context.SaveChangesAsync();

                //await CreateTask(workflowstep.WorkflowId.Value, WorkflowStepId.ToString(), "RFI", stepAction.AssignTo.Value, "Modification Request", workflows.InitiatorId.Value);

                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<bool> CreateTask(int WorkflowId, string WorkflowStepId, string type, int assignto, string template, int initiatorid)
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
            return true;
        }
        [HttpPost("GetTaks")]
        [AllowAnonymous]
        public async Task<IEnumerable<TaksVM>> GetTaks(int userid)
        {
            try
            {

                var query = from ut in _context.UserTasks
                            join wf in _context.Workflows on ut.WorkflowId equals wf.Id
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
                    Ageing = CalculateAgeing(item.DueDate),
                    Status = item.Status,
                    AssignedTo = item.AssignedTo,
                    Isviewed = item.IsViewed,
                    Unique_Id = item.InteractionId != null ? GenerateFormattedId(Convert.ToInt32(item.InteractionId)) : ""
                }).OrderByDescending(x => x.Id).ToList();

                return tasksList;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return new List<TaksVM>();
            }
        }

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

                // 4. Attach these step VMs to the workflow VM
                WorkFlowVMobj.workFlowStepVMs = WFSslst;

                // 5. Calculate progress
                int totalSteps = WFSslst.Count();
                int completedSteps = WFSslst
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


                var stepslist = WFSslst.Select(x => x.Id).ToList();

                var documents = await _context.WorkflowDocuments
                    .Where(x => stepslist.Contains(Convert.ToInt32(x.WorkflowId)))
                    .ToListAsync();




                WorkFlowVMobj.Documents = documents;

                // 10. Return the completed workflow object with steps + documents
                int interactionid = Convert.ToInt32(WorkFlowVMobj.InteractionId);
                var tasklst = await _context.Interactions.Where(x => x.Id == interactionid).ToListAsync();

                WorkFlowVMobj.InterationData = tasklst;


                return WorkFlowVMobj;
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return null;
            }
        }



        [HttpGet("GetUsers")]
        public async Task<List<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        private async System.Threading.Tasks.Task SendModificationEmail(User user)
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
                Body = $"Dear Customer,\n\nFill out the detail on the link below http://example.com/api/forms/SubmitExternalForm/{user.Id}\n\nRegards,\nAlHamra Team",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(user.Email);

            await smtpClient.SendMailAsync(mailMessage);
        }
        private async System.Threading.Tasks.Task SendModificationEmail2(string email)
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
                Body = $"Dear Customer,\n\nFill out the detail on the link below \n\nRegards,\nAlHamra Team",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
        private async Task<StepAction> ConvertformDataToStepAction(IFormCollection formData)
        {
            StepAction stepAction = new StepAction();
            try
            {
                string GetStringValue(string key) => formData.ContainsKey(key) ? formData[key].ToString() : null;

                int? GetNullableIntValue(string key) => int.TryParse(GetStringValue(key), out int value) ? value : null;

                decimal? GetNullableDecimalValue(string key) => decimal.TryParse(GetStringValue(key), out decimal value) ? value : null;

                DateTime? GetNullableDateTimeValue(string key) => DateTime.TryParse(GetStringValue(key), out DateTime value) ? value : null;

                stepAction.StepId = Convert.ToInt32(GetStringValue("stepId"));
                stepAction.ActionType = GetStringValue("actionType");
                stepAction.PerformedBy = GetNullableIntValue("PerformedBy");
                stepAction.PerformedOn = GetNullableDateTimeValue("PerformedOn");
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
                stepAction.ApprovalStartDate = GetNullableDateTimeValue("ApprovalStartDate");
                stepAction.ApprovalEndDate = GetNullableDateTimeValue("ApprovalEndDate");
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

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
        public async Task<bool> CreateWorkFlow(int initiator_id, string workflowname, string jsonInfo)
        {
            if (workflowname == "INTERACTION RECORDING FORM")
            {
                return await CreateInteractionWorkflow(initiator_id, workflowname, jsonInfo);
            }
            return true;
        }
        private async Task<bool> CreateInteractionWorkflow(int initiator_id, string workflowname, string jsonInfo)
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
                step2.ReceivedOn = DateTime.Now;
                step2.DueOn = DateTime.Now.AddDays(4);
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
                step3.ReceivedOn = DateTime.Now;
                step3.DueOn = DateTime.Now.AddDays(6);
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
                step4.ReceivedOn = DateTime.Now;
                step4.DueOn = DateTime.Now.AddDays(8);
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
                step5.ReceivedOn = DateTime.Now;
                step5.DueOn = DateTime.Now.AddDays(10);
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
                step6.ReceivedOn = DateTime.Now;
                step6.DueOn = DateTime.Now.AddDays(12);
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
        [HttpPost("CreateWorkFlowStepAction")]
        public async Task<bool> CreateWorkFlowStepAction(int WorkflowStepId, [FromForm] IFormCollection formData)
        {
            var stepAction = await this.ConvertformDataToStepAction(formData);
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

                        WorkflowDocument workflowDocument = new WorkflowDocument();
                        workflowDocument.StepId = stepAction.Id;
                        workflowDocument.WorkflowId = WorkflowStepId;
                        workflowDocument.DocumentPath = filePath;
                        workflowDocument.DocumentName = file.Name;
                        workflowDocument.UploadedOn = DateTime.Now;
                        await _context.AddAsync(workflowDocument);
                        await _context.SaveChangesAsync();
                        pathData.Add(new
                        {
                            Name = file.Name,
                            Path = filePath
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
                    Files = pathData // Embed pathData (list of files)
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
                  
                    if (workflowstep.StepName == "Review Scope and Site Requirements" && stepAction.ModificationRequest == "Minor Work"
                        && stepAction.SubCategory == "Without Charges")
                    {
                        // workflows.Status = "Approved";
                        workflowstep.Status = "In Progress";
                        await _context.SaveChangesAsync();
                        await SendModificationEmail2("");
                        return true;
                    }
                    var nextstepflow = await _context.WorkflowSteps.Where(x => x.StepName == nextstepname && x.WorkflowId == workflowstep.WorkflowId).FirstOrDefaultAsync();
                   // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(nextstepflow.AssignedTo);
                    var deserializedData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextstepflow.AssignedTo);

                    foreach (var item in deserializedData)
                    {
                        if (item["Rights"].ToString() == "Edit")
                        {
                            await CreateTask(nextstepflow.WorkflowId.Value, nextstepflow.Id.ToString(), "Workflow", Convert.ToInt32(item["Id"].ToString()), "Modification Request", workflows.InitiatorId.Value);
                        }
                    }
                    nextstepflow.Status = "In Progress";
                    await _context.SaveChangesAsync();
                    if (workflowstep.StepName == "Confirm Payment Received")
                    {
                        workflows.Status = "Approved";
                        await _context.SaveChangesAsync();
                    }
                }
                return true;
            }
            else
            {
                return false;
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
                deserializedData.Add(new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "RFI" });
                var data = new { Id = stepAction.AssignTo.ToString(), Status = "Not Approved", Rights = "RFI" };
                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                string jsonString2 = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
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
                foreach (var item in deserializedData)
                {
                    if (item["Rights"] == "Edit" && item["Id"].ToString() == stepAction.PerformedBy.ToString())
                    {
                        item["Status"] = "Not Approved";
                        item["Rights"] = "View";
                    }
                }

                // var deserializedData = JsonSerializer.Deserialize<List<dynamic>>(workflowstep.AssignedTo);

                var data = new Dictionary<string, object> {
                    { "Id" , stepAction.AssignTo.ToString() },{ "Status" , "Not Approved" },{ "Rights" , "Edit" }
                };

                deserializedData.Add(data);
                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                string jsonString2 = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
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

                foreach (var item in deserializedData)
                {
                    if (item["Rights"].ToString() == "Edit")
                    {
                        await CreateTask(workflowstep.WorkflowId.Value, prevstepflow.Id.ToString(), "Return Step", Convert.ToInt32(item["Id"].ToString()), "Modification Request", workflows.InitiatorId.Value);
                        item["Status"] = "Not Approved";
                    }
                }

                string jsonString = JsonSerializer.Serialize(deserializedData, new JsonSerializerOptions { WriteIndented = true });
                workflowstep.Details = jsonString;
                // workflowstep.ExecutedOn = DateTime.Now;
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
            Models.Task task = new Models.Task();
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
        public async Task<IEnumerable<TaksVM>> GetTaks(int userid)
        {
            try
            {
                var lst = await _context.Tasks.Where(x => x.AssignedTo == userid).ToListAsync();
                var taskslst = Mapper.MapToDtos<Task, TaksVM>(lst);
                return taskslst;
            }
            catch (Exception ex)
            {
                return new List<TaksVM>();
            }
        }
        [HttpPost("GetWorkFlow")]
        public async Task<WorkFlowVM> GetWorkFlow(int workflowid)
        {
            try
            {
                var wf = await _context.Workflows.FindAsync(workflowid);
                var WorkFlowVMobj = Mapper.MapToDto<Workflow, WorkFlowVM>(wf);
                var lst = await _context.WorkflowSteps.Where(x => x.WorkflowId == workflowid).ToListAsync();
                var WFSslst = Mapper.MapToDtos<WorkflowStep, WorkFlowStepVM>(lst);
                WorkFlowVMobj.workFlowStepVMs = WFSslst;
                WorkFlowVMobj.WorkflowTypeName = (await _context.WorkflowTypes.FindAsync(WorkFlowVMobj.WorkflowTypeId)).Name;
                WorkFlowVMobj.InitiatorName = (await _context.Users.FindAsync(WorkFlowVMobj.InitiatorId)).Username;
                WorkFlowVMobj.Department = (await _context.Users.FindAsync(WorkFlowVMobj.InitiatorId)).Department;
                return WorkFlowVMobj;
            }
            catch (Exception ex)
            {
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
            }
            catch (Exception ex)
            {
                
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

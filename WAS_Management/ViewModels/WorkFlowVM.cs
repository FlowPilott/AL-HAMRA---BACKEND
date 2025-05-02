using System.Diagnostics;
using WAS_Management.Models;

namespace WAS_Management.ViewModels
{
    public class WorkFlowVM
    {
        public int Id { get; set; }

        public int? WorkflowTypeId { get; set; }

        public int? InitiatorId { get; set; }

        public string? Subject { get; set; }

        public string? Status { get; set; }

        public DateTime? StartedOn { get; set; }

        public string? Progress { get; set; }

        public int? ProcessOwner { get; set; }

        public string? Details { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public string? ReceiptNo { get; set; }

        public string? ReceiptBy { get; set; }

        public decimal? Amount { get; set; }

        public string? VendorName { get; set; }

        public string? PaidBy { get; set; }

        public DateTime? ApprovalStartDate { get; set; }

        public DateTime? ApprovalEndDate { get; set; }

        public string? InteractionId { get; set; }
        public string? WorkflowTypeName { get; set; }
        public string? InitiatorName { get; set; }
        public string? Department { get; set; }
        public string? Identifier { get; set; }

        public string? typeofWork { get; set; }

        public IEnumerable<WorkFlowStepVM> workFlowStepVMs { get; set; }
        public IEnumerable<WorkflowDocument> Documents { get; set; }

        public IEnumerable<object> InterationData { get; set; }
      
    }
    public class WorkFlowStepVM
    {
        public int Id { get; set; }

        public int? WorkflowId { get; set; }

        public string? StepName { get; set; }

        public string? StepDescription { get; set; }

        public string? Type { get; set; }

        public string? AssignedTo { get; set; }

        public string? Status { get; set; }

        public DateTime? ReceivedOn { get; set; }

        public DateTime? DueOn { get; set; }

        public DateTime? ExecutedOn { get; set; }

        public string? Details { get; set; }

        public string? Actiondetails { get; set; }
        public int? ApprovedBy { get; set; }

        public List<WorkflowDocument> Documents { get; set; } = new List<WorkflowDocument>();
    }

    public class DetailModelVM
    {
        public string Id { get; set; }
        public string Rights { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime PerformedOn { get; set; }
        public string RequestedBy { get; set; }
        public string IterationType { get; set; }
        public string StepId { get; set; }

        // New property to hold the answer
        public string Answer { get; set; }
        public string Files { get; set; }
        public string TaskId { get; set; }
    }


    public class PdfGenerator
    {
        public string GeneratePdf(string htmlPath, string outputPath)
        {
            // Path to the wkhtmltopdf binary (ensure it's in PATH or provide the full path)
            string wkhtmlPath = "/usr/bin/wkhtmltopdf";

            // Create the process
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = wkhtmlPath,
                    Arguments = $"--enable-local-file-access \"{htmlPath}\" \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            try
            {
                // Start the process
                process.Start();

                // Capture any output or errors
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"wkhtmltopdf failed. Error: {error}");
                }

                return outputPath; // Return the path to the generated PDF
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                throw;
            }
        }
    }

}

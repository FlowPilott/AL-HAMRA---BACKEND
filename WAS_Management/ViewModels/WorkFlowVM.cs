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

        public IEnumerable<WorkFlowStepVM> workFlowStepVMs { get; set; }
        public IEnumerable<WorkflowDocument> Documents { get; set; }

        public IEnumerable<Interaction> InterationData { get; set; }
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
    }


}

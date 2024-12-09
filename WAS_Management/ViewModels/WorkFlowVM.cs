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

        public IEnumerable<WorkFlowStepVM> workFlowStepVMs { get; set; }
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
    }
}

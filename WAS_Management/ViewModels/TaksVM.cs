namespace WAS_Management.ViewModels
{
    public class TaksVM
    {
        public int Id { get; set; }

        public int? WorkflowId { get; set; }

        public int? StepId { get; set; }

        public string? TaskType { get; set; }

        public string? Department { get; set; }

        public string? Template { get; set; }

        public DateTime? DueDate { get; set; }

        public int? Ageing { get; set; }

        public string? Status { get; set; }

        public int? AssignedTo { get; set; }
    }
}

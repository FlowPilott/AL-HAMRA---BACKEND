using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class WorkflowStep
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
}

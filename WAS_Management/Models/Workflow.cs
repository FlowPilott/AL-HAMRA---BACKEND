using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class Workflow
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
}

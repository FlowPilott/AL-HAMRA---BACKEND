using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class WorkflowDocument
{
    public int Id { get; set; }

    public int? WorkflowId { get; set; }

    public int? StepId { get; set; }

    public string? DocumentName { get; set; }

    public string? DocumentPath { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? UploadedOn { get; set; }
}

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

    public DateTime? ReceiptDate { get; set; }

    public string? ReceiptNo { get; set; }

    public string? ReceiptBy { get; set; }

    public decimal? Amount { get; set; }

    public string? VendorName { get; set; }

    public string? PaidBy { get; set; }

    public DateTime? ApprovalStartDate { get; set; }

    public DateTime? ApprovalEndDate { get; set; }

    public string? InteractionId { get; set; }

    public string? Identifier { get; set; }
}

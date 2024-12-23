using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class StepAction
{
    public int Id { get; set; }

    public int? StepId { get; set; }

    public string? ActionType { get; set; }

    public int? PerformedBy { get; set; }

    public DateTime? PerformedOn { get; set; }

    public string? Comments { get; set; }

    public string? Category { get; set; }

    public string? SubCat { get; set; }

    public decimal? ModificationFee { get; set; }

    public decimal? UnlistedContractorFee { get; set; }

    public decimal? BuiltupExtensionFee { get; set; }

    public decimal? Total { get; set; }

    public string? InvNuumber { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? ContactNumber { get; set; }

    public int? AssignTo { get; set; }

    public string? SubCategory { get; set; }

    public string? ModificationRequest { get; set; }
}

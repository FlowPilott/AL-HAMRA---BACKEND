using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class Interaction
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? CustomerType { get; set; }

    public string? TypeSel { get; set; }

    public string? ProjectName { get; set; }

    public string? UnitNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? ContactNumber { get; set; }

    public string? TypeOfInteraction { get; set; }

    public string? PurposeOfInteraction { get; set; }

    public string? FollowUp { get; set; }

    public DateTime? FollowUpDate { get; set; }

    public string? Remarks { get; set; }

    public string? PropertyType { get; set; }

    public string? OwnerName { get; set; }

    public string? ContactNo { get; set; }

    public string? Email { get; set; }

    public string? WorkType { get; set; }

    public string? InternalWork { get; set; }

    public string? ContractAgreement { get; set; }

    public string? ContractorRepName { get; set; }

    public string? ContractorCompName { get; set; }

    public string? ContractorName { get; set; }

    public string? ContractorContact { get; set; }

    public string? ContractorEmail { get; set; }

    public string? ContractorComp { get; set; }

    public string? ScopeOfWork { get; set; }

    public string? TradeLicence { get; set; }

    public string? EmirateId { get; set; }

    public DateTime? EndDuration { get; set; }

    public string? ThirdPartyLiabilityCert { get; set; }

    public string? CurrentLayout { get; set; }

    public string? ProposedLayout { get; set; }

    public DateTime? StartDuration { get; set; }

    public string? CustomerName { get; set; }

    public string? TradeLicenceNo { get; set; }
}

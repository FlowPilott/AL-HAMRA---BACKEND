using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class Contractor
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Contact { get; set; }

    public string? Email { get; set; }

    public string? CompanyName { get; set; }

    public string? ScopeOfWork { get; set; }

    public string? ContractorTradeLicence { get; set; }

    public string? EmiratesId { get; set; }

    public string? ThirdPartyLiability { get; set; }

    public string? CurrentProposedLayout { get; set; }

    public string? PaymentOptions { get; set; }

    public int? Isactive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
}

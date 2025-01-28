using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class ContractorForm
{
    public int Id { get; set; }

    public string? Company { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Contact { get; set; }

    public string? TradeLicence { get; set; }

    public string? EmiratesId { get; set; }

    public string? ThirdPartyLiability { get; set; }

    public string? VehicleRegistration { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace WAS_Management.Models;





#nullable enable
public partial class StepAction
{
    [NotMapped]
    public List<string>? Checkboxes { get; set; }  // Not stored in DB

    [NotMapped]
    public string? approvalstatus { get; set; }  // Not stored in DB
}
#nullable disable



public class ResaleNOCVM
{

    public string mastercomm { get; set; }
    public string projectName { get; set; }
    public string email { get; set; }
    public string unitNumber { get; set; }

    public string customerName { get; set; }
    public string contactNumber { get; set; }
    public string? Intiatorname { get; set; }

}

public class InteractionVM
{
    public string customertype { get; set; }

    public string unitNumber { get; set; }

    public string emailAddress { get; set; }
    public string contactNumber { get; set; }
}


public class InteractionResponseVM
{
    public string TicketNo { get; set; }
    public string URL { get; set; }
}

//public partial class ContractorForm
//{
//    public int Id { get; set; }

//    public string? Company { get; set; }

//    public string? Name { get; set; }

//    public string? Email { get; set; }

//    public string? Contact { get; set; }

//    public string? TradeLicence { get; set; }

//    public string? EmiratesId { get; set; }

//    public string? ThirdPartyLiability { get; set; }

//    public string? VehicleRegistration { get; set; }
//}

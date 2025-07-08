using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using WAS_Management.ViewModels;

namespace WAS_Management.Models;





#nullable enable
public partial class StepAction
{
    [NotMapped]
    public List<string>? Checkboxes { get; set; }  // Not stored in DB

    [NotMapped]
    public string? approvalstatus { get; set; }  // Not stored in DB

    [NotMapped]
    public DateTime? ReceivedOn { get; set; }

    [NotMapped]
    public DateTime? DueOn { get; set; }

    [NotMapped]
    public DateTime? ExecutedOn { get; set; }
}

public partial class Interaction
{
    [NotMapped]
    public string? paymentOption { get; set; }

}

public class WorkflowStepActionDetails
{
    public int PerformedBy { get; set; }  // Helps identify the user
    public StepAction StepAction { get; set; }
    public string Status { get; set; }
    public string Rights { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public DateTime? DueOn { get; set; }
    public DateTime? ExecutedOn { get; set; }

  
    public List<FileItem> Files { get; set; } = new();
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

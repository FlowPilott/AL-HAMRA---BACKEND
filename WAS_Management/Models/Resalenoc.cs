using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class Resalenoc
{
    public int Id { get; set; }

    public string? Mastercomm { get; set; }

    public string? Projectname { get; set; }

    public string? Email { get; set; }

    public string? Subprojectname { get; set; }

    public string? Unitno { get; set; }

    public string? Customername { get; set; }

    public string? Contactno { get; set; }

    public string? Createdby { get; set; }

    public string? Createddate { get; set; }

    public string? Intiatorname { get; set; }
}

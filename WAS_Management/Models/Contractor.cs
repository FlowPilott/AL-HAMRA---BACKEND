using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class Contractor
{
    public int Id { get; set; }

    public string? CompanyName { get; set; }

    public string? Address { get; set; }

    public string? Landlineno { get; set; }

    public string? Mobileno { get; set; }

    public string? Email { get; set; }

    public string? Plateno { get; set; }

    public string? Platetype { get; set; }

    public string? Placeofissuance { get; set; }

    public string? Typeofvehicle { get; set; }

    public string? Color { get; set; }

    public string? Previouspermit { get; set; }

    public string? Companytradelicence { get; set; }

    public string? Emirateid { get; set; }

    public string? Authorization { get; set; }

    public string? VehicleReg { get; set; }

    public string? VehiclePic { get; set; }

    public string? Passportid { get; set; }

    public DateTime? RenewalDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? Isactive { get; set; }

    public string? Bpnumber { get; set; }

    public string? Paymentoption { get; set; }
}

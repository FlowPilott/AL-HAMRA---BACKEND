using System;
using System.Collections.Generic;

namespace WAS_Management.Models;

public partial class WorkflowType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}

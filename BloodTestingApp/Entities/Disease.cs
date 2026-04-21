using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class Disease
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ResultDisease> ResultDiseases { get; set; } = new List<ResultDisease>();
}

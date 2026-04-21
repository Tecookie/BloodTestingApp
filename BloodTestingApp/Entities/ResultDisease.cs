using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class ResultDisease
{
    public int Id { get; set; }

    public int? ResultId { get; set; }

    public int? DiseaseId { get; set; }

    public string? Note { get; set; }

    public virtual Disease? Disease { get; set; }

    public virtual BloodTestResult? Result { get; set; }
}

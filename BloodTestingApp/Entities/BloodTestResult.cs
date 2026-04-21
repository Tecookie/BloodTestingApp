using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class BloodTestResult
{
    public int Id { get; set; }

    public int? AppointmentId { get; set; }

    public int? DoctorId { get; set; }

    public string? ResultText { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual ICollection<ResultDisease> ResultDiseases { get; set; } = new List<ResultDisease>();
}

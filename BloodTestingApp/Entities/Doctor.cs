using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class Doctor
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? FullName { get; set; }

    public string? Specialty { get; set; }

    public virtual ICollection<AppointmentRequest> AppointmentRequests { get; set; } = new List<AppointmentRequest>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<BloodTestResult> BloodTestResults { get; set; } = new List<BloodTestResult>();

    public virtual User? User { get; set; }
}

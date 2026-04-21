using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class AppointmentRequest
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int DoctorId { get; set; }

    public string? Status { get; set; }

    public DateTime? RespondedAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}

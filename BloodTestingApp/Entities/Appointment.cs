using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class Appointment
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Status { get; set; }

    public int? AssignedDoctorId { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AppointmentRequest> AppointmentRequests { get; set; } = new List<AppointmentRequest>();

    public virtual Doctor? AssignedDoctor { get; set; }

    public virtual BloodTestResult? BloodTestResult { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}

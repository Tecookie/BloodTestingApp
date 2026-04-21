using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? FullName { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User? User { get; set; }
}

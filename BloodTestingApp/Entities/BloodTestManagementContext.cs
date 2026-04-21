using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BloodTestingApp.Entities;

public partial class BloodTestManagementContext : DbContext
{
    public BloodTestManagementContext()
    {
    }

    public BloodTestManagementContext(DbContextOptions<BloodTestManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentRequest> AppointmentRequests { get; set; }

    public virtual DbSet<BloodTestResult> BloodTestResults { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<ResultDisease> ResultDiseases { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07FDFACF9E");

            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.AssignedDoctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AssignedDoctorId)
                .HasConstraintName("FK__Appointme__Assig__46E78A0C");

            entity.HasOne(d => d.Customer).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Custo__45F365D3");
        });

        modelBuilder.Entity<AppointmentRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07EF01330E");

            entity.HasIndex(e => new { e.AppointmentId, e.DoctorId }, "UQ_Appointment_Doctor").IsUnique();

            entity.Property(e => e.RespondedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentRequests)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__4BAC3F29");

            entity.HasOne(d => d.Doctor).WithMany(p => p.AppointmentRequests)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Docto__4CA06362");
        });

        modelBuilder.Entity<BloodTestResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BloodTes__3214EC070C084C0C");

            entity.HasIndex(e => e.AppointmentId, "UQ__BloodTes__8ECDFCC3DC850EBC").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Appointment).WithOne(p => p.BloodTestResult)
                .HasForeignKey<BloodTestResult>(d => d.AppointmentId)
                .HasConstraintName("FK__BloodTest__Appoi__5165187F");

            entity.HasOne(d => d.Doctor).WithMany(p => p.BloodTestResults)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__BloodTest__Docto__52593CB8");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07BD1C8F60");

            entity.HasIndex(e => e.UserId, "UQ__Customer__1788CC4DAF19A6C6").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.User).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.UserId)
                .HasConstraintName("FK__Customers__UserI__3D5E1FD2");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Diseases__3214EC07DBA76021");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Doctors__3214EC0751A0CB63");

            entity.HasIndex(e => e.UserId, "UQ__Doctors__1788CC4D0742AEBD").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Specialty).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .HasConstraintName("FK__Doctors__UserId__412EB0B6");
        });

        modelBuilder.Entity<ResultDisease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ResultDi__3214EC07B677D29F");

            entity.HasIndex(e => new { e.ResultId, e.DiseaseId }, "UQ_Result_Disease").IsUnique();

            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasOne(d => d.Disease).WithMany(p => p.ResultDiseases)
                .HasForeignKey(d => d.DiseaseId)
                .HasConstraintName("FK__ResultDis__Disea__59063A47");

            entity.HasOne(d => d.Result).WithMany(p => p.ResultDiseases)
                .HasForeignKey(d => d.ResultId)
                .HasConstraintName("FK__ResultDis__Resul__5812160E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC078854A0E2");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E486A0750B").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

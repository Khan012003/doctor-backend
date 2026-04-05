using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ClinicContext : DbContext
{
    public ClinicContext(DbContextOptions<ClinicContext> options) : base(options) { }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<MedicationItem> Medications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relations
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Prescription)
            .WithOne()
            .HasForeignKey<Prescription>(p => p.AppointmentId)
            .IsRequired(false);

        modelBuilder.Entity<Prescription>()
            .HasMany(p => p.Medications)
            .WithOne()
            .HasForeignKey(m => m.PrescriptionId);

        // Map primitive collections for PostgreSQL
        modelBuilder.Entity<MedicationItem>()
            .Property(m => m.Frequencies)
            .HasColumnType("text[]");

        modelBuilder.Entity<Prescription>()
            .Property(p => p.RequiredTests)
            .HasColumnType("text[]");
    }
}

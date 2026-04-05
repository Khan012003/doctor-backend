namespace Backend.Models;

public class Prescription
{
    public string Id { get; set; } = string.Empty;
    
    // Foreign Key to Appointment
    public string AppointmentId { get; set; } = string.Empty;
    
    public string Diagnosis { get; set; } = string.Empty;
    public string? DoctorComments { get; set; }
    public string Date { get; set; } = string.Empty;

    // We use a List backing field for EF Core 
    public List<MedicationItem> Medications { get; set; } = new();

    public string[]? RequiredTests { get; set; }
}

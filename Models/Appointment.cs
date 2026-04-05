namespace Backend.Models;

public class Appointment
{
    public string Id { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int QueueNumber { get; set; }
    public bool? IsRevisit { get; set; }

    // Navigation property placeholder
    public Prescription? Prescription { get; set; }
}

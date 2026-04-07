namespace Backend.Models;

public class MedicationItem
{
    public string Id { get; set; } = string.Empty;
    
    // Foreign Key to Prescription
    public string PrescriptionId { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    public string Timing { get; set; } = string.Empty;
    public int DaysToTake { get; set; }
    
    // We can store string array as JSON in a single column or use a separate table.
    // For simplicity with EF Core 8 primitive collections, we can map string[] directly to JSON/Array.
    public string[] Frequencies { get; set; } = Array.Empty<string>();
}

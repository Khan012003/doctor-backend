using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Database Context
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ClinicContext>(options =>
    options.UseNpgsql(connectionString));

// Enable CORS
var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(frontendUrl)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In-memory status for simplicity
bool isClinicOpen = true;

// Define endpoints

app.MapGet("/api/queue", async (ClinicContext db) =>
{
    return await db.Appointments.ToListAsync();
});

app.MapGet("/api/history", async (ClinicContext db) =>
{
    return await db.Prescriptions.Include(p => p.Medications).ToListAsync();
});

app.MapGet("/api/status", () =>
{
    return Results.Ok(isClinicOpen);
});

app.MapPost("/api/status", async (HttpRequest request) =>
{
    // toggle
    isClinicOpen = !isClinicOpen;
    return Results.Ok(isClinicOpen);
});

app.MapPost("/api/book", async (Appointment appointment, ClinicContext db) =>
{
    // Generate an ID if needed
    if (string.IsNullOrEmpty(appointment.Id))
        appointment.Id = Guid.NewGuid().ToString("N").Substring(0, 8);

    appointment.Status = "Pending";
    
    // Determine next queue number
    var maxQueue = await db.Appointments.MaxAsync(a => (int?)a.QueueNumber) ?? 0;
    appointment.QueueNumber = maxQueue + 1;

    db.Appointments.Add(appointment);
    await db.SaveChangesAsync();

    return Results.Ok(appointment.QueueNumber);
});

app.MapPost("/api/complete", async (CompleteRequest req, ClinicContext db) =>
{
    var appointment = await db.Appointments.FindAsync(req.AppointmentId);
    if (appointment != null)
    {
        appointment.Status = "Completed";
    }

    var prescription = new Prescription
    {
        Id = Guid.NewGuid().ToString("N").Substring(0, 8),
        AppointmentId = req.AppointmentId,
        Diagnosis = req.Diagnosis,
        DoctorComments = req.DoctorComments,
        Date = DateTime.UtcNow.ToString("o"),
        Medications = req.Medications,
        RequiredTests = req.RequiredTests
    };

    foreach (var med in prescription.Medications) {
        med.Id = Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    db.Prescriptions.Add(prescription);
    await db.SaveChangesAsync();

    return Results.Ok();
});

// Run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicContext>();
    db.Database.EnsureCreated();
}

app.Run();

// DTO
public class CompleteRequest
{
    public string AppointmentId { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string? DoctorComments { get; set; }
    public List<MedicationItem> Medications { get; set; } = new();
    public string[]? RequiredTests { get; set; }
}

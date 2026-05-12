using PdfOrganizer.Services;
using QuestPDF.Infrastructure;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<PdfService>();

var app = builder.Build();

QuestPDF.Settings.License = LicenseType.Community;

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapFallbackToFile("index.html");

Process.Start(new ProcessStartInfo
{
    FileName = "http://localhost:5000",
    UseShellExecute = true
});

app.Run("http://localhost:5000");
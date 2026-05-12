using QuestPDF.Infrastructure;
using PdfOrganizer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PdfService>();

var app = builder.Build();

QuestPDF.Settings.License = LicenseType.Community;

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.MapControllers();

app.Run();
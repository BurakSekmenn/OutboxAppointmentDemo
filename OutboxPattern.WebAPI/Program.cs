using Mapster;
using Microsoft.EntityFrameworkCore;
using OutboxPattern.WebAPI.Context;
using OutboxPattern.WebAPI.Dtos;
using OutboxPattern.WebAPI.Enum;
using OutboxPattern.WebAPI.Models;
using OutboxPattern.WebAPI.Services;
using Scalar.AspNetCore;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors();

builder.Services.AddOpenApi();


builder.Services.AddHostedService<AppointmentBackGroundService>();


builder.Services.AddFluentEmail("RandevuBilgilendirme@info.com")
    .AddSmtpSender("localhost",25);

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OutboxPattern.WebAPI.Context.AppDbContext>(options =>
{
    options.UseMySql(connectionstring, MySqlServerVersion.LatestSupportedServerVersion);
});


var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();


app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapPost("/appointment", async (AppointmentCreat request, AppDbContext context, CancellationToken cancellationtoken) =>
{
    Appointment appointment = request.Adapt<Appointment>();

    await context.Appointments.AddAsync(appointment, cancellationtoken);

    var payload = new IcsEmailPayload(
        To: appointment.Email,
        Subject: "Randevu Bilgilendirme",
        Body: $"Merhaba {appointment.Name}, randevunuz {appointment.StartsAtUtc:yyyy-MM-dd HH:mm} – {appointment.EndsAtUtc:HH:mm} (UTC) aralýðýnda oluþturuldu.",
        StartUtc: appointment.StartsAtUtc,
        EndUtc: appointment.EndsAtUtc
        );
    // 3) Idempotency için DedupKey (ayný randevu ikinci kez yazýlýrsa atlanýr)
    var dedup = $"ics:{appointment.Email}:{appointment.StartsAtUtc:O}:{appointment.EndsAtUtc:O}";

    await context.OutboxMessages.AddAsync(new OutboxMessage
    {
        MessageType = OutboxMessageType.IcsEmail,
        PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload),
        DedupKey = dedup
    });

    await context.SaveChangesAsync(cancellationtoken);

    return Results.Ok("Randevu baþarýlya oluþturulud");

});

app.MapGet("/appointment", async (AppDbContext context, CancellationToken cancellationtoken) =>
{
   
    List<Appointment> appointments = await context.Appointments.ToListAsync(cancellationtoken);
    return Results.Ok(appointments);

});





app.Run();

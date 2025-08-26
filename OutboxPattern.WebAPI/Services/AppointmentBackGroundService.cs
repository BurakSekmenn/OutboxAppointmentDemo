using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using OutboxPattern.WebAPI.Context;
using OutboxPattern.WebAPI.Dtos;
using OutboxPattern.WebAPI.Enum;
using System.Text;
using System.Text.Json;

namespace OutboxPattern.WebAPI.Services
{
    public class AppointmentBackGroundService(IServiceProvider serviceProvider, ILogger<AppointmentBackGroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("AppointmentBackGroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var fluentmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                    var messages = await dbcontext.OutboxMessages
                        .Where(x => x.Status == OutboxStatus.Pending)
                        .OrderBy(x => x.CreatedAtUtc)
                        .Take(10)
                        .ToListAsync(stoppingToken);

                    foreach (var msg in messages)
                    {
                        try
                        {
                            if (msg.MessageType == OutboxMessageType.IcsEmail)
                                     await HandleIcsEmailAsync(fluentmail, msg.PayloadJson);
                   

                            msg.Status = OutboxStatus.Completed;
                            msg.ProcessedAtUtc = DateTime.UtcNow;
                            msg.LastError = null;
                        }
                        catch (Exception ex)
                        {
                            msg.Attempt++;
                            msg.LastError = ex.Message;

                            var delay = TimeSpan.FromSeconds(Math.Pow(2, msg.Attempt) * 10); 
                            msg.NextAttemptAtUtc = DateTime.UtcNow.Add(delay);

                            if (msg.Attempt >= 5)
                                msg.Status = OutboxStatus.Failed;
                        }
                    }
                    if (messages.Count > 0)
                        await dbcontext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // logger.LogError(ex, "Outbox döngü hatası");
                }

                // Döngüler arasında kısa bekleme
                await Task.Delay(2000, stoppingToken);
            }
        }

        private async Task HandleIcsEmailAsync(IFluentEmail fluentEmail, string payloadJson)
        {
            var payload = JsonSerializer.Deserialize<IcsEmailPayload>(payloadJson)!;

            var ics = $@"
BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//OutboxDemo//TR
METHOD:PUBLISH
BEGIN:VEVENT
DTSTART:{payload.StartUtc:yyyyMMddTHHmmssZ}
DTEND:{payload.EndUtc:yyyyMMddTHHmmssZ}
SUMMARY:{payload.Subject}
DESCRIPTION:{payload.Body}
END:VEVENT
END:VCALENDAR";

            await fluentEmail
                .To(payload.To)
                .Subject(payload.Subject)
                .Body(payload.Body)
                .Attach(new Attachment
                {
                    Data = new MemoryStream(Encoding.UTF8.GetBytes(ics)),
                    Filename = "invite.ics",
                    ContentType = "text/calendar"
                })
                .SendAsync();
        }
    }
}

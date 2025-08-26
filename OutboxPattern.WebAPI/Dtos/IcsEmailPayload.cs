namespace OutboxPattern.WebAPI.Dtos
{
    public sealed record IcsEmailPayload(
     string To,
     string Subject,
     string Body,
     DateTime StartUtc,
     DateTime EndUtc
 );
}

namespace OutboxPattern.WebAPI.Dtos
{
    public sealed record AppointmentCreat(string Name, string Email, DateTime StartsAtUtc, DateTime EndsAtUtc,string Notes);
  
}

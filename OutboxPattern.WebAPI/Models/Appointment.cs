namespace OutboxPattern.WebAPI.Models
{
    public sealed class Appointment : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime StartsAtUtc { get; set; }      
        public DateTime EndsAtUtc { get; set; }        
        public string? Notes { get; set; }
        public string BusinessKey => $"apt:{Id}:{StartsAtUtc:O}";
    }
}

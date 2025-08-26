using OutboxPattern.WebAPI.Enum;

namespace OutboxPattern.WebAPI.Models
{
    public sealed class OutboxMessage : BaseEntity
    {
        public OutboxMessageType MessageType { get; set; }
        public string PayloadJson { get; set; } = default!;

        // İşleme durumu
        public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
        public int Attempt { get; set; } = 0;                // kaç kez denendi
        public DateTime? NextAttemptAtUtc { get; set; }      // exponential backoff için

        // Idempotency (aynı olayı ikinci kez işlememek için)
        public string? DedupKey { get; set; }

        // İş bittiğinde doldur
        public DateTime? ProcessedAtUtc { get; set; }
        public string? LastError { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutboxPattern.WebAPI.Enum;
using OutboxPattern.WebAPI.Models;

namespace OutboxPattern.WebAPI.Configrution
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> b)
        {
            b.ToTable("OutboxMessages");
            b.HasKey(x => x.Id);

            b.Property(x => x.MessageType)
                .IsRequired();

            b.Property(x => x.PayloadJson)
                .IsRequired();

            b.Property(x => x.Status)
                .HasDefaultValue(OutboxStatus.Pending)
                .IsRequired();

            b.Property(x => x.Attempt)
                .HasDefaultValue(0)
                .IsRequired();

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.HasIndex(x => x.DedupKey).IsUnique().HasFilter("[DedupKey] IS NOT NULL");

          
            b.HasIndex(x => new { x.Status, x.NextAttemptAtUtc });
        }
    }
}

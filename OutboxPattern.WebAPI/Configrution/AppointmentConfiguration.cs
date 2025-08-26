using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutboxPattern.WebAPI.Models;

namespace OutboxPattern.WebAPI.Configrution
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> b)
        {
            b.ToTable("Appointments");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            b.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(320); 

            b.Property(x => x.StartsAtUtc)
                .IsRequired();

            b.Property(x => x.EndsAtUtc)
                .IsRequired();

            b.Property(x => x.Notes)
                .HasMaxLength(1000);

            b.Property(x => x.CreatedAtUtc).IsRequired();

            b.HasIndex(x => x.StartsAtUtc);
            b.HasIndex(x => x.Email);
        }
    }
}

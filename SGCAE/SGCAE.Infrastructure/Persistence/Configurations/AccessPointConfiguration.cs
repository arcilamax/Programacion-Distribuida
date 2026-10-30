using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.AccessPoints;

namespace SGCAE.Infrastructure.Persistence.Configurations;

public sealed class AccessPointConfiguration : IEntityTypeConfiguration<AccessPoint>
{
    public void Configure(EntityTypeBuilder<AccessPoint> builder)
    {
        builder.ToTable("AccessPoints");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(a => a.Name).IsUnique();
        builder.Property(a => a.Description).HasMaxLength(300);
        builder.Property(a => a.IsEnabled).IsRequired();
    }
}

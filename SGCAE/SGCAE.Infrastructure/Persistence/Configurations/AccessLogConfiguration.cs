using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.AccessLogs;
using SGCAE.Domain.AccessPoints;
using SGCAE.Domain.Employees;

namespace SGCAE.Infrastructure.Persistence.Configurations;

public sealed class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("AccessLogs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.CredentialCode).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Movement).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(l => l.Result).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(l => l.Reason).HasConversion<string>().HasMaxLength(40);
        builder.Property(l => l.OccurredAt).IsRequired();

        // Relaciones opcionales: un intento con credencial/punto desconocido también se registra.
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(l => l.EmployeeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AccessPoint>()
            .WithMany()
            .HasForeignKey(l => l.AccessPointId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para acelerar las consultas del historial.
        builder.HasIndex(l => l.OccurredAt);
        builder.HasIndex(l => l.EmployeeId);
        builder.HasIndex(l => l.AccessPointId);
    }
}

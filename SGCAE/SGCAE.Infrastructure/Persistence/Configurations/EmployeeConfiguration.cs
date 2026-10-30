using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.Employees;

namespace SGCAE.Infrastructure.Persistence.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Identification).HasMaxLength(30).IsRequired();
        builder.HasIndex(e => e.Identification).IsUnique();

        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();

        // El objeto de valor Email se guarda como un simple string.
        builder.Property(e => e.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(200)
            .IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();

        builder.Property(e => e.Position).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Area).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
    }
}

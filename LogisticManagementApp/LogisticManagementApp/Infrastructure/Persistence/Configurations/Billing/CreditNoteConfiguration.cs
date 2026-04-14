using LogisticManagementApp.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Billing
{
    public class CreditNoteConfiguration : IEntityTypeConfiguration<CreditNote>
    {
        public void Configure(EntityTypeBuilder<CreditNote> builder)
        {
            builder.ToTable("CreditNotes");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.CreditNoteNo)
                .IsUnique();

            builder.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BillToCompany)
                .WithMany()
                .HasForeignKey(x => x.BillToCompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

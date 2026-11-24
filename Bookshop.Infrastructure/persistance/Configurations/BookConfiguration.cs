using Bookshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookshop.Infrastructure.persistance.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BookName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.BookPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.ModifiedAt);
        }
    }
}

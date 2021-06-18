using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace PasswordQueryTool.Backend.Services.Parsing.StateMachines
{
    public class ImportStateEntityConfiguration : IEntityTypeConfiguration<ImportState>
    {
        public void Configure(EntityTypeBuilder<ImportState> builder)
        {
            builder.HasKey(c => c.CorrelationId);

            builder.Property(c => c.CorrelationId)
                .ValueGeneratedNever();

            builder.Property(c => c.CurrentState).IsRequired();

            builder.OwnsMany(p => p.Chunks, a =>
            {
                a.WithOwner(p => p.Owner);

                a.HasKey(e => e.Id);

                a.Property(c => c.Id)
                .ValueGeneratedNever();
            });

            builder.UseXminAsConcurrencyToken();
        }
    }
}
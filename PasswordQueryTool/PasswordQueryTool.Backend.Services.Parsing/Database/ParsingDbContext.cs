using Microsoft.EntityFrameworkCore;
using PasswordQueryTool.Backend.Services.Parsing.StateMachines;
using System;

namespace PasswordQueryTool.Backend.Services.Parsing.Database
{
    public class ParsingDbContext : DbContext
    {
        public ParsingDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ImportState> ImportStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ImportStateEntityConfiguration());
        }
    }
}
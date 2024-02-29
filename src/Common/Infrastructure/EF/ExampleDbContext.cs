using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF
{
    public class ExampleDbContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "public";

        public ExampleDbContext() : base()
        { }

        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(DEFAULT_SCHEMA);
            base.OnModelCreating(builder);

            // Change all datatype decimal default (18,2) => decimal(17, 3)
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(17, 3)");
            }
        }
    }
}

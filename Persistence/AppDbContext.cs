using Microsoft.EntityFrameworkCore;
using hh_napi.Domain;

namespace hh_napi.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<DataSource> DataSources { get; set; } = null!;
        public DbSet<DataPoint> DataPoints { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set table schemas
            modelBuilder.HasDefaultSchema("hh");
            modelBuilder.Entity<User>().ToTable("Users", "hh");
            modelBuilder.Entity<UserCredentials>().ToTable("UserCredentials", "hh");
            modelBuilder.Entity<DataSource>().ToTable("DataSources", "hh");
            modelBuilder.Entity<DataPoint>().ToTable("DataPoints", "hh");

            // Setup relationships
            modelBuilder.Entity<DataSource>()
                .HasOne(ds => ds.CreatedByUser).WithMany()
                .HasForeignKey(ds => ds.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DataPoint>()
                .HasOne(dp => dp.DataSource).WithMany(ds => ds.DataPoints)
                .HasForeignKey(dp => dp.DataSourceId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
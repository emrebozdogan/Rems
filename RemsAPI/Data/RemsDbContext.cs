using Microsoft.EntityFrameworkCore;
using RemsAPI.Entities;

namespace RemsAPI.Data;

public class RemsDbContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
  public DbSet<City> Cities { get; set; }
  public DbSet<District> Districts { get; set; }
  public DbSet<Neighborhood> Neighborhoods { get; set; }
  public DbSet<Property> Properties { get; set; }
  public DbSet<Log> Logs { get; set; }
  public DbSet<AnalysisResult> AnalysisResults { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Property>()
      .HasOne(p => p.User)
      .WithMany(u => u.Properties)
      .HasForeignKey(p => p.UserId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}

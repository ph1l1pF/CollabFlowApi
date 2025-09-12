using CollabFlowApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CollabFlowApi;

public class AppDbContext : DbContext
{
    public DbSet<Collaboration> Collaborations => Set<Collaboration>();

    public DbSet<User> Users => Set<User>();
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.AppleSub).IsUnique();
        });

        modelBuilder.Entity<Collaboration>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasIndex(c => new { c.Id, c.UserId })
                .IsUnique();

            entity.OwnsOne(c => c.Deadline);
            entity.OwnsOne(c => c.Fee);
            entity.OwnsOne(c => c.Partner);
            entity.OwnsOne(c => c.Script);
        });
    }
}
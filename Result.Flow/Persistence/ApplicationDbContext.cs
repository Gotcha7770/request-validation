using Microsoft.EntityFrameworkCore;

namespace Result.Flow.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CreditCard>()
            .HasKey(x => x.Number);
    }
}
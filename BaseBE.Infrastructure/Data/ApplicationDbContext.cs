using BaseBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseBE.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Age).IsRequired();
        });
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
}
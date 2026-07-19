using BaseBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseBE.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Author> Authors { get; set; } = null!;

    public DbSet<Book> Books { get; set; } = null!;

    public DbSet<Member> Members { get; set; } = null!;

    public DbSet<BorrowingTransaction> BorrowingTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Age).IsRequired();
        });

        modelBuilder.Entity<BorrowingTransaction>()
            .HasOne(x => x.Member)
            .WithMany(x => x.BorrowingTransactions)
            .HasForeignKey(x => x.MemberId);

        modelBuilder.Entity<BorrowingTransaction>()
            .HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookId);
    }
}
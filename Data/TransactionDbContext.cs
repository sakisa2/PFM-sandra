using Microsoft.EntityFrameworkCore;
using PFM.Backend.Database.Entities;

namespace PFM.Data
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TransactionSplit> TransactionSplits { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().HasKey(t => t.Id);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Kind)
                .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Direction)
                .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.Catcode)
                .HasPrincipalKey(c => c.Code);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentCode);
        }
    }
}

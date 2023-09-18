using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_NET.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            // *
        }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ItemVariant> ItemVariants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasKey(i => i.Item_Id);

            modelBuilder.Entity<Item>()
                .HasOne(ic => ic.Category)
                .WithMany()
                .HasForeignKey(c => c.Category_Id);

            modelBuilder.Entity<Category>()
                .HasKey(c => c.Category_Id);

            modelBuilder.Entity<Image>()
                .HasKey(i => i.Image_Id);

            modelBuilder.Entity<Image>()
                .HasOne(ii => ii.Item)
                .WithMany(i => i.Images)
                .HasForeignKey(i => i.Item_Id);

            modelBuilder.Entity<ItemVariant>()
                .HasKey(iv => iv.Variant_Id);

            modelBuilder.Entity<ItemVariant>()
                .HasOne(iv => iv.Item)
                .WithMany(i => i.Variants)
                .HasForeignKey(i => i.Item_Id);

            modelBuilder.Entity<Review>()
                 .HasKey(r => r.Review_Id);

            modelBuilder.Entity<Review>()
                .HasOne(ru => ru.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(u => u.User_Id);

            modelBuilder.Entity<Review>()
                .HasOne(ri => ri.Item)
                .WithMany(i => i.Reviews)
                .HasForeignKey(i => i.Item_Id);

            modelBuilder.Entity<User>()
                .HasKey(u => u.User_Id);

            //modelBuilder.Entity<User>()
            //    .HasOne
        }
    }
}

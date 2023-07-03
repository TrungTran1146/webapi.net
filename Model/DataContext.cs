using Microsoft.EntityFrameworkCore;

namespace WebAPI.Model
{
    public class DataContext : DbContext
    // IdentityDbContext<IdentityUser>
    {
        public DataContext() : base() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id)

                    .HasColumnType("serial")
                    .IsRequired();

            });

        }



        public DbSet<Brand> Brands { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<ImportProduct> ImportProducts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<TypeCar> TypeCars { get; set; }

        public DbSet<User> Users { get; set; }
    }

}

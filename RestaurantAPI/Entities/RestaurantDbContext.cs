using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace RestaurantAPI.Entities
{
    public class RestaurantDbContext : DbContext
    {
        string _connectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=RestaurantDb;Trusted_Connection=True;AttachDbFileName=C:\\Users\\carra\\source\\repos\\RestaurantAPI\\carraRestaurantDb.mdf";
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();
            modelBuilder.Entity<Role>()
                .Property(u => u.Name)
                .IsRequired();

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);
            modelBuilder.Entity<Dish>()
                .Property(d=>d.Name)
                .IsRequired();
            modelBuilder.Entity<Adress>()
                .Property(d => d.City)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Adress>()
                .Property(d => d.Street)
                .IsRequired()
                .HasMaxLength(50);
        }
        protected override void OnConfiguring (DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(_connectionString);
        }
    }
}

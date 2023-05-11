using DAL.Auth.Configuration;
using DAL.Auth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Auth.Repository
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public DbSet<Review> Review => Set<Review>();
        public DbSet<Rating> Rating => Set<Rating>();
        public DbSet<Like> Like => Set<Like>();

        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            

            modelBuilder.Entity<Review>()
                .HasOne(one => one.User)
                .WithMany(many => many.Reviews)
                .HasForeignKey(key => key.UserId);
        }
    }
}

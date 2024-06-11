using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options) : base(options)
        {
            
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<ArtistAlbumBridge> ArtistAlbumBridge { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArtistAlbumBridge>()
                .HasKey(_ => new { _.ArtistId, _.AlbumId });

            modelBuilder.Entity<Artist>()
                .HasMany(_ => _.Albums)
                .WithOne(_ => _.Artist)
                .HasForeignKey(_ => _.ArtistId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Album>()
                .HasMany(_ => _.Artists)
                .WithOne(_ => _.Album)
                .HasForeignKey(_ => _.AlbumId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

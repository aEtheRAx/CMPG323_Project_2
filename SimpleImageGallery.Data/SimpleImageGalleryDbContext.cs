using Microsoft.EntityFrameworkCore;
using SimpleImageGallery.Data.Models;

namespace SimpleImageGallery.Data
{
    public class SimpleImageGalleryDbContext : DbContext
    {
        public SimpleImageGalleryDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<ImageTag> ImageTags { get; set; }
    }
}

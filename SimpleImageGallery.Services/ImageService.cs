using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleImageGallery.Data;
using SimpleImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleImageGallery.Services
{

    public class ImageService : IImage
    {

        private readonly SimpleImageGalleryDbContext _ctx;
        public ImageService(SimpleImageGalleryDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<GalleryImage> GetAll()
        {
            return _ctx.GalleryImages.Include(SimpleImageGallery => SimpleImageGallery.Tags /*was Tag gewees*/);
        }

        public IEnumerable<GalleryImage> GetAll(string user_id)
        {
            return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.user_id == user_id);
        }

        public GalleryImage GetById(int id)
        {
            //return _ctx.GalleryImages.Find(id);
            return GetAll().Where(img => img.Id == id).First();
        }

        public IEnumerable<GalleryImage> GetWithTag(string tag)
        {
            return GetAll().Where(img => img.Tags.Any(t => t.Description == tag));
        }

        public CloudBlobContainer GetBlobContainer(string azureConnectionString, string containerName)
        {
            azureConnectionString = "DefaultEndpointsProtocol=https;AccountName=1storage4images;AccountKey=i1/w6pBFHwdSFTJmebWOtbQvjDSQiqQpldC+qVxweJTmzO1R+Ospj9K2DYqXcrArPxKAzA2XeJBvh/M+2x4O0w==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        public async Task SetImage(string title, string tags, Uri uri, string user_id)
        {
            var image = new GalleryImage
            {
                Title = title,
                Tags = ParseTags(tags),
                Url = uri.AbsoluteUri,
                Created = DateTime.Now,
                user_id = user_id
            };

            _ctx.Add(image);
            await _ctx.SaveChangesAsync();
        }

        public async Task SetImage(string title, string tags, Uri uri)
        {
            var image = new GalleryImage
            {
                Title = title,
                Tags = ParseTags(tags),
                Url = uri.AbsoluteUri,
                Created = DateTime.Now
            };

            _ctx.Add(image);
            await _ctx.SaveChangesAsync();
        }

        public List<ImageTag> ParseTags(string tags)
        {
            return tags.Split(",").Select(tag => new ImageTag {
                Description = tag}).ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleImageGallery.Data;
using SimpleImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SimpleImageGallery.Services
{

    public class ImageService : IImage
    {
        private IConfiguration _config;
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

        //Services to find image by means of metadata
        public IEnumerable<GalleryImage> SearchByTitle(string imageTitle,string user_id)
        {
            return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Title == imageTitle && SimpleImageGallery.user_id == user_id);
        }

        public IEnumerable<GalleryImage> SearchByID(int imageID, string user_id)
        {
            return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Id == imageID && SimpleImageGallery.user_id == user_id);
        }

        public IEnumerable<GalleryImage> SearchByTag(string imageTag, string user_id)
        {
            return GetAll().Where(img => img.Tags.Any(t => t.Description == imageTag));
            //return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Tags == ParseTags(imageTag) && SimpleImageGallery.user_id == user_id);
        }

        public IEnumerable<GalleryImage> SearchByUploadDate(string imageDate, string user_id)
        {
            return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Created.ToString() == imageDate && SimpleImageGallery.user_id == user_id);
        }

        public GalleryImage GetById(int id)
        {
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

        public string DeleteMediaFile(int id, string userId)
        {
            GalleryImage image = _ctx.GalleryImages.Find(id);

            if (userId != image.user_id)
                return "false";
            else
            {
                //ImageTag tag = _ctx.ImageTags.Find(_ctx.ImageTags.Where(t => Convert.ToInt16(t.GalleryImageId) == id));
                ImageTag tag = _ctx.ImageTags.Find(id - 2);
                _ctx.ImageTags.Remove(tag);
                //var imageTags = image.Tags.ToList();
                _ctx.GalleryImages.Remove(image);
                _ctx.SaveChanges();
                string BlobNameToDelete = image.Url.Split('/').Last();
                DeleteImage(BlobNameToDelete, "NameOfTheBlobContainer");
                return "true";
            }
        }

        public CloudBlockBlob DeleteImage(string BlobName, string ContainerName)
        {
            string blobstorageconnection = "DefaultEndpointsProtocol=https;AccountName=1storage4images;AccountKey=i1/w6pBFHwdSFTJmebWOtbQvjDSQiqQpldC+qVxweJTmzO1R+Ospj9K2DYqXcrArPxKAzA2XeJBvh/M+2x4O0w==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("imagcontainer");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);
            return blockBlob;
        }

        public async Task updateImage(int imageID, string newTitle, string tags)
        {
            //GalleryImage image = _ctx.GalleryImages.Find(Title);
            GalleryImage image = _ctx.GalleryImages.Find(imageID);
            image.Title = newTitle;
            image.Tags = ParseTags(tags);
            image.Created = DateTime.Now;
            await _ctx.SaveChangesAsync();
        }
    }
}

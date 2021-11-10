using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleImageGallery.Data;
using SimpleImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Logging;

namespace SimpleImageGallery.Services
{

    public class ImageService : IImage
    {
        private IConfiguration _config;
        private readonly SimpleImageGalleryDbContext _ctx;
        private readonly ILogger _logger;

        //Constructor
        public ImageService(SimpleImageGalleryDbContext ctx, ILogger<ImageService> logger)
        {
            _logger = logger;                   //Logging initiated
            _ctx = ctx;
        }

        //Used to get all of the images in the database
        public IEnumerable<GalleryImage> GetAll()
        {
            _logger.LogInformation("ImageService > GetAll() called");
            return _ctx.GalleryImages.Include(SimpleImageGallery => SimpleImageGallery.Tags);
        }

        //Get all of the images that the logged in user owns
        public IEnumerable<GalleryImage> GetAll(string user_id)
        {
            try
            {
                _logger.LogInformation("ImageService > GetAll(user_id) called");
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.user_id == user_id);
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, ex.Message);
                //Return an default image to indicate no result was found
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Title == "Error");
            }
        }

        //Services to find image by means of metadata
        public IEnumerable<GalleryImage> SearchByTitle(string imageTitle,string user_id)
        {
            try
            {
                _logger.LogInformation("ImageService > SearchByTitle called");
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Title == imageTitle && SimpleImageGallery.user_id == user_id);
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, ex.Message);
                //Return an default image to indicate no result was found
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Title == "Error");
            }
        }

        public IEnumerable<GalleryImage> SearchByID(int imageID, string user_id)
        {
            try
            {
                _logger.LogInformation("ImageService > SearchByID called");
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Id == imageID && SimpleImageGallery.user_id == user_id);
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, ex.Message);
                //Return an default image to indicate no result was found
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Title == "Error");
            }
        }

        public IEnumerable<GalleryImage> SearchByTag(string imageTag, string user_id)
        {
            try
            {
                _logger.LogInformation("ImageService > SearchByTag called");
                return GetAll().Where(img => img.Tags.Any(t => t.Description == imageTag));
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, ex.Message);
                //Return an default image to indicate no result was found
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Title == "Error");
            }
        }

        public IEnumerable<GalleryImage> SearchByUploadDate(string imageDate, string user_id)
        {
            try
            {
                _logger.LogInformation("ImageService > SearchByUploadDate called");
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Created.ToString() == imageDate && SimpleImageGallery.user_id == user_id);
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, ex.Message);
                //Return an default image to indicate no result was found
                return _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.Title == "Error");
            }
        }

        public GalleryImage GetById(int id)
        {
            try
            {
                _logger.LogInformation("ImageService > GetById called");
                return GetAll().Where(img => img.Id == id).First();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return GetAll().Where(img => img.Id == 34).First();
            } 
        }

        public IEnumerable<GalleryImage> GetWithTag(string tag)
        {
            return GetAll().Where(img => img.Tags.Any(t => t.Description == tag));
        }

        public CloudBlobContainer GetBlobContainer(string azureConnectionString, string containerName)
        {
            _logger.LogInformation("ImageService > GetBlobContainer called");
            azureConnectionString = "DefaultEndpointsProtocol=https;AccountName=1storage4images;AccountKey=i1/w6pBFHwdSFTJmebWOtbQvjDSQiqQpldC+qVxweJTmzO1R+Ospj9K2DYqXcrArPxKAzA2XeJBvh/M+2x4O0w==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        public async Task SetImage(string title, string tags, Uri uri, string user_id)
        {
            try
            {
                _logger.LogInformation("ImageService > SetImage called");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        //Set image override method
        public async Task SetImage(string title, string tags, Uri uri)
        {
            try
            {
                _logger.LogInformation("ImageService > SetImage called");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public List<ImageTag> ParseTags(string tags)
        {
            return tags.Split(",").Select(tag => new ImageTag {
                Description = tag}).ToList();
        }

        //Delete the photo along with user access and photo metadata (referential integrity) in database
        public string DeleteMediaFile(int id, string userId)
        {
            try
            {
                _logger.LogInformation("ImageService > DeleteMediaFile called");
                GalleryImage image = _ctx.GalleryImages.Find(id);

                if (userId != image.user_id)
                    return "false";
                else
                {
                    //ImageTag tag = _ctx.ImageTags.Find(_ctx.ImageTags.Where(t => Convert.ToInt16(t.GalleryImageId) == id));
                    ImageTag tag = _ctx.ImageTags.Find(id + 2);
                    _ctx.ImageTags.Remove(tag);
                    //var imageTags = image.Tags.ToList();
                    _ctx.GalleryImages.Remove(image);
                    _ctx.SaveChanges();
                    string BlobNameToDelete = image.Url.Split('/').Last();
                    DeleteImage(BlobNameToDelete, "NameOfTheBlobContainer");
                    return "true";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return "false";
            }
        }

        //Delete the image in blob storage
        public CloudBlockBlob DeleteImage(string BlobName, string ContainerName)
        {
            string blobstorageconnection = "DefaultEndpointsProtocol=https;AccountName=1storage4images;AccountKey=i1/w6pBFHwdSFTJmebWOtbQvjDSQiqQpldC+qVxweJTmzO1R+Ospj9K2DYqXcrArPxKAzA2XeJBvh/M+2x4O0w==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("imagcontainer");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);
            return blockBlob;
        }

        //update an image 
        public async Task updateImage(int imageID, string newTitle, string tags)
        {
            try
            {
                GalleryImage image = _ctx.GalleryImages.Find(imageID);
                ImageTag imageTag = _ctx.ImageTags.Find(imageID + 2);
                imageTag.Description = tags;
                //_ctx.ImageTags.Remove(imageTag);
                image.Title = newTitle;
                image.Created = DateTime.Now;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task ShareImage(string title, string tags, Uri uri, string user_id)
        {
                _logger.LogInformation("ImageService > ShareImage called");
                var image = new GalleryImage
                {
                    Title = title,
                    Tags = ParseTags(tags),
                    Url = uri.AbsoluteUri,
                    Created = DateTime.Now,
                    user_id = user_id
                };
                _ctx.GalleryImages.Add(image);
                await _ctx.SaveChangesAsync();
        }
    }
}

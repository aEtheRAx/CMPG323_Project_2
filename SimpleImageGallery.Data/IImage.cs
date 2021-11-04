using Microsoft.WindowsAzure.Storage.Blob;
using SimpleImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageGallery.Data
{
    public interface IImage
    {
        //Used in the GalleryController > Index 
        IEnumerable<GalleryImage> GetAll();
        IEnumerable<GalleryImage> GetAll(string user_id);
        
        //Services to find image by means of metadata
        IEnumerable<GalleryImage> SearchByTitle(string imageTitle, string user_id);
        IEnumerable<GalleryImage> SearchByID(int imageID, string user_id);
        IEnumerable<GalleryImage> SearchByTag(string imageTag, string user_id);
        IEnumerable<GalleryImage> SearchByUploadDate(string imageDate, string user_id);

        IEnumerable<GalleryImage> GetWithTag(string tag);
        GalleryImage GetById(int id);
        CloudBlobContainer GetBlobContainer(string connectionString, string containerName);
        
        //Used to upload new images
        Task SetImage(string title, string tags, Uri uri);
        Task SetImage(string title, string tags, Uri uri, string user_id);
        
        //Used to break string of tags into list of tags
        List<ImageTag> ParseTags(string tags);

        //Used to maintain images
        Task updateImage(int imageID, string newTitle, string tags);
        string DeleteMediaFile(int id, string userId);


    }
}

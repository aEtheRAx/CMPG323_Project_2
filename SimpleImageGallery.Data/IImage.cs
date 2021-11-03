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

        IEnumerable<GalleryImage> GetAll();
        IEnumerable<GalleryImage> GetAll(string user_id);
        IEnumerable<GalleryImage> GetWithTag(string tag);
        GalleryImage GetById(int id);
        CloudBlobContainer GetBlobContainer(string connectionString, string containerName);
        Task SetImage(string title, string tags, Uri uri);
        Task SetImage(string title, string tags, Uri uri, string user_id);
        List<ImageTag> ParseTags(string tags);
        string DeleteMediaFile(int id);

    }
}

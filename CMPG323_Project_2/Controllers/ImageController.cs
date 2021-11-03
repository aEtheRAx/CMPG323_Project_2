using CMPG323_Project_2.Data;
using CMPG323_Project_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleImageGallery.Data;
//using SimpleImageGallery.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMPG323_Project_2.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private IConfiguration _config;
        private IImage _imageService;
        private string AzureConnectionString { get; }

        private readonly SimpleImageGalleryDbContext _ctx;

        public ImageController(IConfiguration config, IImage imageService)
        {
            _config = config;
            _imageService = imageService;
            AzureConnectionString = _config["AzureStorageConnectionString"];
        }

        public IActionResult Upload()
        {
            var model = new UploadImageModel();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadNewImage(IFormFile file, string tags, string title)
        {
            ImageController imgController = new ImageController(_config, _imageService);
            var container = _imageService.GetBlobContainer(AzureConnectionString, "imagcontainer");
            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

            var fileName = content.FileName.Trim('"');
            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());

            //string strCurrentUserName = User.Identity.Name;
            string strCurrentUserName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _imageService.SetImage(title, tags, blockBlob.Uri,strCurrentUserName );
            return RedirectToAction("Index", "Gallery");
        }

        public IActionResult Delete(int id)
        {
            string value = _imageService.DeleteMediaFile(id);
            if (value == "true")
                return RedirectToAction("Index", "Gallery");
            else
                return RedirectToAction("Index", "Gallery");
        }


        /*
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(IFormFile file)
        {
            ImageController imgController = new ImageController(_config, _imageService);
            var container = _imageService.GetBlobContainer(AzureConnectionString, "imagcontainer");
            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');
            var blob = container.GetBlockBlobReference(fileName);
            await blob.DeleteIfExistsAsync();
            return RedirectToAction("Index");
        }*/

        /*
        public async Task<IActionResult> Delete(int id)
        {
            /*ImageController imgController = new ImageController(_config, _imageService);
            var container = _imageService.GetBlobContainer(AzureConnectionString, "imagcontainer");
            var blockBlob = container.GetBlockBlobReference();
            await blockBlob.DeleteIfExistsAsync();
            return RedirectToAction("Index", "Gallery");*/
        /*string blobstorageconnection = _config.GetValue<string>("blobstorage");
        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
        CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        string strContainerName = "imagcontainer";
        CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
        var blob = cloudBlobContainer.GetBlobReference(blobName);
        //await blob.DeleteIfExistsAsync();
        await blob.DeleteAsync();
        return RedirectToAction("Contact", "Home");
    }*/




        /*public void DeleteBlob(string BlobName, string ContainerName)
        {
            string blobstorageconnection = _config.GetValue<string>("blobstorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            //ImageController imgController = new ImageController(_config, _imageService);
            var container = _imageService.GetBlobContainer(AzureConnectionString, "imagcontainer");
            var blockBlob = container.GetBlockBlobReference(BlobName);
            blockBlob.DeleteAsync();
        }*/

        /*
        public async Task<IActionResult> DeleteBlobData(string fileUrl)
        {
            Uri uriObj = new Uri(fileUrl);
            string BlobName = Path.GetFileName(uriObj.LocalPath);

            string blobstorageconnection = _config.GetValue<string>("blobstorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "imagcontainer";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

            string pathPrefix = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/";
            CloudBlobDirectory blobDirectory = cloudBlobContainer.GetDirectoryReference(pathPrefix);
            // get block blob refarence    
            CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(BlobName);

            // delete blob from container        
            await blockBlob.DeleteAsync();
            return RedirectToAction("Index", "Gallery");
        }*/
        /*
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, IFormFile file)
        {
            
            var image = _ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.user_id == id.ToString());
            var container = _imageService.GetBlobContainer(AzureConnectionString, "imagcontainer");
            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');
            var blob = container.GetBlockBlobReference(fileName);
            await blob.DeleteIfExistsAsync();

            _ctx.GalleryImages.Remove((SimpleImageGallery.Data.Models.GalleryImage)image);
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Index", "Gallery");
        }*/
        /*
        //enlear academy
        public void DeleteBlob(string BlobName, string ContainerName)
        {
            string blobstorageconnection = _config.GetValue<string>("blobstorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobName);
            blockBlob.DeleteAsync();
        }

        public ActionResult DeleteMediaFile(int id)
        {
            SimpleImageGalleryDbContext image = new SimpleImageGalleryDbContext();
            image = (SimpleImageGalleryDbContext)_ctx.GalleryImages.Where(SimpleImageGallery => SimpleImageGallery.user_id == id.ToString());
            _ctx.GalleryImages.Remove((SimpleImageGallery.Data.Models.GalleryImage)image);
            _ctx.SaveChanges();
            string BlobNameToDelete = image.Url.Split('/').Last();
            DeleteBlob(BlobNameToDelete, "imagcontainer");         // container name
            return RedirectToAction("Index", "Gallery");                             // return page
        }*/

        //Delete the image in blob storage
        /*public async Task<bool> DeleteImage(string resourceId)
        {
            string blobstorageconnection = _config.GetValue<string>("blobstorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("imagcontainer");
            string BlobNameToDelete = resourceId.Split('/').Last();
            CloudBlockBlob blob = container.GetBlockBlobReference(BlobNameToDelete);
            return await blob.DeleteIfExistsAsync();

        }*/


    }
}

using CMPG323_Project_2.Models;
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
using System.Threading.Tasks;

namespace CMPG323_Project_2.Controllers
{
    public class ImageController : Controller
    {
        private IConfiguration _config;
        private IImage _imageService;
        private string AzureConnectionString { get; }

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

            await _imageService.SetImage(title, tags, blockBlob.Uri);
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

        public async Task<IActionResult> Delete(string blobName)
        {
            string blobstorageconnection = _config.GetValue<string>("blobstorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "imagcontainer";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
            var blob = cloudBlobContainer.GetBlobReference(blobName);
            await blob.DeleteIfExistsAsync();
            return RedirectToAction("Index", "Gallery");
        }
    }
}

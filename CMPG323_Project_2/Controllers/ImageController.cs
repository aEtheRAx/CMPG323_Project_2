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

        public IActionResult Update()
        {
            var model = new updateImageModel();
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

        [HttpPost]
        public async Task<IActionResult> updateExistingImage(int imageID, string newTitle, string tags)
        {
            var image = _imageService.GetById(imageID);
            await _imageService.updateImage(imageID, newTitle, tags);
            return RedirectToAction("Detail", "Gallery");
        }

        public IActionResult Delete(int id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string value = _imageService.DeleteMediaFile(id, userID);
            if (value == "true")
                return RedirectToAction("Index", "Gallery");
            else
                return RedirectToAction("Index", "Gallery");
        }
    }
}

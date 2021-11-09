using CMPG323_Project_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimpleImageGallery.Data;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace CMPG323_Project_2.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private IConfiguration _config;
        //Image service database
        private IImage _imageService;
        //Securely get the connection string to the Azure SQL database
        private string AzureConnectionString { get; }

        private readonly ILogger _logger;

        //Overwrite contructor
        public ImageController(IConfiguration config, IImage imageService)
        {
            _config = config;
            _imageService = imageService;
            AzureConnectionString = _config["AzureStorageConnectionString"];    //Gets the connection string
        }

        //Overwrite contructor
        public ImageController(IConfiguration config, IImage imageService, ILogger<GalleryController> logger)
        {
            _config = config;
            _imageService = imageService;
            _logger = logger;                                                   //Logging initiated
            AzureConnectionString = _config["AzureStorageConnectionString"];    //Set the connection string
        }

        //View for UPLOADING a new image
        public IActionResult Upload()
        {
            var model = new UploadImageModel();
            return View();
        }

        //View for UPDATING an existing image
        public IActionResult Update()
        {
            var model = new updateImageModel();
            return View();
        }

        //Method to upload a new image 
        [HttpPost]
        public async Task<IActionResult> UploadNewImage(IFormFile file, string tags, string title)
        {
            _logger.LogInformation("Upload new Image initaited");
            try
            {
                ImageController imgController = new ImageController(_config, _imageService);
                var container = _imageService.GetBlobContainer(AzureConnectionString, "imagcontainer");
                var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                var fileName = content.FileName.Trim('"');
                var blockBlob = container.GetBlockBlobReference(fileName);
                await blockBlob.UploadFromStreamAsync(file.OpenReadStream());

                //Alternative way to get userID: string strCurrentUserName = User.Identity.Name;
                string strCurrentUserName = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _imageService.SetImage(title, tags, blockBlob.Uri, strCurrentUserName);
                return RedirectToAction("Index", "Gallery");
            }
            catch (Exception ex)
            {
                //Log the exception
                _logger.LogError(ex, "Failed to update image: " + ex.Message);
                //Return the gallery (index)
                return RedirectToAction("Index", "Gallery");
            }       
        }

        //Method to update an existing image
        [HttpPost]
        public async Task<IActionResult> updateExistingImage(int imageID, string newTitle, string Tags)
        {
            _logger.LogInformation("Update an existing Image initaited");
            try
            {
                await _imageService.updateImage(imageID, newTitle, Tags);
                return RedirectToAction("Index", "Gallery");
            }
            catch (Exception ex)
            {
                //Log the exception
                _logger.LogError(ex, "Failed to update image: " + ex.Message);
                //Return the gallery (index)
                return RedirectToAction("Index", "Gallery");
            }
        }

        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Delete Image initaited");
            try
            {
                string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string value = _imageService.DeleteMediaFile(id, userID);
                if (value == "true")
                    return RedirectToAction("Index", "Gallery");
                else
                    return RedirectToAction("Index", "Gallery");
            }
            catch (Exception ex)
            {
                //Log the exception
                _logger.LogError(ex, "Failed to delete image: " + ex.Message);
                //Return the gallery (index)
                return RedirectToAction("Index", "Gallery");
            }
        }
    }
}

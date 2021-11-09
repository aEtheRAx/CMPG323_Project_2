using CMPG323_Project_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleImageGallery.Data;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace CMPG323_Project_2.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    { 
        private readonly IImage _imageService;
        private readonly ILogger _logger;

        public GalleryController(IImage imageService, ILogger<GalleryController> logger)
        {
            _logger = logger;                   //Logging initiated
            _imageService = imageService;       //ImageService initaited
        }

        public IActionResult Index()
        {
            //Log the fact that the index page was accessed.
            _logger.LogInformation("Gallery Index page accessed");
            //Get the currentl logged in users' ID
            string strCurrentUserName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var imageList = _imageService.GetAll(strCurrentUserName);
            var model = new GalleryIndexModel()
            {
                Images = imageList,
                SearchQuery = ""
            };
            return View(model);
        }

        public IActionResult Detail(int id)
        {
            _logger.LogInformation("Reviewing image details...");
            try
            {
                //Get the Image by ID to view it's details
                var image = _imageService.GetById(id);
                //Create an object of the image and return the image along with it's details
                var model = new GalleryDetailModel()
                {
                    Id = image.Id,
                    Title = image.Title,
                    CreatedOn = image.Created,
                    Url = image.Url,
                    Tags = image.Tags.Select(t => t.Description).ToList()
                };
                //Return this view
                return View(model);
            }
            catch (Exception ex)
            {
                //Log the exception
                _logger.LogError(ex, "Failed to view details: " + ex.Message);
                //Return the gallery (index)
                return View("Index");
            }
        }

        public IActionResult SearchImages(string searchString, string selectedValue)
        {
            //Log the fact that a image was searched.
            _logger.LogInformation("Image search initiated");
            //Get the ID of the user currently logged in
            string strCurrentUserName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                //The following code searches through the tables in search for a image with matching metadata.
                //Valid images are returned
                if (selectedValue == "1")           //Title
                {
                    var imageList = _imageService.SearchByTitle(searchString, strCurrentUserName);
                    var model = new GalleryIndexModel()
                    {
                        Images = imageList,
                        SearchQuery = ""
                    };
                    return View(model);
                }
                else if (selectedValue == "2")      //ID
                {
                    var imageList = _imageService.SearchByID(Convert.ToInt16(searchString), strCurrentUserName);
                    var model = new GalleryIndexModel()
                    {
                        Images = imageList,
                        SearchQuery = ""
                    };
                    return View(model);
                }
                else if (selectedValue == "3")      //Tag
                {
                    var imageList = _imageService.SearchByTag(searchString, strCurrentUserName);
                    var model = new GalleryIndexModel()
                    {
                        Images = imageList,
                        SearchQuery = ""
                    };
                    return View(model);
                }
                else if (selectedValue == "4")      //Upload Date
                {
                    var imageList = _imageService.SearchByUploadDate(searchString, strCurrentUserName);
                    var model = new GalleryIndexModel()
                    {
                        Images = imageList,
                        SearchQuery = ""
                    };
                    return View(model);
                }
                else
                    return View("Index");
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while searching for a image: " + ex.Message);
                return View("Index");
            }
        }
    }
}

using CMPG323_Project_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleImageGallery.Data;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace CMPG323_Project_2.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        //private readonly SimpleImageGalleryDbContext _db; 
        private readonly IImage _imageService;
        private readonly ILogger _logger;

        public GalleryController(IImage imageService, ILogger<GalleryController> logger /*SimpleImageGalleryDbContext db*/)
        {
            //_db = db;
            _logger = logger;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
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
            var image = _imageService.GetById(id);
            var model = new GalleryDetailModel()
            {
                Id = image.Id,
                Title = image.Title,
                CreatedOn = image.Created,
                Url = image.Url,
                Tags = image.Tags.Select(t => t.Description).ToList()
            };
            return View(model);
        }

        //[HttpGet]
        public IActionResult SearchImages(string searchString, string selectedValue)
        {
            //Get the ID of the user currently logged in
            string strCurrentUserName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (selectedValue == "1")
            {
                var imageList = _imageService.SearchByTitle(searchString, strCurrentUserName);
                var model = new GalleryIndexModel()
                {
                    Images = imageList,
                    SearchQuery = ""
                };
                return View(model);
            }
            else if (selectedValue == "2")
            {
                var imageList = _imageService.SearchByID(Convert.ToInt16(searchString), strCurrentUserName);
                var model = new GalleryIndexModel()
                {
                    Images = imageList,
                    SearchQuery = ""
                };
                return View(model);
            }
            else if (selectedValue == "3")
            {
                var imageList = _imageService.SearchByTag(searchString, strCurrentUserName);
                var model = new GalleryIndexModel()
                {
                    Images = imageList,
                    SearchQuery = ""
                };
                return View(model);
            }
            else if (selectedValue == "4")
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
    }
}

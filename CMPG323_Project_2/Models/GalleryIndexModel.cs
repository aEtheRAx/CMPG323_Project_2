using SimpleImageGallery.Data.Models;
using System.Collections.Generic;

namespace CMPG323_Project_2.Models
{
    //Model for viewing the gallery 
    public class GalleryIndexModel
    {
        public IEnumerable<GalleryImage> Images { get; set; }
        public string SearchQuery { get; set; }
        public string selectedvalue { get; set; }
    }
}

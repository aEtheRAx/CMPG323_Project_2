using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMPG323_Project_2.Models
{
    //Model for displaying an Image details
    public class GalleryDetailModel
    {
        //basic attributes of a image
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
        public List<string> Tags { get; set; }
    }
}

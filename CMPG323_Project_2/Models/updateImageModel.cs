using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMPG323_Project_2.Models
{
    public class updateImageModel
    {
        public string Id { get; set; }
        public int imageID { get; set; }
        public string Title { get; set; }
        public string newTitle { get; set; }
        public string Tags { get; set; }
    }
}

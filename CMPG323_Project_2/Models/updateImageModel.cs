using System;
using System.Collections.Generic;

namespace CMPG323_Project_2.Models
{
    //Model for updating an Images
    public class updateImageModel
    {
        public string Id { get; set; }
        public int imageID { get; set; }
        public string Title { get; set; }
        public string newTitle { get; set; }
        public string Tags { get; set; }

    }
}

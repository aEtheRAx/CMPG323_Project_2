using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace CMPG323_Project_2.Models
{
    //Model for uploading a new image
    public class UploadImageModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public IFormFile ImageUpload { get; set; }
    }
}

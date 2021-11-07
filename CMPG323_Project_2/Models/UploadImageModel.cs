using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMPG323_Project_2.Models
{
    public class UploadImageModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public IFormFile ImageUpload { get; set; }
    }
}

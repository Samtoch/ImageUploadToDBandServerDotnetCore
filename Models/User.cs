using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageUploadToDBWithDotnetCore.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image_ { get; set; }
        public byte[] passport { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}

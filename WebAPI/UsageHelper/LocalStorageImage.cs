using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsageHelper
{
    public class LocalStorageImage
    {
        private readonly string Url;

        public LocalStorageImage(string url)
        {
            Url = url;
        }
        public async Task<string> Upload(IFormFile file,string fileName)
        {
            var ccc = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Resources\Images", fileName);
            using Stream fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return Path.Combine(@"Resources\Images", fileName);
        }
    }
}

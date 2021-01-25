using erp_project.Entities;
using erp_project.Entities.Tables;
using erp_project.Libraries.Abstracts;
using erp_project.Libraries.Models;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace erp_project.Services.Drawing
{
    public static class Drawing
    {
        public static bool Drawigs(List<int> Widths, List<int> Heights, string pathToSaveLarge, string[] Pathsave)
        {
         
            using (MagickImage image = new MagickImage(pathToSaveLarge))
            {
                image.Format = image.Format; // Get or Set the format of the image.
                image.Resize(Widths[0], Heights[0]); // fit the image into the requested width and height.
                image.Quality = 75; // This is the Compression level.
                image.Write(Pathsave[0]);
            }
            using (MagickImage image = new MagickImage(pathToSaveLarge))
            {
                image.Format = image.Format; // Get or Set the format of the image.
                image.Resize(Widths[1], Heights[1]); // fit the image into the requested width and height.
                image.Quality = 75; // This is the Compression level.
                image.Write(Pathsave[1]);
            }
            return true;
        }
    }
}

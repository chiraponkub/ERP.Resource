using erp_project.Entities;
using erp_project.Entities.Tables;
using erp_project.Libraries.Abstracts;
using erp_project.Libraries.Models;
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
        public static bool Drawigs(IFormFile file ,List<string> ListPathToSave)
        {
            for (int i = 0; i < 2; i++)
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var img = Image.FromStream(stream))
                    {
                        if (i == 0)
                        {
                            int Width = 640;
                            int Height = (Width * img.Size.Height) / img.Size.Width;
                            Bitmap newImage = new Bitmap(Width, Height);
                            using (var g = Graphics.FromImage(newImage))
                            {
                                g.DrawImage(img, 0, 0, Width, Height);
                                newImage.Save(ListPathToSave[i]);
                                g.Dispose();
                                newImage.Dispose();
                            }
                        }
                        else if (i == 1)
                        {
                            int Width = 1280;
                            int Height = (Width * img.Size.Height) / img.Size.Width;
                            Bitmap newImage = new Bitmap(Width, Height);
                            using (var g = Graphics.FromImage(newImage))
                            {
                                g.DrawImage(img, 0, 0, Width, Height);
                                newImage.Save(ListPathToSave[i]);
                                g.Dispose();
                                newImage.Dispose();
                            }
                        }
                        //else
                        //{
                        //    int Width = 1920;
                        //    int Height = (Width * img.Size.Height) / img.Size.Width;
                        //    Bitmap newImage = new Bitmap(Width, Height);
                        //    using (var g = Graphics.FromImage(newImage))
                        //    {
                        //        g.DrawImage(img, 0, 0, Width, Height);
                        //        newImage.Save(ListPathToSave[i]);
                        //        g.Dispose();
                        //        newImage.Dispose();
                        //    }
                        //}
                    }
                }
            }
            return true;
        }
    }
}

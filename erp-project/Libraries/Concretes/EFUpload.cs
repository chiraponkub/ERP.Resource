using erp_project.Entities;
using erp_project.Entities.Tables;
using erp_project.Libraries.Abstracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using static erp_project.Libraries.Models.m_Upload;

namespace erp_project.Libraries.Concretes
{
    public class EFUpload : IUpload
    {

        private readonly DBConnect db;

        public EFUpload(DBConnect db)
        {
            this.db = db;
        }

        public m_uploadfile UploadFile(IFormFile file, int id)
        {
            try
            {
                var folderName = Path.Combine("Resources", "File"); // folder ที่เก็บไฟล์ในโปรเจค 
                var folderName1 = Path.Combine("File"); // เอาไว้แสดงค่า return
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                var epoch1 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่
                var Split = file.FileName.Split("."); // ดึงค่านามสกุลไฟล์
                var fileNamehash = epoch1 + "." + Split[1]; // ชื่อไฟล์ + นามสกุล เอาไว้บันทึกลง ฐานข้อมูล
                var fullPath = folderName1 + "\\" + fileNamehash;  //folder ที่เก็บไฟล์ + ชื่อไฟล์ เอาไว้แสดงค่า return 
                var pathToSave = Directory.CreateDirectory(folderName) + "\\" + fileNamehash; // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(pathToSave, FileMode.Create))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        var savefiletodata = new Files
                        {
                            UserId = id,
                            Name = epoch1,
                            Type = Split[1],
                        };
                        db.Files.Add(savefiletodata);
                        db.SaveChanges();
                    }
                }

                var ssss = new m_uploadfile
                {
                    OriginalName = fileName,
                    NewFilename = fileNamehash,
                    Path = folderName1,
                    FullPath = fullPath,
                    Type = Split[1]
                };
                return ssss;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public m_uploadimage Uploadimage(IFormFile file, int id)
        {
            try
            {
                var folderName = Path.Combine("Resources", "Images"); // folder ที่เก็บไฟล์ในโปรเจค 
                var folderName1 = Path.Combine("Images"); // เอาไว้แสดงค่า return
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var epoch1 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                var Split = file.FileName.Split(".");
                var fileNamehash = epoch1 + "." + Split[1];


                var filenameSmall = "small-" + fileNamehash;
                var filenameMediun = "medium-" + fileNamehash;
                var filenameLarge = "large-" + fileNamehash;
                var fullPath = folderName1 + "\\large-" + fileNamehash;


                if (file.Length > 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var pathToSaveSmall = Directory.CreateDirectory(folderName)+"\\"+ filenameSmall; // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                        var pathToSaveMediun = Directory.CreateDirectory(folderName)+"\\"+ filenameMediun; // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                        var pathToSaveLarge = Directory.CreateDirectory(folderName)+"\\"+ filenameLarge; // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                        using (var stream = file.OpenReadStream())
                        {
                            using (var img = Image.FromStream(stream))
                            {

                                if (i == 0)
                                {
                                    int Width = img.Size.Width * 2;
                                    int Height = img.Size.Height * 2;
                                    Bitmap newImage = new Bitmap(Width, Height);
                                    using (var g = Graphics.FromImage(newImage))
                                    {
                                        g.DrawImage(img, 0, 0, Width, Height);
                                        newImage.Save(pathToSaveSmall);
                                        g.Dispose();
                                        newImage.Dispose();
                                    }
                                }
                                else if (i == 1)
                                {
                                    int Width = img.Size.Width * 3;
                                    int Height = img.Size.Height * 3;
                                    Bitmap newImage = new Bitmap(Width, Height);
                                    using (var g = Graphics.FromImage(newImage))
                                    {
                                        g.DrawImage(img, 0, 0, Width, Height);
                                        newImage.Save(pathToSaveMediun);
                                        g.Dispose();
                                        newImage.Dispose();
                                    }
                                }
                                else
                                {
                                    int Width = img.Size.Width * 4;
                                    int Height = img.Size.Height * 4;
                                    Bitmap newImage = new Bitmap(Width, Height);
                                    using (var g = Graphics.FromImage(newImage))
                                    {
                                        g.DrawImage(img, 0, 0, Width, Height);
                                        newImage.Save(pathToSaveLarge);
                                        g.Dispose();
                                        newImage.Dispose();
                                    }
                                }
                            }
                        }
                    }
                }
                var savefiletodata = new UploadFiles
                {
                    UserId = id,
                    Imagename = fileNamehash,
                };
                db.UploadFiles.Add(savefiletodata);
                db.SaveChanges();

                var list = new List<string> { filenameSmall, filenameMediun, filenameLarge };

                return new m_uploadimage
                {
                    OriginalName = fileName,
                    NewImagename = fileNamehash,
                    Path = folderName1,
                    fullPath = fullPath,
                    size = list
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}

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

        public List<m_uploadfile> UploadFile(List<IFormFile> files, Guid id, string SetPath)
        {
            try
            {
                var res = new List<m_uploadfile> { };
                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                    var NewName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่
                    var Split = file.ContentType; // ดึงค่านามสกุลไฟล์
                    string folderName;
                    string pathToSave;

                    if (SetPath != null)
                    {
                        folderName = (Path.Combine("Resources" , SetPath)).Replace("\\", "/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        pathToSave = (Directory.CreateDirectory(folderName) + "\\" + NewName).Replace("\\", "/");
                        using (var fileStream = new FileStream(pathToSave, FileMode.Create))
                        {
                            fileStream.Position = 0;
                            file.CopyTo(fileStream);
                            var savefiletodata = new Upload
                            {
                                Name = NewName,
                                Type = Split,
                                UserId = id,
                                Path = SetPath,
                                FullPath = pathToSave,
                                CreatedAt = DateTime.Now
                            };
                            db.Upload.Add(savefiletodata);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        folderName = (Path.Combine("Resources")).Replace("\\","/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        pathToSave = (Directory.CreateDirectory(folderName) + "\\" + NewName).Replace("\\","/");
                        using (var fileStream = new FileStream(pathToSave, FileMode.Create))
                        {
                            fileStream.Position = 0;
                            file.CopyTo(fileStream);
                            var savefiletodata = new Upload
                            {
                                Name = NewName,
                                Type = Split,
                                UserId = id,
                                Path = folderName,
                                FullPath = pathToSave,
                                CreatedAt = DateTime.Now
                            };
                            db.Upload.Add(savefiletodata);
                            db.SaveChanges();
                        }
                    }
                    res.Add(new m_uploadfile
                    {
                        OriginalName = fileName,
                        NewFilename = NewName,
                        Path = folderName,
                        FullPath = pathToSave,
                        Type = Split
                    });
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<m_removefile> removefiles(List<string> files, Guid id,string Path)
        {
            try
            {
                var res = new List<m_removefile> { };

                res.Add(new m_removefile
                {
                    NameFile = "",
                    Path = "",
                    FullPath = "",
                    Type = ""
                });
                return res;
            }
            catch (Exception ex)
            {
                throw new  Exception(ex.Message);
            }
        }


        public List<m_uploadimage> Uploadimage(List<IFormFile> files, Guid id, string SetPath)
        {
            try
            {
                string folderName;
                string filenameSmall;
                string filenameMediun;
                string filenameLarge;
                string fullPath;
                string NewName;
                string fileName;
                string pathToSaveSmall;
                string pathToSaveMediun;
                string pathToSaveLarge;

                string[] ssss = { "small-", "medium-", "large-" };

                var res = new List<m_uploadimage> { };
                foreach (var file in files)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                    NewName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่

                    var Split = file.ContentType; // ดึงค่านามสกุลไฟล์
                    var Split1 = file.ContentType.Split("/"); // ดึงค่านามสกุลไฟล์
                    if (SetPath != null)
                    {
                        folderName = (Path.Combine("Resources", SetPath)).Replace("\\","/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        for (int i = 0; i < 3; i++)
                        {
                            filenameSmall = "small-" + NewName;
                            filenameMediun = "medium-" + NewName;
                            filenameLarge = "large-" + NewName;
                            pathToSaveSmall = (Directory.CreateDirectory(folderName) + "\\" + filenameSmall + "." + Split1[1]).Replace("\\","/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveMediun = (Directory.CreateDirectory(folderName) + "\\" + filenameMediun + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            fullPath = folderName + "\\large-" + NewName;
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
                                            newImage.Save(pathToSaveSmall);
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
                                            newImage.Save(pathToSaveMediun);
                                            g.Dispose();
                                            newImage.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        int Width = 1920;
                                        int Height = (Width * img.Size.Height) / img.Size.Width;
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
                        filenameLarge = "large-" + NewName;
                        pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Split1[1]).Replace("\\","/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                        var savefiletodata1 = new Upload
                        {
                            Name = NewName,
                            Type = Split,
                            UserId = id,
                            Path = folderName,
                            FullPath = pathToSaveLarge,
                            CreatedAt = DateTime.Now
                        };
                        db.Upload.Add(savefiletodata1);
                        db.SaveChanges();
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            filenameSmall = "small-" + NewName;
                            filenameMediun = "medium-" + NewName;
                            filenameLarge = "large-" + NewName;
                            folderName = Path.Combine("Resources"); // folder ที่เก็บไฟล์ในโปรเจค 
                            pathToSaveSmall = (Directory.CreateDirectory(folderName) + "\\" + filenameSmall + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveMediun = (Directory.CreateDirectory(folderName) + "\\" + filenameMediun + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            fullPath = folderName + "\\large-" + NewName;
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
                                            newImage.Save(pathToSaveSmall);
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
                                            newImage.Save(pathToSaveMediun);
                                            g.Dispose();
                                            newImage.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        int Width = 1920;
                                        int Height = (Width * img.Size.Height) / img.Size.Width;
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
                        filenameLarge = "large-" + NewName;
                        folderName = (Path.Combine("Resources")).Replace("\\","/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge).Replace("\\","/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                        var savefiletodata = new Upload
                        {
                            Name = NewName,
                            Type = Split,
                            UserId = id,
                            Path = folderName,
                            FullPath = pathToSaveLarge,
                            CreatedAt = DateTime.Now
                        };
                        db.Upload.Add(savefiletodata);
                        db.SaveChanges();
                    }
                    filenameSmall = "small-" + NewName;
                    filenameMediun = "medium-" + NewName;
                    var list = new List<string> { filenameSmall, filenameMediun, filenameLarge };
                    res.Add(new m_uploadimage
                    {
                        OriginalName = fileName,
                        NewImagename = NewName,
                        Path = folderName,
                        fullPath = pathToSaveLarge,
                        size = list
                    });
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}

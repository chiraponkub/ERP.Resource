using erp_project.Entities;
using erp_project.Entities.Tables;
using erp_project.Libraries.Abstracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
                    var type = file.FileName.Split(".");  // ดึงค่านามสกุลไฟล์
                    string folderName;
                    string pathToSave;
                    string PathToSaveDb;

                    if (SetPath != null)
                    {
                        folderName = (Path.Combine("Resources", SetPath)).Replace("\\", "/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        pathToSave = (Directory.CreateDirectory(folderName) + "\\" + NewName + "." + type[1]).Replace("\\", "/");
                        PathToSaveDb = SetPath + "/" + NewName + "." + type[1].Replace("\\", "/");
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
                                FullPath = PathToSaveDb,
                                CreatedAt = DateTime.Now,
                                NewName = NewName + "." + type[1],
                                OriginalName = file.FileName
                            };
                            db.Upload.Add(savefiletodata);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        folderName = (Path.Combine("Resources")).Replace("\\", "/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        pathToSave = (Directory.CreateDirectory(folderName) + "\\" + NewName + "." + type[1]).Replace("\\", "/");
                        PathToSaveDb = NewName + "." + type[1].Replace("\\", "/");
                        using (var fileStream = new FileStream(pathToSave, FileMode.Create))
                        {
                            fileStream.Position = 0;
                            file.CopyTo(fileStream);
                            var savefiletodata = new Upload
                            {
                                Name = NewName,
                                Type = Split,
                                UserId = id,
                                Path = "",
                                FullPath = PathToSaveDb,
                                CreatedAt = DateTime.Now,
                                NewName = NewName + "." + type[1],
                                OriginalName = file.FileName
                            };
                            db.Upload.Add(savefiletodata);
                            db.SaveChanges();
                        }
                    }
                    res.Add(new m_uploadfile
                    {
                        OriginalName = fileName,
                        NewFilename = NewName,
                        Path = folderName.Contains("Resources/") ? SetPath : "",
                        FullPath = PathToSaveDb,
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

        public object removeFiles(List<string> files)
        {
            foreach (var file in files)
            {
                var DB = db.Upload;
                var sql = DB.FirstOrDefault(e => e.Name == file);
                var delete = Path.Combine("Resources" + "/" + sql.FullPath);
                File.Delete(delete);
                DB.Remove(sql);
                db.SaveChanges();
            }
            return "Deleted successfully.";
        }

        public object removeImage(List<string> files)
        {
            foreach (var file in files)
            {
                var DB = db.Upload;
                var sql = DB.FirstOrDefault(e => e.Name == file);
                var Split = sql.NewName.Split("-");
                var Paths = sql.Path;
                if (File.Exists(Path.Combine("Resources", sql.FullPath).Replace("\\","/")))
                {
                    var deleteSmall = Path.Combine("Resources" + "/" + Paths + "/small-" + Split[1]).Replace("//","/");
                    var deleteMediun = Path.Combine("Resources" + "/" + Paths + "/medium-" + Split[1]).Replace("//", "/");
                    var deleteLarge = Path.Combine("Resources" + "/" + Paths + "/large-" + Split[1]).Replace("//", "/");
                    File.Delete(deleteSmall);
                    File.Delete(deleteMediun);
                    File.Delete(deleteLarge);
                    DB.Remove(sql);
                    db.SaveChanges();
                }
            }
            return "Deleted successfully.";
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
                string PathToSaveDb;

                string[] ssss = { "small-", "medium-", "large-" };

                var res = new List<m_uploadimage> { };
                foreach (var file in files)
                {
                    var type = file.FileName.Split(".");  // ดึงค่านามสกุลไฟล์
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                    NewName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่

                    var Split = file.ContentType; // ดึงค่านามสกุลไฟล์
                    var Split1 = file.ContentType.Split("/"); // ดึงค่านามสกุลไฟล์
                    if (SetPath != null)
                    {
                        folderName = (Path.Combine("Resources", SetPath)).Replace("\\", "/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        for (int i = 0; i < 3; i++)
                        {
                            filenameSmall = "small-" + NewName;
                            filenameMediun = "medium-" + NewName;
                            filenameLarge = "large-" + NewName;
                            pathToSaveSmall = (Directory.CreateDirectory(folderName) + "\\" + filenameSmall + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveMediun = (Directory.CreateDirectory(folderName) + "\\" + filenameMediun + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            fullPath = folderName + "\\large-" + NewName;
                            PathToSaveDb = SetPath + "/" + NewName + "." + type[1].Replace("\\", "/");
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
                        pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Split1[1]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                        var SaveFullPath = SetPath + "/" + filenameLarge + "." + Split1[1];
                        var savefiletodata1 = new Upload
                        {
                            Name = NewName,
                            Type = Split,
                            UserId = id,
                            Path = SetPath,
                            FullPath = SaveFullPath,
                            CreatedAt = DateTime.Now,
                            NewName  = filenameLarge + "." + Split1[1],
                            OriginalName = file.FileName
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
                            PathToSaveDb = SetPath + "/" + NewName + "." + type[1].Replace("\\", "/");
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
                        filenameLarge = "large-" + NewName + "." + Split1[1].Replace("\\", "/");
                        folderName = (Path.Combine("Resources")).Replace("\\", "/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้

                        var savefiletodata = new Upload
                        {
                            Name = NewName,
                            Type = Split,
                            UserId = id,
                            Path = "",
                            FullPath =  filenameLarge,
                            CreatedAt = DateTime.Now,
                            NewName = filenameLarge,
                            OriginalName = file.FileName
                        };
                        db.Upload.Add(savefiletodata);
                        db.SaveChanges();
                    }
                    filenameSmall = "small-" + NewName + "." + Split1[1].Replace("\\", "/");
                    filenameMediun = "medium-" + NewName + "." + Split1[1].Replace("\\", "/");
                    filenameLarge = "large-" + NewName + "." + Split1[1].Replace("\\", "/");
                    var list = new List<string> { filenameSmall, filenameMediun, filenameLarge };
                    if (SetPath == null)
                    {
                        PathToSaveDb = filenameLarge;
                    }
                    else
                    {
                        PathToSaveDb = SetPath + "/" + filenameLarge;
                    }
                    res.Add(new m_uploadimage
                    {
                        OriginalName = fileName,
                        NewImageName = NewName,
                        Path = folderName.Contains("Resources/") ? SetPath : "",
                        fullPath = PathToSaveDb,
                        sizes = list
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

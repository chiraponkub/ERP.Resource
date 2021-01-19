using erp_project.Entities;
using erp_project.Entities.Tables;
using erp_project.Libraries.Abstracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

        public List<m_uploadfile> UploadFile(List<IFormFile> files,string userid ,string SetPath)
        {
            try
            {
                var res = new List<m_uploadfile> { };
                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                    var NewName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่
                    var ContentType = file.ContentType; // ดึงค่านามสกุลไฟล์
                    string folderName;
                    string pathToSave;
                    string PathToSaveDb;
                    var Splittype = file.FileName.Split(".");  // ดึงค่านามสกุลไฟล์
                    int Number = Splittype.Count();
                    int type = Number - 1;

                    if (SetPath != null)
                    {
                        folderName = (Path.Combine("wwwroot", SetPath)).Replace("\\", "/");
                        pathToSave = (Directory.CreateDirectory(folderName) + "\\" + NewName + "." + Splittype[type]).Replace("\\", "/");
                        PathToSaveDb = SetPath + "/" + NewName + "." + Splittype[type].Replace("\\", "/");
                        using (var fileStream = new FileStream(pathToSave, FileMode.Create))
                        {
                            fileStream.Position = 0;
                            file.CopyTo(fileStream);
                            var savefiletodata = new Upload
                            {
                                Name = NewName,
                                Type = ContentType,
                                UserId = Guid.Parse(userid),
                                Path = SetPath,
                                FullPath = PathToSaveDb,
                                CreatedAt = DateTime.Now,
                                NewName = NewName + "." + Splittype[type],
                                OriginalName = file.FileName
                            };
                            db.Upload.Add(savefiletodata);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        folderName = (Path.Combine("wwwroot")).Replace("\\", "/");
                        pathToSave = (Directory.CreateDirectory(folderName) + "\\" + NewName + "." + Splittype[type]).Replace("\\", "/");
                        PathToSaveDb = NewName + "." + Splittype[type].Replace("\\", "/");
                        using (var fileStream = new FileStream(pathToSave, FileMode.Create))
                        {
                            fileStream.Position = 0;
                            file.CopyTo(fileStream);
                            var savefiletodata = new Upload
                            {
                                Name = NewName,
                                Type = ContentType,
                                UserId = Guid.Parse(userid),
                                Path = "",
                                FullPath = PathToSaveDb,
                                CreatedAt = DateTime.Now,
                                NewName = NewName + "." + Splittype[type],
                                OriginalName = file.FileName
                            };
                            db.Upload.Add(savefiletodata);
                            db.SaveChanges();
                        }
                    }
                    res.Add(new m_uploadfile
                    {
                        OriginalName = fileName,
                        NewFilename = NewName + "." + Splittype[type],
                        Path = folderName.Contains("wwwroot/") ? SetPath : "",
                        FullPath = PathToSaveDb,
                        Type = ContentType
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
                string delete;
                var sql = DB.FirstOrDefault(e => e.Name == file);
                if (sql == null)
                {
                    if (File.Exists(Path.Combine("wwwroot", file).Replace("\\", "/")))
                    {
                        delete = Path.Combine("wwwroot", file).Replace("\\", "/");
                        File.Delete(delete);
                    }
                }
                else
                {
                    delete = Path.Combine("wwwroot" + "/" + sql.FullPath);
                    File.Delete(delete);
                    DB.Remove(sql);
                    db.SaveChanges();
                }
            }
            return "Deleted successfully.";
        }

        public object removeImage(List<string> files)
        {
            foreach (string thisfile in files)
            {
                string file = thisfile.Split("-").Last().Split(".").First();

                var DB = db.Upload;
                var sql = DB.FirstOrDefault(e => e.Name == file);
                
                string deleteSmall;
                string deleteMediun;
                string deleteLarge;
                if (sql == null)
                {
                    if (File.Exists(Path.Combine("wwwroot", file).Replace("\\", "/")))
                    {
                        string delete = Path.Combine("wwwroot", file).Replace("\\", "/");
                        File.Delete(delete);
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine("wwwroot", sql.FullPath).Replace("\\", "/")))
                    {
                        var Split = sql.NewName.Split("-");
                        var Paths = sql.Path;
                        deleteSmall = Path.Combine("wwwroot" + "/" + Paths + "/small-" + Split[1]).Replace("//", "/");
                        deleteMediun = Path.Combine("wwwroot" + "/" + Paths + "/medium-" + Split[1]).Replace("//", "/");
                        deleteLarge = Path.Combine("wwwroot" + "/" + Paths + "/large-" + Split[1]).Replace("//", "/");
                        File.Delete(deleteSmall);
                        File.Delete(deleteMediun);
                        File.Delete(deleteLarge);
                        DB.Remove(sql);
                        db.SaveChanges();
                    }
                }
            }
            return "Deleted successfully.";
        }

        public async void Uploadimage(IFormFile file, string userid, string SetPath, string NewName)
        {
            try
            {
                string folderName;
                string filenameSmall;
                string filenameMediun;
                string filenameLarge;
                string fullPath;
                string fileName;
                string pathToSaveSmall;
                string pathToSaveMediun;
                string pathToSaveLarge;
                string PathToSaveDb;

                string[] ssss = { "small-", "medium-", "large-" };

                var res = new List<m_uploadimage> { };
                //foreach (var file in files)
                //{
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                    //NewName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่

                    var ContentType = file.ContentType; // ดึงค่านามสกุลไฟล์
                    var Splittype = file.FileName.Split(".");  // ดึงค่านามสกุลไฟล์
                    int Number = Splittype.Count();
                    int type = Number - 1;
                    if (SetPath != null)
                    {
                        folderName = (Path.Combine("wwwroot", SetPath)).Replace("\\", "/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        for (int i = 0; i < 3; i++)
                        {
                            filenameSmall = "small-" + NewName;
                            filenameMediun = "medium-" + NewName;
                            filenameLarge = "large-" + NewName;
                            pathToSaveSmall = (Directory.CreateDirectory(folderName) + "\\" + filenameSmall + "." + Splittype[type]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveMediun = (Directory.CreateDirectory(folderName) + "\\" + filenameMediun + "." + Splittype[type]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Splittype[type]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            fullPath = folderName + "\\large-" + NewName;
                            PathToSaveDb = SetPath + "/" + NewName + "." + Splittype[type].Replace("\\", "/");
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
                        pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Splittype[type]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                        var SaveFullPath = SetPath + "/" + filenameLarge + "." + Splittype[type];
                        var savefiletodata1 = new Upload
                        {
                            Name = NewName,
                            Type = ContentType,
                            UserId = Guid.Parse(userid),
                            Path = SetPath,
                            FullPath = SaveFullPath,
                            CreatedAt = DateTime.Now,
                            NewName = filenameLarge + "." + Splittype[type],
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
                            folderName = Path.Combine("wwwroot"); // folder ที่เก็บไฟล์ในโปรเจค 
                            pathToSaveSmall = (Directory.CreateDirectory(folderName) + "\\" + filenameSmall + "." + Splittype[type]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveMediun = (Directory.CreateDirectory(folderName) + "\\" + filenameMediun + "." + Splittype[type]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge + "." + Splittype[type]).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้
                            fullPath = folderName + "\\large-" + NewName;
                            PathToSaveDb = SetPath + "/" + NewName + "." + Splittype[type].Replace("\\", "/");
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
                        filenameLarge = "large-" + NewName + "." + Splittype[type].Replace("\\", "/");
                        folderName = (Path.Combine("wwwroot")).Replace("\\", "/"); // folder ที่เก็บไฟล์ในโปรเจค 
                        pathToSaveLarge = (Directory.CreateDirectory(folderName) + "\\" + filenameLarge).Replace("\\", "/"); // ที่บันทึกไฟล์ ที่สามารถตั้งค่าได้

                        var savefiletodata = new Upload
                        {
                            Name = NewName,
                            Type = ContentType,
                            UserId = Guid.Parse(userid),
                            Path = "",
                            FullPath = filenameLarge,
                            CreatedAt = DateTime.Now,
                            NewName = filenameLarge,
                            OriginalName = file.FileName
                        };
                        db.Upload.Add(savefiletodata);
                        db.SaveChanges();
                    }
                //filenameSmall = "small-" + NewName + "." + Splittype[type].Replace("\\", "/");
                //filenameMediun = "medium-" + NewName + "." + Splittype[type].Replace("\\", "/");
                //filenameLarge = "large-" + NewName + "." + Splittype[type].Replace("\\", "/");
                //var list = new List<string> { filenameSmall, filenameMediun, filenameLarge };
                //if (SetPath == null)
                //{
                //    PathToSaveDb = filenameLarge;
                //}
                //else
                //{
                //    PathToSaveDb = SetPath + "/" + filenameLarge;
                //}
                //res.Add(new m_uploadimage
                //{
                //    OriginalName = fileName,
                //    NewImageName = NewName + "." + Splittype[type],
                //    Path = folderName.Contains("wwwroot/") ? SetPath : "",
                //    fullPath = PathToSaveDb,
                //    sizes = list
                //});
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<m_getupload> Get()
        {
            var sql = db.Upload.Select(e => new m_getupload 
            {
                fileName = e.Name,
                fullPath = e.FullPath,
                type = e.Type
            }).ToList();
            return sql;
        }
    }
}

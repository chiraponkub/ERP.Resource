using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using erp_project.Entities;
using erp_project.Libraries.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static erp_project.Libraries.Models.m_Upload;

namespace erp_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ERPControllerBase
    {
        private readonly IUpload Upload;
        private readonly DBConnect db;

        public UploadController(IUpload _upload, DBConnect db)
        {
            Upload = _upload;
            this.db = db;
        }

        /// <summary>
        /// แสดงข้อมูล
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("GetFile")]
        public IActionResult Get()
        {
            try
            {
                return Ok(Upload.Get());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// อัพโหลดไฟล์รูปภาพ
        /// </summary>
        /// <param name="files">ไฟล์รูปภาพที่อัพโหลด</param>
        /// <param name="SetPath">ที่เก็บไฟล์</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("Uploadimg")]
        public ActionResult<m_uploadimage> Uploadimage(List<IFormFile> files, string SetPath)
        {
            try
            {
                string[] ssss = { "small-", "medium-", "large-" };
                string PathToSaveDb;

                var userid = UserLoginId;

                if (SetPath != null && files.Count() == 0)
                {
                    return BadRequest("The image is not uploaded.");
                }
                if (files.Count() == 0 && SetPath == null)
                {
                    return BadRequest("The image is not uploaded.");
                }
                if (SetPath == null)
                {
                    List<m_uploadimage> res = new List<m_uploadimage>();
                    foreach (var file in files)
                    {
                        string NewName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่
                        var Splittype = file.FileName.Split(".");  // ดึงค่านามสกุลไฟล์
                        int Number = Splittype.Count();
                        int type = Number - 1;
                        string filenameSmall = "small-" + NewName + "." + Splittype[type].Replace("\\", "/");
                        string filenameMediun = "medium-" + NewName + "." + Splittype[type].Replace("\\", "/");
                        string filenameLarge = "large-" + NewName + "." + Splittype[type].Replace("\\", "/");
                        var list = new List<string> { filenameSmall, filenameMediun, filenameLarge };
                        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                        string folderName = (Path.Combine("wwwroot")).Replace("\\", "/");
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
                            NewImageName = NewName + "." + Splittype[type],
                            Path = folderName.Contains("wwwroot/") ? SetPath : "",
                            fullPath = PathToSaveDb,
                            sizes = list
                        });
                        //Task.Run(() =>
                        //{
                        //    Upload.Uploadimage(file, userid, SetPath = null, NewName);
                        //});
                        Upload.Uploadimage(file, userid, SetPath = null, NewName);
                    }
                    return Ok(res);
                }
                else
                {
                    List<m_uploadimage> res = new List<m_uploadimage>();
                    foreach (var file in files)
                    {
                        string NewName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); // ตั้งชื่อไฟล์ไหม่
                        var Splittype = file.FileName.Split(".");  // ดึงค่านามสกุลไฟล์
                        int Number = Splittype.Count();
                        int type = Number - 1;
                        string filenameSmall = "small-" + NewName + "." + Splittype[type].Replace("\\", "/");
                        string filenameMediun = "medium-" + NewName + "." + Splittype[type].Replace("\\", "/");
                        string filenameLarge = "large-" + NewName + "." + Splittype[type].Replace("\\", "/");
                        var list = new List<string> { filenameSmall, filenameMediun, filenameLarge };
                        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'); // ชื่อไฟล์
                        string folderName = (Path.Combine("wwwroot")).Replace("\\", "/");
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
                            NewImageName = NewName + "." + Splittype[type],
                            Path = folderName.Contains("wwwroot/") ? SetPath : "",
                            fullPath = PathToSaveDb,
                            sizes = list
                        });
                        Upload.Uploadimage(file, userid, SetPath = null, NewName);
                    }
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// ลบรูป
        /// </summary>
        /// <param name="files">ชื่อรูป</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("RemoveImage")]
        public ActionResult RemoveImage(List<string> files)
        {
            try
            {
                if (files == null)
                {
                    return BadRequest("The picture is not specified.");
                }
                return Ok(Upload.removeImage(files));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// อัพโหลดไฟล์
        /// </summary>
        /// <param name="files">ไฟล์ที่อัพโหลด</param>
        /// <param name="SetPath">ที่เก็บไฟล์</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("Uploadfile")]
        public ActionResult<m_uploadfile> Uploadfile(List<IFormFile> files, string SetPath)
        {
            try
            {
                var userid = UserLoginId;
                if (SetPath != null && files.Count() == 0)
                {
                    return BadRequest("The File is not uploaded.");
                }
                if (files.Count() == 0 && SetPath == null)
                {
                    return BadRequest("The File is not uploaded.");
                }
                if (SetPath == null)
                {
                    return Ok(Upload.UploadFile(files, userid, SetPath = null));
                }
                else
                {
                    return Ok(Upload.UploadFile(files, userid, SetPath));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// ลบไฟล์
        /// </summary>
        /// <param name="files">ชื่อไฟล์</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("RemoveFile")]
        public ActionResult RemoveFile(List<string> files)
        {
            try
            {
                if (files == null)
                {
                    return BadRequest("The picture is not specified.");
                }
                return Ok(Upload.removeFiles(files));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}

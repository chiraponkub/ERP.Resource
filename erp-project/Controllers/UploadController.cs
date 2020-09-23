using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="file">ไฟล์รูปภาพที่อัพโหลด</param>
        /// <param name="SetPath">ที่เก็บไฟล์</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Uploadimg")]
        public ActionResult<m_uploadimage> Uploadimage(List<IFormFile> file, string SetPath)
        {
            try
            {
                Guid req = Guid.NewGuid();
                if (SetPath != null && file.Count() == 0)
                {
                    return BadRequest("The image is not uploaded.");
                }
                if (file.Count() == 0 && SetPath == null)
                {
                    return BadRequest("The image is not uploaded.");
                }
                if (SetPath == null)
                {
                    return Ok(Upload.Uploadimage(file, req, SetPath = null));
                }
                else
                {
                    return Ok(Upload.Uploadimage(file, req, SetPath));
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
        /// <param name="file">ชื่อรูป</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("RemoveImage")]
        public ActionResult RemoveImage(List<string> file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("The picture is not specified.");
                }
                return Ok(Upload.removeImage(file));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// อัพโหลดไฟล์
        /// </summary>
        /// <param name="file">ไฟล์ที่อัพโหลด</param>
        /// <param name="SetPath">ที่เก็บไฟล์</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Uploadfile")]
        public ActionResult<m_uploadfile> Uploadfile(List<IFormFile> file, string SetPath)
        {
            try
            {
                Guid req = Guid.NewGuid();
                if (SetPath != null && file.Count() == 0)
                {
                    return BadRequest("The File is not uploaded.");
                }
                if (file.Count() == 0 && SetPath == null)
                {
                    return BadRequest("The File is not uploaded.");
                }
                if (SetPath == null)
                {
                    return Ok(Upload.UploadFile(file, req, SetPath = null));
                }
                else
                {
                    return Ok(Upload.UploadFile(file, req, SetPath));
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
        /// <param name="file">ชื่อไฟล์</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("RemoveFile")]
        public ActionResult RemoveFile(List<string> file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("The picture is not specified.");
                }
                return Ok(Upload.removeFiles(file));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

    }
}

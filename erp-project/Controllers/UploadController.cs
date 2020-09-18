using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public UploadController(IUpload _upload)
        {
            Upload = _upload;
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
                    return BadRequest("File not selected.");
                }
                return Ok(Upload.removefiles(file));
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
                    return BadRequest("The image is not uploaded.");
                }
                if (file.Count() == 0 && SetPath == null)
                {
                    return BadRequest("The image is not uploaded.");
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





    }
}

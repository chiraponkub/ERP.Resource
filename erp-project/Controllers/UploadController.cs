using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using erp_project.Libraries.Abstracts;
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
        /// อัพโหลดรูปภาพ
        /// </summary>
        /// <param name="file">ไฟล์ที่อัพรูปภาพ</param>
        /// <param name="id">ไอดีของผู้อัพรูปภาพ</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Uploadimg")]
        public ActionResult<m_uploadimage> Uploadimage(IFormFile file, int id)
        {
            if (file == null)
            {
                return BadRequest("The image is not uploaded.");
            }
            return Ok(Upload.Uploadimage(file, id));
        }

        /// <summary>
        /// อัพโหลดไฟล์
        /// </summary>
        /// <param name="file">ไฟล์ที่อัพโหลด</param>
        /// <param name="id">ไอดีของผู้อัพโหลด</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Uploadfile")]
        public ActionResult<m_uploadfile> Uploadfile(IFormFile file, int id)
        {
            if (file == null)
            {
                return BadRequest("The file is not upload.");
            }
            return Ok(Upload.UploadFile(file, id));
        }





    }
}

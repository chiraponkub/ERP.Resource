using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static erp_project.Libraries.Models.m_Upload;

namespace erp_project.Libraries.Abstracts
{
    public interface IUpload
    {
        m_uploadfile UploadFile(IFormFile file, int id);
        m_uploadimage Uploadimage(IFormFile file, int id);

    }
}

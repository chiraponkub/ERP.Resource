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
        // upload
        List<m_uploadfile> UploadFile(List<IFormFile> files, Guid id, string SetPath);
        List<m_uploadimage> Uploadimage(List<IFormFile> files, Guid id, string SetPath);

        //remove
        object removefiles(List<string> files);

    }
}

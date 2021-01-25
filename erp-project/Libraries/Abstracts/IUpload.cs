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
        List<m_uploadfile> UploadFile(List<IFormFile> files,string UserLoginId, string SetPath);
        void Uploadimage(IFormFile file, string NewName, string SetPath, string userid);

        //remove
        object removeFiles(List<string> files);
        object removeImage(List<string> files);

        //Get
        List<m_getupload> Get();


        // Test
        //void TEST(IFormFile file, string NewName, string SetPath, string userid);
        //void TEST(List<IFormFile> files, string NewName);
    }
}

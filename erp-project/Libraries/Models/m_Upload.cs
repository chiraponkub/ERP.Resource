using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace erp_project.Libraries.Models
{
    public class m_Upload
    {
        public class m_uploadimage
        {
            /// <summary>
            /// ชื่อเดิมของรูปภาพ
            /// </summary>
            public string OriginalName { get; set; }

            /// <summary>
            /// ชื่อใหม่ของรูปภาพ
            /// </summary>
            public string NewImageName { get; set; }

            /// <summary>
            /// ที่เก็บรูปภาพ
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// ที่เก็บรูปภาพพร้อมชื่อรูปภาพ
            /// </summary>
            public string fullPath { get; set; }

            /// <summary>
            /// ขนาดต่างๆ ของรูปภาพ
            /// </summary>
            public List<string> sizes { get; set; }
        }

        public class list_images
        {
            public List<string> size { get; set; }
        }

        public class m_getupload 
        {
            /// <summary>
            /// ชื่อไฟล์
            /// </summary>
            public string fileName { get; set; }
            /// <summary>
            /// รูปภาพ
            /// </summary>
            public string fullPath { get; set; }

            /// <summary>
            /// ชนิดของไฟล์
            /// </summary>
            public string type { get; set; }
        }

        public class m_uploadfile
        {
            /// <summary>
            /// ชื่อเดิมของไฟล์
            /// </summary>
            public string OriginalName { get; set; }

            /// <summary>
            /// ชื่อใหม่ของไฟล์
            /// </summary>
            public string NewFilename { get; set; }
            
            /// <summary>
            /// ที่เก็บไฟล์
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// ที่เก็บไฟล์พร้อมชื่อไฟล์
            /// </summary>
            public string FullPath { get; set; }

            /// <summary>
            /// ชนิดของไฟล์
            /// </summary>
            public string Type { get; set; }

            public long FileSize { get; set; }
        }
    }
}

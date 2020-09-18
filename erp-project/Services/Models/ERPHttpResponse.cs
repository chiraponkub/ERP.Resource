namespace erp_project.Services.Models
{
    /// <summary>
    /// ส่วนนี้เป็นของ ERP ที่หากว่า Http Service ส่ง Request ไปหา service อื่นๆจะ Response ค่ามาตามนี้
    /// </summary>
    public class ERPHttpResponse<T>
    {
        /// <summary>
        /// ข้อความที่ Server ปลายทางส่งมา
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// ข้อมูลที่ Server ปลายทางส่งมา
        /// </summary>
        public T data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using erp_project.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace erp_project.Middlewares
{
    /// <summary>
    /// ดักจับ Error ทั้งหมดของระบบ ก่อนจะ Response ออกไป
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            try
            {
                await _next(httpContext);
                switch ((HttpStatusCode)httpContext.Response.StatusCode)
                {
                    // หากหาหน้าไม่เจอแล้วอยู่ใน Development Mode จะ Redirect ไปหน้า Swagger help
                    case HttpStatusCode.NotFound:
                        if (_environment.IsDevelopment())
                        {
                            var Path = httpContext.Request.Path;
                            var ProxyPath = !string.IsNullOrEmpty(HelperConfig.ProxyPath) ? $"/{HelperConfig.ProxyPath}" : "/";
                            if (Path.StartsWithSegments(ProxyPath))
                            {
                                httpContext.Response.Redirect($"{ProxyPath}help/index.html");
                            }
                        }
                        break;
                    case HttpStatusCode.Forbidden:
                        httpStatusCode = HttpStatusCode.Forbidden;
                        throw new Exception("Forbidden");
                    case HttpStatusCode.Unauthorized:
                        httpStatusCode = HttpStatusCode.Unauthorized;
                        throw new Exception("Unauthorized");
                }
            }
            // หากเกิดข้อผิดพลาดอื่นๆ Response Message ออกมาหากเป็น Development mode จะมีรายละเอียดเพิ่มเติมมาด้วย
            catch (Exception ex)
            {
                string responseMessage = JsonSerializer.Serialize(new
                {
                    message = ex.ErrorMessage(),
                    exception = _environment.IsDevelopment() ? new { ex.Source, ex.StackTrace, } : null
                });
                httpContext.Response.StatusCode = (int)httpStatusCode;
                httpContext.Response.ContentType = "application/json";
                if (ex.Message.Equals("Unauthorized")) httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsync(responseMessage);
            }
        }
    }
}

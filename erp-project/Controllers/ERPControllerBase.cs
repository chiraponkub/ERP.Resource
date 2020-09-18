﻿using erp_project.Services.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Net;
using System.Text.Json;
using erp_project.Library.Concretes;
using erp_project.Services.Abstracts;
using System.Linq;
using erp_project.Services.Models;

namespace erp_project.Controllers
{
    /// <summary>
    /// ส่วนนี้เป็น Class กลางของ Controller ให้ Controller ทั้งหมด inherite (extends) ตัวนี้
    /// </summary>
    public class ERPControllerBase : ControllerBase
    {
        /// <summary>
        /// สำหรับยิงข้อมูลไปหา Server API อื่นๆเหมือน Front-End
        /// </summary>
        protected readonly IHttpApiService HttpService;

        /// <summary>
        /// สำหรับดึงข้อมูลจากไฟล์ appsetting.json
        /// </summary>
        protected readonly IConfiguration Configuration;

        /// <summary>
        /// สำหรับการเข้ารหัสด้วย Json Web Token
        /// </summary>
        protected readonly IJwtSecurityService JwtService;

        /// <summary>
        /// สำหรับ Hash และ compare ข้อมูลต่างๆ
        /// </summary>
        protected readonly IHashSerucityService hashService;

        public ERPControllerBase()
        {
            HttpService = new HttpApiService();
            JwtService = new JwtSecurityService();
            hashService = new HashSerucityService();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false);
            configurationBuilder.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true);
            configurationBuilder.AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();
        }

        /// <summary>
        /// ดึงข้อมูลผู้ใช้งานที่เข้าสู่ระบบจาก HTTP Service 
        /// จำเป็นจะต้องกำหนดค่า ApiUrls:AuthenURL ในไฟล์ appsettings.{environtment}.json ก่อน
        /// </summary>
        protected T GetUserLogin<T>()
        {
            HttpService.Authorization(UserAuthorization);
            var authenURL = Configuration.GetValue<string>("ApiUrls:AuthenURL");
            var response = HttpService.Get<ERPHttpResponse<T>>(authenURL).Result;
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception(response.StatusCode.ToString());
            return response.Content.data;
        }

        /// <summary>
        /// ดึงข้อมูล Authorization ที่ Client ส่งมายืนยันตัวตน
        /// </summary>
        protected string UserAuthorization => Request.Headers.Count(m => m.Key.Equals("Authorization")) > 0 ? Request.Headers["Authorization"].ToString() : "";

        /// <summary>
        /// ไอดีผู้ใช้งานที่เข้าสู่ระบบ
        /// </summary>
        protected string UserLoginId => User.FindFirstValue(ClaimTypes.Sid);

        /// <summary>
        /// สิทธิ์ผู้ใช้งานที่เข้าสู่ระบบ
        /// </summary>
        protected string UserLoginRole => User.FindFirstValue(ClaimTypes.Role);

        /// <summary>
        /// ส่งข้อมูลพร้อมกับ Token
        /// </summary>
        protected OkObjectResult OkAuthentication(string message, string userID, string userRole)
        {
            return new OkObjectResult(new
            {
                message,
                token = JwtService.GenerateJWTAuthentication(userID, userRole)
            });
        }

        /// <summary>
        /// Response ค่าที่สำเร็จออกไป (สนใจข้อความเป็นหลัก)
        /// </summary>
        protected OkObjectResult Ok(string message, object data = null)
        {
            return base.Ok(ResponseResult(message, data));
        }

        /// <summary>
        /// Response ค่าที่สำเร็จออกไป (สนใจข้อมูลเป็นหลัก)
        /// </summary>
        protected OkObjectResult Ok(object data, string message = "Ok")
        {
            return base.Ok(ResponseResult(message, data));
        }

        /// <summary>
        /// Response ค่าที่ล้มเหลวออกไป (สนใจข้อความเป็นหลัก)
        /// </summary>
        protected BadRequestObjectResult BadRequest(string message, object data = null)
        {
            return base.BadRequest(ResponseResult(message, data));
        }

        /// <summary>
        /// Response ค่าที่ล้มเหลวออกไป (สนใจข้อมูลเป็นหลัก)
        /// </summary>
        protected BadRequestObjectResult BadRequest(object data = null, string message = "BadRequest")
        {
            return base.BadRequest(ResponseResult(message, data));
        }

        /// <summary>
        /// เปลี่ยนค่าใหม่ให้อยู่ในรูปแบบที่กำหนด
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, object> ResponseResult(string message, object data)
        {
            var values = new Dictionary<string, object>
            {
                { "message", message },
                { "data", data }
            };

            if (User.Identity.IsAuthenticated)
            {
                values.Add("token", JwtService.GenerateJWTAuthentication(UserLoginId.ToString(), UserLoginRole));
            }
            return values;
        }
    }
}

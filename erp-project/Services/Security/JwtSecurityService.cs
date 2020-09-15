using erp_project.Services.Abstracts;
using erp_project.Services.Models;
using Jose;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace erp_project.Services.Security
{
    /// <summary>
    /// ส่วนสำหรับใช้งาน JWT (Json Web Token)
    /// </summary>
    public class JwtSecurityService : IJwtSecurityService
    {
        /// <summary>
        /// กำหนด Security Key เพื่อเข้ารหัสกับ Json Web Token
        /// </summary>
        public const string SECRET_KEY = "XkVudGVycHJpc2UtUmVzb3VyY2UtUGxhbm5pbmdAOVRfQXZlc3RhJD89";
        public const string ISSUER = "https://9t.com/";
        public const string AUDIENCE = "https://9t.com/";

        public string JWTEncode<T>(T data, int minute)
        {
            try
            {
                var payload = new JwtPayload<T>
                {
                    Data = data,
                    Expire = DateTime.UtcNow.AddMinutes(minute).Ticks
                };
                return JWT.Encode(payload, Encoding.ASCII.GetBytes(SECRET_KEY), JwsAlgorithm.HS256);
            }
            catch
            {
                return null;
            }
        }

        public T JWTDecode<T>(string token)
        {
            try
            {
                var payload = JWT.Decode<JwtPayload<T>>(token, Encoding.ASCII.GetBytes(SECRET_KEY), JwsAlgorithm.HS256);
                if (payload.Expire < DateTime.UtcNow.Ticks) throw new Exception("Token is exprise");
                return payload.Data;
            }
            catch
            {
                return default;
            }
        }

        public string GenerateJWTAuthentication(string id, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Sid, id),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer: ISSUER,
                audience: AUDIENCE,
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using erp_project.Services.Abstracts;
using System;
using System.Security.Cryptography;

namespace erp_project.Services.Security
{
    /// <summary>
    /// ส่วนของการเข้ารหัสข้อมูลที่ไม่สามารถถอดรหัสได้
    /// </summary>
    public class HashSerucityService : IHashSerucityService
    {
        private const int SALT_SIZE = 24;
        private const int NUM_ITERATIONS = 1000;
        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string PasswordHash(string password)
        {
            byte[] buf = new byte[SALT_SIZE];
            rng.GetBytes(buf);
            string salt = Convert.ToBase64String(buf);

            Rfc2898DeriveBytes deriver2898 = new Rfc2898DeriveBytes(password.Trim(), buf, NUM_ITERATIONS);
            string hash = Convert.ToBase64String(deriver2898.GetBytes(16));
            return salt + ':' + hash;
        }

        public bool PasswordVerify(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false;
            byte[] buf = Convert.FromBase64String(parts[0]);
            Rfc2898DeriveBytes deriver2898 = new Rfc2898DeriveBytes(password.Trim(), buf, NUM_ITERATIONS);
            string computedHash = Convert.ToBase64String(deriver2898.GetBytes(16));
            return parts[1].Equals(computedHash);
        }
    }
}

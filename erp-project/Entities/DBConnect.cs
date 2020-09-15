using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace erp_project.Entities
{
    /// <summary>
    /// สำหรับเชื่อมต่อและ Setting Database
    /// </summary>
    public partial class DBConnect : DbContext
    {
        /// <summary>
        /// สำหรับค้นหาตัวแปรของ appsetting.json
        /// </summary>
        public readonly IConfiguration Configuration;

        public DBConnect(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// ตั้งค่าเพิ่มเติมเมื่อเกิด Event การสร้าง Model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        /// <summary>
        /// ตั้งค่าการเชื่อมต่อ Connection String ของ Database
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("ConnectDatabase"));
                //optionsBuilder.UseSqlServer("Data Source=10.50.41.12;Initial Catalog=erp_dev;Persist Security Info=True;User ID=sa;Password=Addlink123!");
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
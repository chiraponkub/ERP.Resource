using erp_project.Entities.Tables;
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
            #region ต่อ DB ครั้งแรก 17/09/2020.
            modelBuilder.Entity<Upload>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__tmp_ms_x__72E12F1ACB6FB4F5");

                entity.ToTable("Upload", "Resource");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(13);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.FullPath)
                    .IsRequired()
                    .HasColumnName("fullPath")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(30);

                entity.Property(e => e.UserId).HasColumnName("userID");
            });
            #endregion 
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
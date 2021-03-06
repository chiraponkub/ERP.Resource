using erp_project.Entities.Tables;
using Microsoft.EntityFrameworkCore;

namespace erp_project.Entities
{
    /// <summary>
    /// สำหรับกำหนด Model ที่จะมาทำ DbSet ไว้ที่นี่
    /// </summary>
    public partial class DBConnect : DbContext
    {
        public virtual DbSet<Upload> Upload { get; set; }
    }
}

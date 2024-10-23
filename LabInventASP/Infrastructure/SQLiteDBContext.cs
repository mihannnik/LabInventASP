using LabInventASP.Models;
using Microsoft.EntityFrameworkCore;

namespace LabInventASP.Infrastructure
{
    public class SQLiteDBContext : DbContext
    {
        public DbSet<DeviceStatus> DeviceStatuses { get; set; }
        public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=Database.sqlite");
        }
    }
}

using LabInventASP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LabInventASP.Infrastructure
{
    public class SQLiteDBContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<DeviceStatus> DeviceStatuses { get; set; }
        public SQLiteDBContext(IConfiguration configuration, DbContextOptions<SQLiteDBContext> options) : base(options)
        {
            _configuration = configuration;
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_configuration["Database:DataSource"]}");
        }
    }
}

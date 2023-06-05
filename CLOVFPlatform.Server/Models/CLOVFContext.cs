using System;
using Microsoft.EntityFrameworkCore;

namespace CLOVFPlatform.Server.Models
{
	/// <summary>
	/// CLOVF DB Context
	/// </summary>
	public class CLOVFContext : DbContext
    {
		/// <summary>
		/// 직원 정보
		/// </summary>
		public DbSet<Employee> Employee { get; set; }

		public CLOVFContext()
		{
		}

		public CLOVFContext(DbContextOptions<CLOVFContext> options) : base(options)
		{

		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=clovf.db"); // SQLite 데이터베이스 경로 및 파일명 지정
        }
    }
}


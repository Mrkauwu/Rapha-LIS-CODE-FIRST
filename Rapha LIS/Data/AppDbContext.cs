using Microsoft.EntityFrameworkCore;
using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<PatientModel> Patients { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TestModel> Test { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientModel>(entity =>
            {
                // Set default value for DateCreated
                entity.Property(e => e.DateCreated)
                      .HasDefaultValueSql("GETDATE()");

                // Computed column for BarcodeID
                entity.Property(e => e.BarcodeID)
                      .HasComputedColumnSql(
                          "'LAB-' + CAST(YEAR([DateCreated]) AS NVARCHAR) + 'W' + RIGHT('0' + CAST(DATEPART(WEEK, [DateCreated]) AS NVARCHAR), 2) + '-' + RIGHT('00000' + CAST([Id] AS NVARCHAR), 5)",
                          stored: false // or true if you want it persisted in the table
                      );
            });
        }
    }
}

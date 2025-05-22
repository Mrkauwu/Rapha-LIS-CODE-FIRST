using Microsoft.EntityFrameworkCore;
using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();

            // Seed Admin User if none exist
            if (!context.Users.Any())
            {
                var adminUser = new UserModel
                {
                    Name = "admin", // or "Admin" if you want
                    Age = 18,
                    Sex = null,
                    Username = "admin",
                    Password = "admin2025",
                    DateCreated = DateTime.Now
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            // Seed Tests table if empty
            if (!context.Test.Any())
            {
                var tests = new TestModel[]
                {
                    new TestModel { Test = "Hemoglobin", NormalValue = "13.5-17.5 g/dL" },
                    new TestModel { Test = "White Blood Cells", NormalValue = "4,000-11,000 cells/mcL" },
                    new TestModel { Test = "Platelets", NormalValue = "150,000-450,000/mcL" }
                };

                context.Test.AddRange(tests);
                context.SaveChanges();
            }
        }
    }
}

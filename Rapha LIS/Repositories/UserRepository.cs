
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVP_LEARNING.Repositories;
using Rapha_LIS.Data;
using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Repositories
{
    public class UserRepository : BaseRepository, IUserControlRepository, ISigninRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public void DeleteUser(int Id)
        {
            var user = _context.Users.Find(Id); // Corrected 'id' to 'Id' to match the method parameter
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }



        public List<UserModel> GetAll()
        {
            return _context.Users.OrderByDescending(u => u.DateCreated).ToList();
        }

        public List<FilteredUserModel> GetFilteredName()
        {
            return _context.Users
                .OrderByDescending(u => u.DateCreated)
                .Select(u => new FilteredUserModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Age = u.Age,
                    Sex = u.Sex,
                    DateCreated = u.DateCreated
                })
                .ToList();
        }

        public List<FilteredUserModel> GetByFilteredName(string value)
        {
            int id = int.TryParse(value, out var parsedId) ? parsedId : 0;

            return _context.Users
                .Where(u => u.Id == id || u.Name.StartsWith(value))
                .OrderByDescending(u => u.DateCreated)
                .Select(u => new FilteredUserModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Age = u.Age,
                    Sex = u.Sex,
                    DateCreated = u.DateCreated
                })
                .ToList();
        }

        // Validate user login
        public string? ValidateUser(string username, string password)
        {
            // Find the user by username (case-sensitive or insensitive depending on your DB collation)
            var user = _context.Users
                .AsNoTracking()   // No need to track for read-only
                .FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                string storedPassword = user.Password?.Trim() ?? "";
                string fullName = user.Name ?? "";

                if (storedPassword == password.Trim())
                {
                    return fullName; // Successful login, return full name
                }
            }

            return null; // Login failed
        }

        public int InsertEmptyUser()
        {
            var user = new UserModel
            {
                Name = "",
                Age = null,
                Sex = "",
                Username = "",
                Password = "",
                DateCreated = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user.Id;  // EF automatically populates the ID after save
        }

        public void SaveOrUpdateUser(UserModel user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser != null)
            {
                // Update existing user
                existingUser.Name = user.Name ?? "";
                existingUser.Age = user.Age;
                existingUser.Sex = user.Sex ?? "";
                existingUser.Username = user.Username ?? "";
                existingUser.Password = user.Password ?? "";
                existingUser.DateCreated = DateTime.Now; // Or DateModified if you track update time
            }
            else
            {
                // Insert new user
                user.DateCreated = DateTime.Now;
                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }

    }
}

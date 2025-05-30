﻿
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

        public void DeleteUser(List<int> ids)
        {
            var user = _context.Users.Where(p => ids.Contains(p.Id)).ToList();
            if (user.Any())
            {
                _context.Users.RemoveRange(user);
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
        public (string? Name, string? Role) ValidateUser(string username, string password)
        {
            var user = _context.Users
        .AsNoTracking()
        .FirstOrDefault(u => u.Username == username);

            if (user != null && user.Password?.Trim() == password.Trim())
            {
                return (user.Name, user.Role);
            }

            return (null, null);
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
                DateCreated = DateTime.Now,
                Role = "User"
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
                existingUser.DateCreated = DateTime.Now; //Modify Later to Last Modified
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

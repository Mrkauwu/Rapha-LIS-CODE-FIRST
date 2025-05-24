using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Sex { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

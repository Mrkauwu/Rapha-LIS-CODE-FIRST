using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public string? Test { get; set; }
        public string? NormalValue { get; set; }

        public override string ToString()
        {
            return Test ?? string.Empty; // Ensures a non-null value is returned  
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public class LeukocytesModel
    {
        public int Id { get; set; }
        public string? Leukocytes { get; set; }
        public string? LeukocytesNormalValue { get; set; }

        public override string ToString()
        {
            return Leukocytes ?? string.Empty; // Ensures a non-null value is returned  
        }
    }
}

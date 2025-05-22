using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public class FilteredPatientModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Sex { get; set; }
        public string? Physician { get; set; }
        public string? MedTech { get; set; }
        public string? Test { get; set; }
        public string? TestResult { get; set; }
        public string? NormalValue { get; set; }
        public string? Leukocytes { get; set; }
        public string? LeukocytesResult { get; set; }
        public string? LeukocytesNormalValue { get; set; }
        public DateTime DateCreated { get; set; }
        public string? BarcodeID { get; set; }
    }
}

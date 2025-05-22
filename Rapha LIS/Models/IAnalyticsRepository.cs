using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public interface IAnalyticsRepository
    {
        PatientModel? GetPatientByHRI(string id);
        Task UpdateExaminer(string barcodeID, string examinerName);
        List<PatientModel> GetPatientHRI(string examinerName);
        
    }
}

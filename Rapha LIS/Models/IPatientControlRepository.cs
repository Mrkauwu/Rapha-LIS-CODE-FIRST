using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public interface IPatientControlRepository
    {
        void AddPatient(PatientModel patientModel);
        void DeletePatient(List<int> ids);
        List<PatientModel> GetAll();
        List<FilteredPatientModel> GetFilteredName();
        List<FilteredPatientModel> GetByFilteredName(string value);
        int InsertEmptyPatient();
        void SaveOrUpdatePatient(PatientModel patient);
        List<PatientModel> GetBarcodeIdsByPatientIds(List<int> patientIds);

    }
}

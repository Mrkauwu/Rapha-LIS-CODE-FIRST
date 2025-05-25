
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using MVP_LEARNING.Repositories;
using Rapha_LIS.Data;
using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Document.NET;

namespace Rapha_LIS.Repositories
{
    public class PatientRepository : BaseRepository, IPatientControlRepository, IAnalyticsRepository, ITestListRepository, ILeukocytesListRepository, IDashboardRepository
    {
        private readonly AppDbContext _context;
        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        //Analytics

        public async Task UpdateExaminer(string barcodeID, string examinerName)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.BarcodeID == barcodeID);
            if (patient != null)
            {
                patient.MedTech = examinerName;
                await _context.SaveChangesAsync();
            }
            else
            {
                MessageBox.Show("Failed to update patient record.");
            }
        }

        public PatientModel? GetPatientByHRI(string id)
        {
            var patient = _context.Patients
                .AsNoTracking() // no tracking for read-only
                .FirstOrDefault(p => p.BarcodeID == id);

            return patient;
        }

        public List<PatientModel> GetPatientHRI(string examinerName)
        {
            // Using EF Core LINQ to query Patients where MedTech matches examinerName
            var patientHRI = _context.Patients
                .AsNoTracking()          // Read-only, no tracking needed
                .Where(p => p.MedTech == examinerName)
                .ToList();

            return patientHRI;
        }

        public void AddPatient(PatientModel patientModel)
        {
            _context.Patients.Add(patientModel);
            _context.SaveChanges(); // Saves changes to the database
        }

        public List<FilteredPatientModel> GetFilteredName()
        {
            return _context.Patients
                .OrderByDescending(p => p.DateCreated)
                .Select(p => new FilteredPatientModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Age = p.Age,
                    Sex = p.Sex,
                    Physician = p.Physician,
                    MedTech = p.MedTech,
                    Test = p.Test,
                    TestResult = p.TestResult,
                    NormalValue = p.NormalValue,
                    Leukocytes = p.Leukocytes,
                    LeukocytesResult = p.LeukocytesResult,
                    LeukocytesNormalValue = p.LeukocytesNormalValue,
                    DateCreated = p.DateCreated
                }).ToList();
        }


        public List<FilteredPatientModel> GetByFilteredName(string value)
        {
            int.TryParse(value, out int id);
            var query = _context.Patients.AsQueryable();

            if (id > 0)
            {
                query = query.Where(p => p.Id == id);
            }
            else
            {
                query = query.Where(p => p.Name.StartsWith(value));
            }

            return query.OrderByDescending(p => p.DateCreated)
                        .Select(p => new FilteredPatientModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Age = p.Age,
                            Sex = p.Sex,
                            Physician = p.Physician,
                            MedTech = p.MedTech,
                            Test = p.Test,
                            TestResult = p.TestResult,
                            NormalValue = p.NormalValue,
                            Leukocytes = p.Leukocytes,
                            LeukocytesResult = p.LeukocytesResult,
                            LeukocytesNormalValue = p.LeukocytesNormalValue,
                            DateCreated = p.DateCreated
                        }).ToList();
        }


        public List<PatientModel> GetAll()
        {
            return _context.Patients
                .OrderByDescending(p => p.DateCreated) // Order by DateCreated (descending)
                .AsNoTracking() // Improves performance for read-only queries\
                .Take(50)
                .ToList();
        }

        public int InsertEmptyPatient()
        {
            var patient = new PatientModel
            {
                Name = "",
                Age = null,
                Sex = "",
                Physician = "",
                MedTech = "",
                Test = "",
                TestResult = "",
                NormalValue = "",
                Leukocytes = "",
                LeukocytesResult = "",
                LeukocytesNormalValue = "",
                DateCreated = DateTime.Now
            };

            _context.Patients.Add(patient);
            _context.SaveChanges();
            return patient.Id;
        }


        public void SaveOrUpdatePatient(PatientModel patient)
        {
            var existing = _context.Patients.FirstOrDefault(p => p.Id == patient.Id);
            if (existing != null)
            {
                existing.Name = patient.Name;
                existing.Age = patient.Age;
                existing.Sex = patient.Sex;
                existing.Physician = patient.Physician;
                existing.MedTech = patient.MedTech;
                existing.Test = patient.Test;
                existing.TestResult = patient.TestResult;
                existing.NormalValue = patient.NormalValue;
                existing.Leukocytes = patient.Leukocytes;
                existing.LeukocytesResult = patient.LeukocytesResult;
                existing.LeukocytesNormalValue = patient.LeukocytesNormalValue;
                existing.DateCreated = DateTime.Now;
            }
            else
            {
                patient.DateCreated = DateTime.Now;
                _context.Patients.Add(patient);
            }

            _context.SaveChanges();
        }


        public void DeletePatient(List<int> ids)
        {
            var patients = _context.Patients.Where(p => ids.Contains(p.Id)).ToList();

            if (patients.Any())
            {
                _context.Patients.RemoveRange(patients);
                _context.SaveChanges();
            }
        }

        public IEnumerable<TestModel> GetAllTests()
        {
            return _context.Test.ToList();
        }

        public List<PatientModel> GetBarcodeIdsByPatientIds(List<int> patientIds)
        {
            return _context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .OrderBy(p => p.DateCreated) // oldest to newest
                .Select(p => new PatientModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Age = p.Age ?? 0,
                    BarcodeID = p.BarcodeID,
                    DateCreated = p.DateCreated
                })
                .ToList();
        }

        public List<PatientModel> PrintPatientResult(List<int> patientIds)
        {
            return _context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .OrderBy(p => p.DateCreated) // oldest to newest
                .Select(p => new PatientModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Age = p.Age ?? 0,
                    Sex = p.Sex,
                    Physician = p.Physician,
                    MedTech = p.MedTech,
                    Test = p.Test,
                    TestResult = p.TestResult,
                    NormalValue = p.NormalValue,
                    Leukocytes = p.Leukocytes,
                    LeukocytesResult = p.LeukocytesResult,
                    LeukocytesNormalValue = p.LeukocytesNormalValue,
                    DateCreated = p.DateCreated
                })
                .ToList();
        }

        public IEnumerable<LeukocytesModel> GetAllLeukocytes()
        {
            return _context.Leukocytes.ToList();
        }

        public List<PatientModel> GetSome()
        {
            return _context.Patients
                .OrderByDescending(p => p.DateCreated) // Order by DateCreated (descending)
                .AsNoTracking() // Improves performance for read-only queries\
                .Take(50)
                .ToList();
        }
    }

    static class SqlDataReaderExtensions
    {
        public static string GetStringOrEmpty(this SqlDataReader reader, string columnName)
            => reader[columnName] == DBNull.Value ? "" : reader[columnName].ToString();

        public static string? GetNullableString(this SqlDataReader reader, string columnName)
            => reader[columnName] == DBNull.Value ? null : reader[columnName].ToString();

        public static int? GetNullableInt(this SqlDataReader reader, string columnName)
            => reader[columnName] == DBNull.Value ? (int?)null : Convert.ToInt32(reader[columnName]);
    }



}


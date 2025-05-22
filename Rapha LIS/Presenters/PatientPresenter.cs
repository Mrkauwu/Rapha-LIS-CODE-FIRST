using Rapha_LIS.Models;
using Rapha_LIS.Repositories;
using Rapha_LIS.Views;
using Rapha_LIS.Views.TListEventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Words.NET;
using static MaterialSkin.Controls.MaterialCheckedListBox;
using static System.Net.Mime.MediaTypeNames;

namespace Rapha_LIS.Presenters
{
    public class PatientPresenter
    {
        //Analytics
        private readonly IPatientAnalyticsView patientAnalyticsView;
        private readonly IAnalyticsRepository analyticsRepository;
        private BindingSource analyticsBindingSource;
        private List<PatientModel> patientHRI;
        
        //Patient Control
        private readonly IPatientControlView patientView;
        private readonly IPatientControlRepository patientRepository;
        private readonly BindingSource PatientControlBindingSource;
        private List<FilteredPatientModel>? filteredPatientList;
        private List<PatientModel>? patientModel;




        //Test List
        private readonly ITestListView testList;
        private readonly ITestListRepository testListRepository;

        public PatientPresenter(IPatientControlView patientView, IPatientControlRepository patientRepository,
                                IPatientAnalyticsView patientAnalyticsView, IAnalyticsRepository analyticsRepository
                                , ITestListView testList, ITestListRepository testListRepository)
        {

            //TestList
            this.testList = testList ?? throw new ArgumentNullException(nameof(testList));
            this.testListRepository = testListRepository ?? throw new ArgumentNullException(nameof(testListRepository));

            testList.SearchTestRequested += TestList_SearchTestRequested;
            testList.SaveTestRequested += TestList_SaveTestRequested;

            //PatientControlView
            this.patientView = patientView ?? throw new ArgumentNullException(nameof(patientView));
            this.patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));

            //PatientControlView
            this.patientView.SearchRequestedByName += PatientView_SearchRequestedByName;
            this.patientView.AddPatientRequested += PatientView_AddRequested;
            this.patientView.DeletePatientRequested += PatientView_DeleteRequested;
            patientView.CellValueEdited += (s, e) => PatientView_CellValueEdited(null, e.RowIndex);
            patientView.OpenTestListRequested += PatientView_OpenTestListRequested;
            this.PatientControlBindingSource = new BindingSource();  // ✅ Initialize first
            this.patientView.BindPatientControlList(PatientControlBindingSource);  // ✅ Now it's not null


            //Analytics
            this.patientAnalyticsView = patientAnalyticsView ?? throw new ArgumentNullException(nameof(patientAnalyticsView));
            this.analyticsRepository = analyticsRepository ?? throw new ArgumentNullException(nameof(analyticsRepository));

            this.analyticsBindingSource = new BindingSource();
            
            patientHRI = analyticsRepository.GetPatientHRI(SigninPresenter.LoggedInUserFullName ?? "");
            analyticsBindingSource.DataSource = patientHRI;
            patientAnalyticsView.BindPatientAnalyticsList(analyticsBindingSource);

            this.patientAnalyticsView.SearchRequestedByHIR += PatientAnalyticsView_SearchRequestedByHIR;
            this.patientAnalyticsView.AnalyticsCellValueEdited += (s, e) => PatientAnalyticsView_CellValueEdited(null, e.RowIndex);

            LoadAllPatientList();
            LoadTestList();
        }

        private void TestList_SaveTestRequested(object? sender, EventArgs e)
        {
            var form = (Form)testList;
            form.DialogResult = DialogResult.OK;
            form.Close();
        }

        private void TestList_SearchTestRequested(object? sender, EventArgs e)
        {
            var searchTerm = (testList as TestListView)?.txtSearch.Text ?? "";
            testList.SetTestList(testListRepository.GetAllTests()
                .Where(t => t.Test.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        private void LoadTestList() => testList.SetTestList(testListRepository.GetAllTests());

        private void PatientAnalyticsView_CellValueEdited(object value, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= (patientHRI?.Count ?? 0)) return;

            try
            {
                patientRepository.SaveOrUpdatePatient(patientHRI[rowIndex]);
            }
            catch (Exception ex)
            {
                patientView.ShowMessage($"Auto-save failed: {ex.Message}", "Error");
            }
        }

        

        private void PatientView_CellValueEdited(object value, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= (patientModel?.Count ?? 0)) return;

            try
            {
                patientRepository.SaveOrUpdatePatient(patientModel[rowIndex]);
            }
            catch (Exception ex)
            {
                patientView.ShowMessage($"Auto-save failed: {ex.Message}", "Error");
            }
        }

        private void PatientView_OpenTestListRequested(object? sender, TestListEventArgs e)
        {
            if (((Form)testList).ShowDialog() == DialogResult.OK)
                patientView.UpdateRowWithSelectedTests(e.RowIndex, testList.SelectedTests);
        }

        private void LoadAllPatientList()
        {
            patientModel = patientRepository.GetAll().ToList();
            PatientControlBindingSource.DataSource = patientModel;
            
            patientHRI = analyticsRepository.GetPatientHRI(SigninPresenter.LoggedInUserFullName ?? "");
            analyticsBindingSource.DataSource = patientHRI;
        }




        

        //Analytics
        private void PatientAnalyticsView_SearchRequestedByHIR(object? sender, EventArgs e)
        {
            string inputId = patientAnalyticsView.SearchQueryByHIR.Trim();
            if (string.IsNullOrWhiteSpace(inputId))
            {
                patientView.ShowMessage("Please enter a Barcode ID.");
                return;
            }
            var patient = analyticsRepository.GetPatientByHRI(inputId);
            if (patient != null)
            {
                if (!string.IsNullOrEmpty(SigninPresenter.LoggedInUserFullName))
                {
                    analyticsRepository.UpdateExaminer(inputId, SigninPresenter.LoggedInUserFullName);
                }
                patientView.ShowMessage($"Patient {patient.Name} examined by {SigninPresenter.LoggedInUserFullName}");
            }
            else
            {
                patientView.ShowMessage("User not found!");
            }
            LoadAllPatientList();
        }



        //Patient Control View

        private void PatientView_AddRequested(object? sender, EventArgs e)
        {
            try
            {
                var newPatient = new PatientModel
                {
                    Id = patientRepository.InsertEmptyPatient(),
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

                patientModel?.Insert(0, newPatient);
                PatientControlBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                patientView.ShowMessage($"Add failed: {ex.Message}", "Error");
            }
        }

        private void PatientView_DeleteRequested(object? sender, EventArgs e)
        {
            var ids = patientView.SelectedPatient;
            if (!ids.Any())
            {
                patientView.ShowMessage("Please select at least one row to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete the selected record(s)?",
                    "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                patientRepository.DeletePatient(ids);
                patientView.ShowMessage("Selected record(s) deleted.");
                LoadAllPatientList();
            }
            catch (Exception ex)
            {
                patientView.ShowMessage($"Error: {ex.Message}", "Error");
            }
        }


        private void PatientView_SearchRequestedByName(object? sender, EventArgs e)
        {
            bool emptyValue = string.IsNullOrWhiteSpace(this.patientView.SearchQueryByName);
            if (emptyValue == false)
                filteredPatientList = patientRepository.GetByFilteredName(this.patientView.SearchQueryByName);
            else filteredPatientList = patientRepository.GetFilteredName();
            PatientControlBindingSource.DataSource = filteredPatientList;
        }
    }
}

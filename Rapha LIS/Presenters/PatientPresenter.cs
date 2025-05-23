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
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Words.NET;
using static MaterialSkin.Controls.MaterialCheckedListBox;
using static System.Net.Mime.MediaTypeNames;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using System.Drawing;
using ZXing.PDF417.Internal;
using Xceed.Document.NET;

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
        private int currentPrintIndex = 0;

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
            patientView.PrintBarcodeRequested += (s, e) => PatientView_PrintBarcodeRequested();
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
            patientAnalyticsView.PrintResultRequested += PatientAnalyticsView_PrintResultRequested;
            this.patientAnalyticsView.AnalyticsCellValueEdited += (s, e) => PatientAnalyticsView_CellValueEdited(null, e.RowIndex);

            LoadAllPatientList();
            LoadTestList();
        }

        private void PatientAnalyticsView_PrintResultRequested(object? sender, EventArgs e)
        {
            var selectedIds = patientAnalyticsView.SelectedToPrint;

            if (!selectedIds.Any())
            {
                patientView.ShowMessage("Please select at least one patient to print.");
                return;
            }

            var patientsToPrint = analyticsRepository.PrintPatientResult(selectedIds).ToList();

            if (patientsToPrint.Count == 0)
            {
                patientView.ShowMessage("No patients found for selected IDs.");
                return;
            }

            GeneratePatientReport(patientsToPrint);

        }

        private void GeneratePatientReport(List<PatientModel> patients)
        {
            string filePath = Path.Combine(Path.GetTempPath(), $"PatientReport_{DateTime.Now:yyyyMMddHHmmss}.docx");

            using (var doc = DocX.Create(filePath))
            {
                var logoImage = doc.AddImage(@"C:\Users\Mrka\Desktop\Software Design\Project Rapha\Rapha LIS FINAL\Images\LOGO.png");  // Change path
                var picture = logoImage.CreatePicture();
                var logoParagraph = doc.InsertParagraph();
                logoParagraph.AppendPicture(picture).Alignment = Alignment.center;

                doc.InsertParagraph("Rapha Diagnostic Laboratory")
                    .Font("Bell MT").FontSize(22).Bold().Alignment = Alignment.center;
                doc.InsertParagraph("\"Your Health is our Priority\"")
                    .Font("Arial").FontSize(12).Italic().Alignment = Alignment.center;
                doc.InsertParagraph("Kidapawan City")
                    .Font("Arial").FontSize(12).Alignment = Alignment.center;

                foreach (var patient in patients)
                {
                    doc.InsertParagraph("HEMATOLOGY")
                        .Font("Cambria").FontSize(22).Bold()
                        .Highlight(Xceed.Document.NET.Highlight.cyan)
                        .SpacingAfter(20).Alignment = Alignment.center;

                    var infoTable = doc.AddTable(2, 3);
                    infoTable.Design = TableDesign.None;
                    infoTable.Alignment = Alignment.center;
                    infoTable.AutoFit = AutoFit.Contents;

                    infoTable.Rows[0].Cells[0].Paragraphs[0].Append($"Name: {patient.Name}");
                    infoTable.Rows[0].Cells[1].Paragraphs[0].Append($"Age: {patient.Age}");
                    infoTable.Rows[0].Cells[2].Paragraphs[0].Append($"Date: {patient.DateCreated.ToShortDateString()}");

                    infoTable.Rows[1].Cells[0].Paragraphs[0].Append($"Address: {"Kidapawan City"}");
                    infoTable.Rows[1].Cells[1].Paragraphs[0].Append($"Sex: {patient.Sex}");
                    infoTable.Rows[1].Cells[2].Paragraphs[0].Append($"Physician: {patient.Physician}");

                    doc.InsertTable(infoTable);
                    doc.InsertParagraph().SpacingAfter(20);

                    var tests = System.Text.RegularExpressions.Regex
                        .Split((patient.Test ?? "").Trim(), @"\s{2,}|\r?\n")
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    var results = System.Text.RegularExpressions.Regex
                        .Split((patient.TestResult ?? "").Trim(), @"\s{2,}|\r?\n")
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    var normals = System.Text.RegularExpressions.Regex
                    .Split((patient.NormalValue ?? "").Trim(), @"\s{2,}")
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray();

                    var maxRows = new[] { tests.Length, results.Length, normals.Length }.Max();
                    var testTable = doc.AddTable(maxRows + 1, 3);
                    testTable.Alignment = Alignment.center;
                    testTable.Design = TableDesign.TableGrid;

                    testTable.Rows[0].Cells[0].Paragraphs[0].Append("Test").Bold();
                    testTable.Rows[0].Cells[1].Paragraphs[0].Append("Result").Bold();
                    testTable.Rows[0].Cells[2].Paragraphs[0].Append("Normal Value").Bold();

                    for (int i = 0; i < maxRows; i++)
                    {
                        testTable.Rows[i + 1].Cells[0].Paragraphs[0].Append(tests.ElementAtOrDefault(i)?.Trim() ?? "");
                        testTable.Rows[i + 1].Cells[1].Paragraphs[0].Append(results.ElementAtOrDefault(i)?.Trim() ?? "");
                        testTable.Rows[i + 1].Cells[2].Paragraphs[0].Append(normals.ElementAtOrDefault(i)?.Trim() ?? "");
                    }


                    doc.InsertTable(testTable);
                    doc.InsertParagraph().SpacingAfter(20);

                    var signTable = doc.AddTable(2, 2);
                    signTable.Alignment = Alignment.center;
                    signTable.Design = TableDesign.None;

                    signTable.Rows[0].Cells[0].Paragraphs[0].Append($"{"NENA SALCEDO LINGAYON, MD, FPSP"}").Alignment = Alignment.center;
                    signTable.Rows[0].Cells[1].Paragraphs[0].Append($"{patient.MedTech}").Alignment = Alignment.center;

                    signTable.Rows[1].Cells[0].Paragraphs[0].Append("Pathologist").Italic().Alignment = Alignment.center;
                    signTable.Rows[1].Cells[1].Paragraphs[0].Append("Medical Technologist").Italic().Alignment = Alignment.center;

                    doc.InsertTable(signTable);
                    doc.InsertParagraph().InsertPageBreakAfterSelf();
                }

                doc.Save();
            }

            // Open Word
            Process.Start(new ProcessStartInfo()
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }


        private void PatientView_PrintBarcodeRequested()
        {
            var selectedIds = patientView.SelectedPatient;

            if (!selectedIds.Any())
            {
                patientView.ShowMessage("Please select at least one patient to print.");
                return;
            }

            var patientsToPrint = patientRepository.GetBarcodeIdsByPatientIds(selectedIds).ToList();

            if (patientsToPrint.Count == 0)
            {
                patientView.ShowMessage("No patients found for selected IDs.");
                return;
            }

            currentPrintIndex = 0;  // Reset before printing

            PrintDocument doc = new PrintDocument();
            doc.DefaultPageSettings.PaperSize = new PaperSize("TestTubeLabel", 197, 98);
            doc.PrintPage += (s, args) =>
            {
                int y = 10;

                var writer = new ZXing.BarcodeWriter<System.Drawing.Bitmap>
                {
                    Format = ZXing.BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions { Width = 200, Height = 60 },
                    Renderer = new ZXing.Rendering.BitmapRenderer()
                };

                // Print only one patient per page
                var patient = patientsToPrint[currentPrintIndex];
                var barcodeValue = patient.BarcodeID ?? patient.Id.ToString();
                var barcodeImage = writer.Write(barcodeValue);

                args.Graphics.DrawString($"Name: {patient.Name}", new System.Drawing.Font("Arial", 10), Brushes.Black, 10, y);
                y += 20;
                args.Graphics.DrawString($"Age: {patient.Age}", new System.Drawing.Font("Arial", 10), Brushes.Black, 10, y);
                y += 20;
                args.Graphics.DrawImage(barcodeImage, new Point(10, y));
                y += barcodeImage.Height + 30;

                currentPrintIndex++;

                // If there are more patients, request another page
                args.HasMorePages = currentPrintIndex < patientsToPrint.Count;
            };

            // Ask user to select printer
            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.Document = doc;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    // Show preview after printer selection
                    using (PrintPreviewDialog previewDialog = new PrintPreviewDialog
                    {
                        Document = doc,
                        Width = 800,
                        Height = 600
                    })
                    {
                        previewDialog.ShowDialog();
                    }
                }
            }
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
        private async void PatientAnalyticsView_SearchRequestedByHIR(object? sender, EventArgs e)
        {
            string inputId = patientAnalyticsView.SearchQueryByHIR.Trim();
            if (string.IsNullOrWhiteSpace(inputId))
            {
                patientView.ShowMessage("Please enter a Barcode ID.");
                return;
            }

            var patient = analyticsRepository.GetPatientByHRI(inputId); // sync call, okay for now

            if (patient != null)
            {
                if (!string.IsNullOrEmpty(SigninPresenter.LoggedInUserFullName))
                {
                    await analyticsRepository.UpdateExaminer(inputId, SigninPresenter.LoggedInUserFullName);
                }
            }
            else
            {
                patientView.ShowMessage("Barcode is invalid!");
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

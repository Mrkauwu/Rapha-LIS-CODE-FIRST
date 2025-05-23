using Guna.UI2.WinForms;
using MaterialSkin;
using MaterialSkin.Controls;
using Rapha_LIS.Models;
using Rapha_LIS.Views.CEditEventArgs;
using Rapha_LIS.Views.TListEventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Rapha_LIS.Views
{
    public partial class Rapha_LIS : MaterialForm, IPatientControlView, IUserControlView, IPatientAnalyticsView
    {
        private bool isEdit;
        private bool isEditUser;
        private bool isEditResult;

        public Rapha_LIS()
        {
            InitializeComponent();
            AssociateAndRaiseViewEvents();


            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
            (Primary)0xFFFFFF,  // Clean white background for a clinical look
            (Primary)0xE3F2FD,  // Soft blue for a calming, medical feel
            (Primary)0x64B5F6,  // Standard blue for professional contrast
            (Accent)0x1E88E5,  // Orange for energy and urgency in alerts
            TextShade.BLACK
            );

            dgvPatientControl.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvUserControl.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAnalyticsPatients.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        }


        //Patient Control

        private void AssociateAndRaiseViewEvents()
        {
            // PatientControl TabPage
            btnAddPatient.Click += (s, e) => AddPatientRequested?.Invoke(this, EventArgs.Empty);
            txtPatientControlSearch.TextChanged += (s, e) => StartSearchTimer("Patient");
            btnDelete.Click += (s, e) => DeletePatientRequested?.Invoke(this, EventArgs.Empty);
            btnPrintBarcode.Click += (s, e) => PrintBarcodeRequested?.Invoke(this, EventArgs.Empty);

            dgvPatientControl.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    CellValueEdited?.Invoke(this, new CellEditEventArgs(e.RowIndex, e.ColumnIndex));
            };


            // User Control TabPage
            btnAddUser.Click += (s, e) => UserAddRequested?.Invoke(this, EventArgs.Empty);
            btnDeleteUser.Click += (s, e) => DeleteUserRequested?.Invoke(this, EventArgs.Empty);

            // Update all references to the field in the file:  
            txtUserControlSearch.TextChanged += (s, e) => StartSearchTimer("User");
            dgvUserControl.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    UserCellValueEdited?.Invoke(this, new CellEditEventArgs(e.RowIndex, e.ColumnIndex));
            };

            // Analytics TabPage
            txtAnalyticsSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SearchRequestedByHIR?.Invoke(this, EventArgs.Empty); };
            button1.Click += (s, e) => SearchRequestedByHIR?.Invoke(this, EventArgs.Empty);
            btnPrintResult.Click += (s, e) => PrintResultRequested?.Invoke(this, EventArgs.Empty);
            dgvAnalyticsPatients.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    AnalyticsCellValueEdited?.Invoke(this, new CellEditEventArgs(e.RowIndex, e.ColumnIndex));
            };

            timer1.Tick += (s, e) =>
            {
                timer1.Stop();

                switch (timer1.Tag?.ToString())
                {
                    case "Patient":
                        SearchRequestedByName?.Invoke(this, EventArgs.Empty);
                        break;
                    case "User":
                        UserSearchRequestedByName?.Invoke(this, EventArgs.Empty);
                        break;
                    case "Result":
                        ResultSearchRequested?.Invoke(this, EventArgs.Empty);
                        break;
                }
            };


        }

        private void StartSearchTimer(string searchType)
        {
            timer1.Stop();
            timer1.Tag = searchType;
            timer1.Start();
        }

        public bool IsEdit
        {
            get { return isEdit; }
            set { isEdit = value; }
        }

        public string SearchQueryByName
        {
            get { return txtPatientControlSearch.Text; }
            set { txtPatientControlSearch.Text = value; }
        }

        public void BindPatientControlList(BindingSource patientControlList)
        {

            dgvPatientControl.DataSource = patientControlList;

        }

        //User Control

        public string UserSearchQueryByName
        {
            get { return txtUserControlSearch.Text; }
            set { txtUserControlSearch.Text = value; }
        }



        public void BindUserControlList(BindingSource userControlList)
        {
            dgvUserControl.DataSource = userControlList;
        }

        public List<int> SelectedUser =>
            dgvUserControl.SelectedRows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow)
            .Select(r => int.TryParse(r.Cells["Id"].Value?.ToString(), out var id) ? id : 0)
            .Where(id => id != 0)
            .ToList();

        public string SearchQueryByHIR
        {
            get { return txtAnalyticsSearch.Text; }
            set { txtAnalyticsSearch.Text = value; }
        }

        public void BindPatientAnalyticsList(BindingSource patientAnalyticsList)
        {
            dgvAnalyticsPatients.DataSource = patientAnalyticsList;

        }


        public List<int> SelectedPatient =>
            dgvPatientControl.SelectedRows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow)
            .Select(r => int.TryParse(r.Cells["Id"].Value?.ToString(), out var id) ? id : 0)
            .Where(id => id != 0)
            .ToList();

        public List<int> SelectedToPrint =>
            dgvAnalyticsPatients.SelectedRows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow)
            .Select(r => int.TryParse(r.Cells["Id"].Value?.ToString(), out var id) ? id : 0)
            .Where(id => id != 0)
            .ToList();

        public void ShowMessage(string message, string title = "Info")
        {
            MessageBox.Show(message, title);
        }

        private void dgvPatientControl_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.Enabled = dgvPatientControl.CurrentCell.ColumnIndex != dgvPatientControl.Columns["Test"].Index;
        }

        private void dgvPatientControl_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvPatientControl.Columns[e.ColumnIndex].Name != "Test") return;

            e.Cancel = true;

            var currentTests = (dgvPatientControl.Rows[e.RowIndex].Cells["Test"].Value?.ToString() ?? "")
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            OpenTestListRequested?.Invoke(this, new TestListEventArgs(currentTests, e.RowIndex));
        }

        public void UserShowMessage(string message, string title = "Info")
        {
            MessageBox.Show(message, title);
        }

        

        public void UpdateRowWithSelectedTests(int rowIndex, List<TestModel> selectedTests)
        {
            if (dgvPatientControl.Rows.Count <= rowIndex) return;

            var row = dgvPatientControl.Rows[rowIndex];
            row.Cells["Test"].Value = string.Join(Environment.NewLine, selectedTests.Select(t => t.Test));
            row.Cells["NormalValue"].Value = string.Join(Environment.NewLine, selectedTests.Select(t => t.NormalValue.Replace("\r", " ").Replace("\n", " ").Trim()));
            row.Cells["TestResult"].Value = string.Join(Environment.NewLine, selectedTests.Select(_ => "Pending"));
        }

        private void Rapha_LIS_Load(object sender, EventArgs e)
        {

        }

        private void dgvAnalyticsPatients_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Column header mapping
            var headers = new Dictionary<string, string>
        {
            { "TestResult", "Result" },
            { "NormalValue", "Normal Value" },
            { "LeukocytesResult", "Leukocytes Result" },
            { "LeukocytesNormalValue", "Normal Value" },
            { "Age", "Patient Age" },
            { "Sex", "Gender" },
        };

            foreach (var pair in headers)
            {
                if (dgvAnalyticsPatients.Columns.Contains(pair.Key))
                    dgvAnalyticsPatients.Columns[pair.Key].HeaderText = pair.Value;
            }


            if (dgvAnalyticsPatients.Columns.Contains("BarcodeID"))
                dgvAnalyticsPatients.Columns["BarcodeID"].Visible = false;

            string[] readOnlyColumns = { "Id", "MedTech", "NormalValue", "DateCreated", "LeukocytesNormalValue", "Physician", "Sex", "Age", "Name", "Test" };

            foreach (var colName in readOnlyColumns)
            {
                if (dgvAnalyticsPatients.Columns.Contains(colName))
                    dgvAnalyticsPatients.Columns[colName].ReadOnly = true;
            }

        }

        private void dgvUserControl_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvUserControl.Columns.Contains("Sex"))
                dgvUserControl.Columns["Sex"].HeaderText = "Gender";
        }

        private void dgvPatientControl_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Column header mapping
            var headers = new Dictionary<string, string>
        {
            { "TestResult", "Result" },
            { "NormalValue", "Normal Value" },
            { "LeukocytesResult", "Leukocytes Result" },
            { "LeukocytesNormalValue", "Normal Value" },
            { "Age", "Patient Age" },
            { "Sex", "Gender" },
        };

            foreach (var pair in headers)
            {
                if (dgvPatientControl.Columns.Contains(pair.Key))
                    dgvPatientControl.Columns[pair.Key].HeaderText = pair.Value;
            }


            if (dgvPatientControl.Columns.Contains("BarcodeID"))
                dgvPatientControl.Columns["BarcodeID"].Visible = false;

            string[] readOnlyColumns = { "Id", "MedTech", "NormalValue", "DateCreated", "LeukocytesNormalValue", "TestResult", "LeukocytesResult" };

            foreach (var colName in readOnlyColumns)
            {
                if (dgvPatientControl.Columns.Contains(colName))
                    dgvPatientControl.Columns[colName].ReadOnly = true;
            }

        }




        //IPatientControlView Eventhandler
        public event EventHandler? SearchRequestedByName;
        public event EventHandler? AddPatientRequested;
        public event EventHandler<CellEditEventArgs>? CellValueEdited;
        public event EventHandler<TestListEventArgs>? OpenTestListRequested;
        public event EventHandler PrintBarcodeRequested;

        //IUserControlView EventHandler
        public event EventHandler? UserSearchRequestedByName;
        public event EventHandler? UserAddRequested;
        public event EventHandler? UserActionRequested;
        public event EventHandler<CellEditEventArgs>? UserCellValueEdited;
        public event EventHandler DeleteUserRequested;

        //IPatientAnalyticsView EventHandler
        public event EventHandler? PrintResultRequested;
        public event EventHandler? AnalyticsActionRequested;
        public event EventHandler<CellEditEventArgs>? AnalyticsCellValueEdited;

        //IPatientResult EventHandler
        public event EventHandler? ResultSearchRequested;
        public event EventHandler? ResultActionRequested;
        public event EventHandler DeletePatientRequested;
        public event EventHandler? SearchRequestedByHIR;
    }
}

using Guna.UI2.WinForms;
using MaterialSkin;
using MaterialSkin.Controls;
using Rapha_LIS.Models;
using Rapha_LIS.Presenters;
using Rapha_LIS.Views.CEditEventArgs;
using Rapha_LIS.Views.LListEventArgs;
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
    public partial class Rapha_LIS : MaterialForm, IPatientControlView, IUserControlView, IPatientAnalyticsView, IDashboard
    {
        private bool isEdit;
        private int? passwordVisibleRowIndex = null;

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

            label2.Font = new Font("Segoe UI", 20);
            label1.Font = new Font("Segoe UI", 20);
            Color myColor = System.Drawing.Color.FromArgb(100, 181, 246);
            Color foreColor = Color.White;
            label3.BackColor = myColor;
            label3.ForeColor = foreColor;
            label4.BackColor = myColor;
            label4.ForeColor = foreColor;
            label5.BackColor = myColor;
            label5.ForeColor = foreColor;
            label6.BackColor = myColor;
            label6.ForeColor = foreColor;
            label7.BackColor = myColor;
            label7.ForeColor = foreColor;
            label8.BackColor = myColor;
            label8.ForeColor = foreColor;
            label9.BackColor = myColor;
            label9.ForeColor = foreColor;
            label10.BackColor = myColor;
            label10.ForeColor = foreColor;

            fbtnAddPatient.ForeColor = Color.White;
            fbtnAddUser.ForeColor = Color.White;
            fbtnPrintBarcode.ForeColor = Color.White;
            fbtnScanBarcode.ForeColor = Color.White;


        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }


        public void TabVisibilityBasedOnUserRole()
        {
            materialTabControl1.TabPages.Remove(Dashboard);
            materialTabControl1.TabPages.Remove(PatientManagement);
            materialTabControl1.TabPages.Remove(UserManagement);
        }


        //Patient Control

        private void AssociateAndRaiseViewEvents()
        {
            //Dashboard TabPage
            fbtnAddPatient.Click += (s, e) =>
            {
                materialTabControl1.SelectedTab = PatientManagement;
                AddPatientRequested?.Invoke(this, EventArgs.Empty);
            };

            fbtnAddUser.Click += (s, e) =>
            {
                materialTabControl1.SelectedTab = UserManagement;
                UserAddRequested?.Invoke(this, EventArgs.Empty);
            };

            fbtnPrintBarcode.Click += (s, e) => materialTabControl1.SelectedTab = PatientManagement;

            fbtnScanBarcode.Click += (s, e) => materialTabControl1.SelectedTab = PatientAnalytics;

            // PatientControl TabPage
            btnAddPatient.Click += (s, e) => AddPatientRequested?.Invoke(this, EventArgs.Empty);
            txtPatientControlSearch.TextChanged += (s, e) => StartSearchTimer("Patient");
            btnDelete.Click += (s, e) => DeletePatientRequested?.Invoke(this, EventArgs.Empty);
            btnPrintBarcode.Click += (s, e) => PrintBarcodeRequested?.Invoke(this, EventArgs.Empty);

            refreshToolStripMenuItem.Click += (s, e) => RefreshRequested?.Invoke(this, EventArgs.Empty);
            addPatientToolStripMenuItem.Click += (s, e) => AddPatientRequested?.Invoke(this, EventArgs.Empty);
            deletePatientToolStripMenuItem.Click += (s, e) => DeletePatientRequested?.Invoke(this, EventArgs.Empty);
            printBarcodeToolStripMenuItem.Click += (s, e) => PrintBarcodeRequested?.Invoke(this, EventArgs.Empty);

            dgvPatientControl.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    CellValueEdited?.Invoke(this, new CellEditEventArgs(e.RowIndex, e.ColumnIndex));
            };


            // User Control TabPage
            btnAddUser.Click += (s, e) => UserAddRequested?.Invoke(this, EventArgs.Empty);
            btnDeleteUser.Click += (s, e) => DeleteUserRequested?.Invoke(this, EventArgs.Empty);
            txtUserControlSearch.TextChanged += (s, e) => StartSearchTimer("User");
            dgvUserControl.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    UserCellValueEdited?.Invoke(this, new CellEditEventArgs(e.RowIndex, e.ColumnIndex));
            };

            addUserToolStripMenuItem.Click += (s, e) => UserAddRequested?.Invoke(this, EventArgs.Empty);
            deleteUserToolStripMenuItem.Click += (s, e) => DeleteUserRequested?.Invoke(this, EventArgs.Empty);

            // Analytics TabPage
            txtAnalyticsSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SearchRequestedByHIR?.Invoke(this, EventArgs.Empty); };
            btnPrintResult.Click += (s, e) => PrintResultRequested?.Invoke(this, EventArgs.Empty);
            dgvAnalyticsPatients.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    AnalyticsCellValueEdited?.Invoke(this, new CellEditEventArgs(e.RowIndex, e.ColumnIndex));
            };
            refreshToolStripMenuItem2.Click += (s, e) => RefreshAnalyticsRequested?.Invoke(this, EventArgs.Empty);
            printResultToolStripMenuItem.Click += (s, e) => PrintResultRequested?.Invoke(this, EventArgs.Empty);

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
            var columnName = dgvPatientControl.Columns[e.ColumnIndex].Name;

            if (columnName == "Test")
            {
                e.Cancel = true;

                var currentTests = (dgvPatientControl.Rows[e.RowIndex].Cells["Test"].Value?.ToString() ?? "")
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                OpenTestListRequested?.Invoke(this, new TestListEventArgs(currentTests, e.RowIndex));
            }
            else if (columnName == "Leukocytes")
            {
                e.Cancel = true;

                var currentLeukocytes = (dgvPatientControl.Rows[e.RowIndex].Cells["Leukocytes"].Value?.ToString() ?? "")
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                OpenLeukocytesListRequested?.Invoke(this, new LeukocytesListEventArgs(currentLeukocytes, e.RowIndex));
            }
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

        public void UpdateRowWithSelectedLeukocytes(int rowIndex, List<LeukocytesModel> selectedLeukocytes)
        {
            if (dgvPatientControl.Rows.Count <= rowIndex) return;

            var row = dgvPatientControl.Rows[rowIndex];
            row.Cells["Leukocytes"].Value = string.Join(Environment.NewLine, selectedLeukocytes.Select(t => t.Leukocytes));
            row.Cells["LeukocytesNormalValue"].Value = string.Join(Environment.NewLine, selectedLeukocytes.Select(t => t.LeukocytesNormalValue.Replace("\r", " ").Replace("\n", " ").Trim()));
            row.Cells["LeukocytesResult"].Value = string.Join(Environment.NewLine, selectedLeukocytes.Select(_ => "Pending"));
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
            if (dgvAnalyticsPatients.Columns.Contains("Test"))
                dgvAnalyticsPatients.Columns["Test"].Width = 150;
            if (dgvAnalyticsPatients.Columns.Contains("NormalValue"))
                dgvAnalyticsPatients.Columns["NormalValue"].Width = 150;

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

            if (dgvUserControl.Columns.Contains("Role"))
                dgvUserControl.Columns["Role"].Visible = false;

            string[] readOnlyColumns = { "Id", "DateCreated", };

            foreach (var colName in readOnlyColumns)
            {
                if (dgvUserControl.Columns.Contains(colName))
                    dgvUserControl.Columns[colName].ReadOnly = true;
            }
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
            if (dgvPatientControl.Columns.Contains("Test"))
                dgvPatientControl.Columns["Test"].Width = 150;
            if (dgvPatientControl.Columns.Contains("NormalValue"))
                dgvPatientControl.Columns["NormalValue"].Width = 150;



            string[] readOnlyColumns = { "Id", "MedTech", "NormalValue", "DateCreated", "LeukocytesNormalValue", "TestResult", "LeukocytesResult" };

            foreach (var colName in readOnlyColumns)
            {
                if (dgvPatientControl.Columns.Contains(colName))
                    dgvPatientControl.Columns[colName].ReadOnly = true;
            }
        }

        private void materialTabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage.Text == "Logout") // or check by tab index or name
            {
                DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    LogoutRequested.Invoke(this, EventArgs.Empty);
                }

                e.Cancel = true; // Prevent the tab from opening
            }
        }

        private void dgvUserControl_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvUserControl.Columns[e.ColumnIndex].Name == "Password")
            {
                if (e.Value != null && e.RowIndex != passwordVisibleRowIndex)
                {
                    e.Value = new string('*', e.Value.ToString()?.Length ?? 0);
                    e.FormattingApplied = true;
                }
            }
        }

        private void showPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUserControl.SelectedRows.Count > 0)
            {
                passwordVisibleRowIndex = dgvUserControl.SelectedRows[0].Index;
                dgvUserControl.Invalidate(); // Refresh the grid to trigger CellFormatting
            }
        }

        private void dgvUserControl_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUserControl.SelectedRows.Count > 0 &&
        passwordVisibleRowIndex != null &&
        dgvUserControl.SelectedRows[0].Index != passwordVisibleRowIndex)
            {
                passwordVisibleRowIndex = null;
                dgvUserControl.Invalidate();
            }
        }

        private void dgvUserControl_Leave(object sender, EventArgs e)
        {
            if (passwordVisibleRowIndex != null)
            {
                passwordVisibleRowIndex = null;
                dgvUserControl.Invalidate();
            }
        }

        public void BindDashboardList(BindingSource dashboardList)
        {
            dgvDashboard.DataSource = dashboardList;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void dgvDashboard_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

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
                if (dgvDashboard.Columns.Contains(pair.Key))
                    dgvDashboard.Columns[pair.Key].HeaderText = pair.Value;
            }

            if (dgvDashboard.Columns.Contains("BarcodeID"))
                dgvDashboard.Columns["BarcodeID"].Visible = false;
        }

        //IPatientControlView Eventhandler
        public event EventHandler? SearchRequestedByName;
        public event EventHandler? AddPatientRequested;
        public event EventHandler<CellEditEventArgs>? CellValueEdited;
        public event EventHandler<TestListEventArgs>? OpenTestListRequested;
        public event EventHandler PrintBarcodeRequested;
        public event EventHandler<LeukocytesListEventArgs>? OpenLeukocytesListRequested;
        public event EventHandler RefreshRequested;
        public event EventHandler DeletePatientRequested;
        public event EventHandler LogoutRequested;

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
        public event EventHandler? RefreshAnalyticsRequested;
        public event EventHandler? SearchRequestedByHIR;

    }
}

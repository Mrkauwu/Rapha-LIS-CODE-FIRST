using Rapha_LIS.Models;
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
    public partial class TestListView : Form, ITestListView
    {
        public TestListView()
        {
            InitializeComponent();
            AssociateAndRaiseEvents();
        }

        private void AssociateAndRaiseEvents()
        {
            btnSave.Click += (_, _) => SaveTestRequested?.Invoke(this, EventArgs.Empty);
            txtSearch.TextChanged += (_, _) => SearchTestRequested?.Invoke(this, EventArgs.Empty);
        }

        public List<TestModel> SelectedTests =>
            clbTests.CheckedItems.Cast<object>()
                .OfType<TestModel>()
                .ToList();

        public void SetTestList(IEnumerable<TestModel> tests)
        {
            clbTests.Items.Clear();
            clbTests.Items.AddRange(tests.Cast<object>().ToArray());
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public event EventHandler? SaveTestRequested;
        public event EventHandler? SearchTestRequested;
    }
}

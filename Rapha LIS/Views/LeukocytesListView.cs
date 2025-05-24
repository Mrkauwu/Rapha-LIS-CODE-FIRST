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
using static System.Net.Mime.MediaTypeNames;

namespace Rapha_LIS.Views
{
    public partial class LeukocytesListView : Form, ILeukocytesListView
    {
        public LeukocytesListView()
        {
            InitializeComponent();
            AssociateAndRaiseEvents();
        }

        private void AssociateAndRaiseEvents()
        {
            btnSave.Click += (_, _) => SaveLeukocytesRequested?.Invoke(this, EventArgs.Empty);
            txtSearch.TextChanged += (_, _) => SearchLeukocytesRequested?.Invoke(this, EventArgs.Empty);
        }

        public List<LeukocytesModel> SelectedLeukocytes =>
            clbLeukocytes.CheckedItems.Cast<object>()
                .OfType<LeukocytesModel>()
                .ToList();

        public void SetLeukocytesList(IEnumerable<LeukocytesModel> leukocytes)
        {
            clbLeukocytes.Items.Clear();
            clbLeukocytes.Items.AddRange(leukocytes.Cast<object>().ToArray());
        }

        public event EventHandler? SaveLeukocytesRequested;
        public event EventHandler? SearchLeukocytesRequested;
    }
}

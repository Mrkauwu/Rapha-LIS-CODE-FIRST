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

        private HashSet<int> _checkedLeukocyteIds = new();


        public LeukocytesListView()
        {
            InitializeComponent();
            AssociateAndRaiseEvents();
        }

        private void AssociateAndRaiseEvents()
        {
            btnSave.Click += (_, _) =>
            {
                _checkedLeukocyteIds.Clear(); // Clear selections on save

                foreach (LeukocytesModel item in clbLeukocytes.CheckedItems)
                    _checkedLeukocyteIds.Add(item.Id);

                SaveLeukocytesRequested?.Invoke(this, EventArgs.Empty);
            };

            txtSearch.TextChanged += (_, _) => SearchLeukocytesRequested?.Invoke(this, EventArgs.Empty);

            clbLeukocytes.ItemCheck += (s, e) =>
            {
                var item = clbLeukocytes.Items[e.Index] as LeukocytesModel;
                if (item == null) return;

                // Use BeginInvoke to wait for the check state to update
                this.BeginInvoke(new Action(() =>
                {
                    if (clbLeukocytes.GetItemChecked(e.Index))
                        _checkedLeukocyteIds.Remove(item.Id); // Now unchecked
                    else
                        _checkedLeukocyteIds.Add(item.Id); // Now checked
                }));
            };
        }


        public List<LeukocytesModel> SelectedLeukocytes =>
            clbLeukocytes.CheckedItems.Cast<object>()
                .OfType<LeukocytesModel>()
                .ToList();

        public void SetLeukocytesList(IEnumerable<LeukocytesModel> leukocytes)
        {
            clbLeukocytes.Items.Clear();

            foreach (var leukocyte in leukocytes)
            {
                bool isChecked = _checkedLeukocyteIds.Contains(leukocyte.Id);
                clbLeukocytes.Items.Add(leukocyte, isChecked);
            }
        }

        public event EventHandler? SaveLeukocytesRequested;
        public event EventHandler? SearchLeukocytesRequested;
    }
}

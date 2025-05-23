using Rapha_LIS.Models;
using Rapha_LIS.Views.CEditEventArgs;
using Rapha_LIS.Views.TListEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
    public interface IPatientControlView
    {
        string SearchQueryByName { get; set; }

        bool IsEdit { get; set; }

        event EventHandler SearchRequestedByName;
        event EventHandler AddPatientRequested;
        event EventHandler DeletePatientRequested;
        event EventHandler PrintBarcodeRequested;
        event EventHandler<CellEditEventArgs>? CellValueEdited;
        List<int> SelectedPatient { get; }
        void BindPatientControlList(BindingSource patientControlList);
        void ShowMessage(string message, string title = "Info");
        void UpdateRowWithSelectedTests(int rowIndex, List<TestModel> selectedTests);
        event EventHandler<TestListEventArgs>? OpenTestListRequested;
    }
}

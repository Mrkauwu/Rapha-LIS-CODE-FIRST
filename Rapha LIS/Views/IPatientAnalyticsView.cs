using Rapha_LIS.Views.CEditEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
     public interface IPatientAnalyticsView
    {
        string SearchQueryByHIR { get; set; }
        event EventHandler? PrintResultRequested;
        event EventHandler? AnalyticsActionRequested;
        event EventHandler<CellEditEventArgs>? AnalyticsCellValueEdited;
        event EventHandler? SearchRequestedByHIR;
        List<int> SelectedToPrint { get; }
        void BindPatientAnalyticsList(BindingSource patientAnalyticsList);
        void Show();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
    public interface IDashboardView
    {
        event EventHandler DashboardRefreshRequested;
        void BindDashboardList(BindingSource dashboardList);
        void SetPendingCount(int count);
        void SetInProcessCount(int count);
        void SetCompleteCount(int count);
    }
}

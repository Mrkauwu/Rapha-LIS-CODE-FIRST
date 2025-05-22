using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
    public interface ITestListView
    {
        event EventHandler? SaveTestRequested;
        event EventHandler? SearchTestRequested;
        List<TestModel> SelectedTests { get; }
        void SetTestList(IEnumerable<TestModel> tests);
    }
}

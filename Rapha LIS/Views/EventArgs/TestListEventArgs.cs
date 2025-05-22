using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views.TListEventArgs
{
    public class TestListEventArgs : EventArgs
    {
        public List<string> CurrentTests { get; }
        public int RowIndex { get; }

        public TestListEventArgs(List<string> currentTests, int rowIndex)
        {
            CurrentTests = currentTests;
            RowIndex = rowIndex;
        }
    }
}

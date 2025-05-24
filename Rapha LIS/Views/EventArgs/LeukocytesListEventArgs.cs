using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views.LListEventArgs
{
    public class LeukocytesListEventArgs
    {
        public List<string> CurrentLeukocytes { get; }
        public int RowIndex { get; }

        public LeukocytesListEventArgs(List<string> currentLeukocytes, int rowIndex)
        {
            CurrentLeukocytes = currentLeukocytes;
            RowIndex = rowIndex;
        }
    }
}

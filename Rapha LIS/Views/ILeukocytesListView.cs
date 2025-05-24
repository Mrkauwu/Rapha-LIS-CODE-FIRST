using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
    public interface ILeukocytesListView
    {
        event EventHandler? SaveLeukocytesRequested;
        event EventHandler? SearchLeukocytesRequested;
        List<LeukocytesModel> SelectedLeukocytes { get; }
        void SetLeukocytesList(IEnumerable<LeukocytesModel> leukocytes);
    }
}

using Rapha_LIS.Models;
using Rapha_LIS.Views.CEditEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
    public interface IUserControlView
    {
        string UserSearchQueryByName { get; set; }

        event EventHandler UserSearchRequestedByName;
        event EventHandler UserAddRequested;
        event EventHandler<CellEditEventArgs>? UserCellValueEdited;

        void UserShowMessage(string message, string title = "Info");
        void BindUserControlList(BindingSource userControlList);
        void Show();
    }
}

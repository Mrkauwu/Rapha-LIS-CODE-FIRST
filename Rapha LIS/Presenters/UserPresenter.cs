using Rapha_LIS.Models;
using Rapha_LIS.Repositories;
using Rapha_LIS.Views;
using Rapha_LIS.Views.CEditEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Presenters
{
    public class UserPresenter
    {
        private readonly IUserControlRepository userRepository;
        private readonly IUserControlView userControlView;
        private readonly BindingSource UserControlBindingSource;
        private List<UserModel>? userList;
        private List<FilteredUserModel>? filteredUserList;

        public UserPresenter(IUserControlView userControlView, IUserControlRepository userRepository) 
        {
            //PatientControlView
            this.userControlView = userControlView ?? throw new ArgumentNullException(nameof(userControlView));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

            //PatientControlView
            this.userControlView.UserSearchRequestedByName += UserControlView_UserSearchRequestedByName;
            this.userControlView.UserAddRequested += UserControlView_UserAddRequested;
            this.userControlView.UserCellValueEdited += (s, e) => UserControlView__CellValueEdited(null, e.RowIndex);

            this.UserControlBindingSource = new BindingSource();  // ✅ Initialize first
            this.userControlView.BindUserControlList(UserControlBindingSource);

            //PatientActionView


            LoadAllUserList();
            this.userControlView.Show();
        }

        private void UserControlView__CellValueEdited(object value, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= (userList?.Count ?? 0)) return;

            try
            {
                userRepository.SaveOrUpdateUser(userList[rowIndex]);
            }
            catch (Exception ex)
            {
                userControlView.UserShowMessage($"Auto-save failed: {ex.Message}", "Error");
            }
        }

        private void LoadAllUserList()
        {
            userList = userRepository.GetAll().ToList();
            UserControlBindingSource.DataSource = userList;
        }

        //User Control



        private void UserControlView_UserAddRequested(object? sender, EventArgs e)
        {
            try
            {
                var newUser = new UserModel
                {
                    Id = userRepository.InsertEmptyUser(),
                    Name = "",
                    Age = null,
                    Sex = "",
                    DateCreated = DateTime.Now
                };

                userList?.Insert(0, newUser);
                UserControlBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                userControlView.UserShowMessage($"Add failed: {ex.Message}", "Error");
            }
        }

        private void UserControlView_UserSearchRequestedByName(object? sender, EventArgs e)
        {
                bool emptyValue = string.IsNullOrWhiteSpace(this.userControlView.UserSearchQueryByName);
                if (emptyValue == false)
                filteredUserList = userRepository.GetByFilteredName(this.userControlView.UserSearchQueryByName); 
            else filteredUserList = userRepository.GetFilteredName();
            UserControlBindingSource.DataSource = filteredUserList;
        }

        private void UserActionVIew_UserDeleteRequested(object? sender, EventArgs e)
        {
            var user = (UserModel)UserControlBindingSource.Current;
            userRepository.DeleteUser(user.Id);
            //userActionVIew.IsSuccessful = true;
            //userActionVIew.Message = "User Deleted Successfuly";
            LoadAllUserList();
        }
    }
}

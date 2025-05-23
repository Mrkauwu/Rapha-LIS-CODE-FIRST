using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public interface IUserControlRepository
    {

        void DeleteUser(List<int> ids);
        List<UserModel> GetAll();
        void SaveOrUpdateUser(UserModel user);
        List<FilteredUserModel> GetFilteredName();
        List<FilteredUserModel> GetByFilteredName(string value);
        int InsertEmptyUser();
    }
}

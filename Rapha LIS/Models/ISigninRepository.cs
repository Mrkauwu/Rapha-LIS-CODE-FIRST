using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Models
{
    public interface ISigninRepository
    {
        string? ValidateUser(string username, string password);
    }
}

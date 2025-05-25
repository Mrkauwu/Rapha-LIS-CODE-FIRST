using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Helpers
{
    public static class Session
    {
        public static UserModel? CurrentUser { get; set; }
    }
}

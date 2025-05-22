using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Views
{
    public interface ISigninView
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

        event EventHandler? SigninRequested;
    }
}

using Rapha_LIS.Models;
using Rapha_LIS.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Presenters
{
    public class SigninPresenter
    {
        private readonly ISigninView signinView;
        private readonly ISigninRepository signinRepository;
        public static string? LoggedInUserFullName { get; set; }

        public SigninPresenter(ISigninView signinView, ISigninRepository signinRepository) 
        {
            this.signinView = signinView ?? throw new ArgumentNullException(nameof(signinView));
            this.signinRepository = signinRepository ?? throw new ArgumentNullException(nameof(signinRepository));

            this.signinView.SigninRequested += SigninView_SigninRequested;
        }
        private void SigninView_SigninRequested(object? sender, EventArgs e)
        {
            string? Name = signinRepository.ValidateUser(signinView.Username, signinView.Password);

            if (!string.IsNullOrEmpty(Name))
            {
                LoggedInUserFullName = Name;
                MessageBox.Show("Login successful! Welcome, " + Name + ".", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ((Form)signinView).DialogResult = DialogResult.OK;  // ✅ This closes the login
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}

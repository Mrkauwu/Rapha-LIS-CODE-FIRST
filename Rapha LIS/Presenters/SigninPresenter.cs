using Microsoft.VisualBasic.ApplicationServices;
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

        //Dashboard
        private readonly IDashboardView dashboardView;
        public static string? LoggedInUserFullName { get; set; }
        public static string? LoggedInUserRole { get; set; }


        public SigninPresenter(ISigninView signinView, ISigninRepository signinRepository, IDashboardView dashboardView) 
        {

            //Dashboard
            this.dashboardView = dashboardView ?? throw new ArgumentNullException(nameof(dashboardView));

            this.signinView = signinView ?? throw new ArgumentNullException(nameof(signinView));
            this.signinRepository = signinRepository ?? throw new ArgumentNullException(nameof(signinRepository));

            this.signinView.SigninRequested += SigninView_SigninRequested;
        }
        private void SigninView_SigninRequested(object? sender, EventArgs e)
        {
            
            var (name, role) = signinRepository.ValidateUser(signinView.Username, signinView.Password);

            if (!string.IsNullOrEmpty(name))
            {
                LoggedInUserFullName = name;
                LoggedInUserRole = role;

                MessageBox.Show("Login successful! Welcome, " + signinView.Username + ".", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ((Form)signinView).DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}

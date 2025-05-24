using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rapha_LIS.Views
{
    public partial class SigninView : MaterialForm, ISigninView
    {
        public SigninView()
        {
            InitializeComponent();
            AssociateAndRaiseViewEvents();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
                        (Primary)0xFFFFFF,  // Clean white background for a clinical look
                        (Primary)0xE3F2FD,  // Soft blue for a calming, medical feel
                        (Primary)0x64B5F6,  // Standard blue for professional contrast
                        (Accent)0x1E88E5,  // Orange for energy and urgency in alerts
                        TextShade.BLACK
                        );

        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        private void AssociateAndRaiseViewEvents()
        {
            btnSignin.Click += (s, e) =>
            {
                SigninRequested?.Invoke(this, EventArgs.Empty);
            };

            txtUsername.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SigninRequested?.Invoke(this, EventArgs.Empty);
                    e.SuppressKeyPress = true; // Prevent "ding" sound
                }
            };

            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SigninRequested?.Invoke(this, EventArgs.Empty);
                    e.SuppressKeyPress = true; // Prevent "ding" sound
                }
            };
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !checkBox.Checked;
        }

        public string? Username
        {
            get { return txtUsername.Text; }
            set { txtUsername.Text = value; }
        }
        public string? Password
        {
            get { return txtPassword.Text; }
            set { txtPassword.Text = value; }
        }

        public event EventHandler? SigninRequested;
    }
}

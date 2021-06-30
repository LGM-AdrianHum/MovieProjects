using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace XBMC_Remote
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (TBIp.Text == "" || TBPort.Text == "")
            {
                MessageBox.Show("Please enter in IP and Port");
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "settings.ini", false))
                {
                    sw.WriteLine(TBIp.Text);
                    sw.WriteLine(TBPort.Text);
                    sw.WriteLine(TBUser.Text);
                    sw.WriteLine(TBPassword.Text);
                }

                Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "settings.ini"))
            {
                TBIp.Text = sr.ReadLine();
                TBPort.Text = sr.ReadLine();
                TBUser.Text = sr.ReadLine();
                TBPassword.Text = sr.ReadLine();
            }
        }

    }
}

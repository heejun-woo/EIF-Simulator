using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Equipment_Interface_Demo.Popup
{
    partial class SubStat : Form
    {
        public int iSelectValue = 0;
        public SubStat()
        {
            InitializeComponent();

            Main.Visible = true;
            pnShutDown.Visible = false;
        }

        private void SelectValue(int i)
        {
            iSelectValue = i;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lbEqpStat_Click(object sender, EventArgs e)
        {
            Main.Visible = false;
            pnShutDown.Visible = true;
        }

        private void label25_Click(object sender, EventArgs e)
        {
            SelectValue(196);
        }

        private void label24_Click(object sender, EventArgs e)
        {
            SelectValue(104);
        }

        private void label22_Click(object sender, EventArgs e)
        {
            SelectValue(140);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

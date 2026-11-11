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
    partial class CurrentWorkType : Form
    {
        public int iSelectValue = 0;
        public CurrentWorkType()
        {
            InitializeComponent();

            Main.Visible = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void label9_Click(object sender, EventArgs e)
        {
            if (sender is not Label lb)
                return;

            Panel panel = lb.Parent as Panel;

            if (panel == null)
                return;

            foreach (Label label in panel.Controls.OfType<Label>())
            {
                label.BackColor = Color.Yellow;
                label.ForeColor = Color.Black;
            }

            lb.BackColor = Color.Black;
            lb.ForeColor = Color.Yellow;

            iSelectValue = Convert.ToInt32(lb.Tag);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

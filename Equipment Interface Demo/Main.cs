using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Equipment_Interface_Demo
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Notching fm = new Notching();
            fm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Packaging fm = new Packaging();
            fm.Show();
        }
    }
}

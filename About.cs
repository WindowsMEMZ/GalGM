using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                label2.Text = "GalGM 0.3.0 (64位) by WindowsMEMZ - Preview";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

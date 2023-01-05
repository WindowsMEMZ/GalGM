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
    public partial class Outputs : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Outputs()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
        public ComboBox GetComboBox
        {
            get { return comboBox1; }
        }
        public TextBox GetTextBox
        {
            get { return textBox1; }
        }

        private void Outputs_Load(object sender, EventArgs e)
        {

        }
    }
}

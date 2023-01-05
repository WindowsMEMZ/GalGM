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
    public partial class Resources_Audio : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event DelegateButtonClicked Button8Clicked;

        public Resources_Audio()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
        public ListView GetListView
        {
            get { return listView3; }
        }

        private void Resources_Audio_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Button8Clicked(sender, e);
        }
    }
}

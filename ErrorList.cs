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
    public partial class ErrorList : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ErrorList()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }

        public ListView GetListView
        {
            get { return listView1; }
        }

        private void ErrorList_Load(object sender, EventArgs e)
        {

        }
    }
}

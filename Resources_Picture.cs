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
    public delegate void DelegateButtonClicked(object sender, EventArgs e);

    public partial class Resources_Picture : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event DelegateButtonClicked Button7Clicked;
        public event DelegateButtonClicked Button9Clicked;

        public Resources_Picture()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
        public ImageList GetImageList
        {
            get { return imageList1; }
        }
        public ListView GetListView
        {
            get { return listView2; }
        }
        public Label GetLabel
        {
            get { return label9; }
        }

        private void Resources_Picture_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Button7Clicked(sender, e);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Button9Clicked(sender, e);
        }
    }
}

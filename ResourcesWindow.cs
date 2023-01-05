using System;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    public partial class ResourcesWindow : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event DelegateButtonClicked Button7Clicked;
        public event DelegateButtonClicked Button9Clicked;
        public event DelegateButtonClicked Button8Clicked;

        public static Resources_Picture resP = new Resources_Picture();
        public static Resources_Audio resA = new Resources_Audio();
        ImageList imageListPic = resP.imageList1;
        ListView listViewPic = resP.listView2;
        Label labelPic = resP.label9;
        ListView listViewAudio = resA.listView3;

        public ResourcesWindow()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;

            var pictures = new Resources_Picture() { TabText = "图片" };
            pictures.Button7Clicked += new DelegateButtonClicked(Button7_Click);
            pictures.Button9Clicked += new DelegateButtonClicked(Button9_Click);
            pictures.Show(this.dockPanel1, DockState.Document);
            var audios = new Resources_Audio() { TabText = "音频" };
            audios.Button8Clicked += new DelegateButtonClicked(Button8_Click);
            audios.Show(this.dockPanel1, DockState.Document);

            imageListPic = pictures.GetImageList;
            listViewPic = pictures.GetListView;
            labelPic = pictures.GetLabel;
            listViewAudio = audios.GetListView;
        }
        public ImageList GetPicImageList
        {
            get { return imageListPic; }
        }
        public ListView GetPicListView
        {
            get { return listViewPic; }
        }
        public Label GetPicLabel
        {
            get { return labelPic; }
        }
        public ListView GetAudioListView
        {
            get { return listViewAudio; }
        }

        private void ResourcesWindow_Load(object sender, EventArgs e)
        {

        }

        private void Button7_Click(object sender, EventArgs e)
        {
            Button7Clicked(sender, e);
        }
        private void Button9_Click(object sender, EventArgs e)
        {
            Button9Clicked(sender, e);
        }
        private void Button8_Click(object sender, EventArgs e)
        {
            Button8Clicked(sender, e);
        }
    }
}

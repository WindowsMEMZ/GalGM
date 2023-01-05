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
    public delegate void DelegateCheckBoxCheckedChanged(object sender, EventArgs e);

    public partial class ProjectSettings : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event DelegateCheckBoxCheckedChanged CheckBox1CheckedChanged;

        public ProjectSettings()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
        public CheckBox GetCheckBox1
        {
            get { return checkBox1; }
        }

        private void ProjectSettings_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox1CheckedChanged(sender, e);
        }

        private void checkBox1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(checkBox1, "GalGM默认将您的资源直接复制到输出文件以供使用，您可以通过加密来保护资源，启用加密会影响代码执行效率。注意：这并不能保证您的资源不被找到");
        }
    }
}

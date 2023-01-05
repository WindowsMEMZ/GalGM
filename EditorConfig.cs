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
    public partial class EditorConfig : Form
    {
        public EditorConfig()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            INIHelper.Write("Debug", "openDebugTool", checkBox1.Checked.ToString(), "./editor.config");
        }

        private void EditorConfig_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = bool.Parse(INIHelper.Read("Debug", "openDebugTool", "true", "./editor.config"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    public delegate void DelegateCheckedChanged(object sender, EventArgs e);

    public partial class StartPageSetting : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event DelegateCheckedChanged Checkbox1CheckedChanged;
        public event DelegateCheckedChanged Checkbox2CheckedChanged;
        public event DelegateCheckedChanged Checkbox3CheckedChanged;
        public event DelegateCheckedChanged Checkbox4CheckedChanged;
        public event DelegateCheckedChanged Checkbox5CheckedChanged;
        public event DelegateTextChanged TextBox1Changed;
        public event DelegateTextChanged TextBox2Changed;
        public event DelegateTextChanged TextBox3Changed;

        private string workSpace;

        public StartPageSetting(string wS = "nil")
        {
            workSpace = wS;

            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
        public CheckBox GetCheckBox1
        {
            get { return checkBox1; }
        }
        public CheckBox GetCheckBox2
        {
            get { return checkBox2; }
        }
        public CheckBox GetCheckBox3
        {
            get { return checkBox3; }
        }
        public CheckBox GetCheckBox4
        {
            get { return checkBox4; }
        }
        public CheckBox GetCheckBox5
        {
            get { return checkBox5; }
        }
        public TextBox GetTextBox1
        {
            get { return textBox1; }
        }
        public TextBox GetTextBox2
        {
            get { return textBox2; }
        }
        public TextBox GetTextBox3
        {
            get { return textBox3; }
        }

        private void StartPageSetting_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = bool.Parse(INIHelper.Read("Profile", "isUseStartPic", "false", $"{workSpace}startPage.prof"));
            checkBox2.Checked = bool.Parse(INIHelper.Read("Profile", "isModifyButton", "false", $"{workSpace}startPage.prof"));
            checkBox3.Checked = bool.Parse(INIHelper.Read("Profile", "isReversalTextColor", "false", $"{workSpace}startPage.prof"));
            checkBox4.Checked = bool.Parse(INIHelper.Read("Profile", "isModifyButtonPic", "false", $"{workSpace}startPage.prof"));
            checkBox5.Checked = bool.Parse(INIHelper.Read("Profile", "isUsingBGM", "false", $"{workSpace}startPage.prof"));

            RefreshCheckBoxStatus();

            textBox1.Text = INIHelper.Read("Profile", "picPath", "", $"{workSpace}startPage.prof");
            textBox2.Text = INIHelper.Read("Profile", "buttonBgPath", "", $"{workSpace}startPage.prof");
            textBox3.Text = INIHelper.Read("Profile", "bgmPath", "", $"{workSpace}startPage.prof");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "选择图片";
            dialog.Filter = "图像文件(*.png)|*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(dialog.FileName, $"{workSpace}assets\\{dialog.FileName.Split('\\')[dialog.FileName.Split('\\').Length - 1]}");
                textBox1.Text = $"{workSpace}assets\\{dialog.FileName.Split('\\')[dialog.FileName.Split('\\').Length - 1]}";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "选择图片";
            dialog.Filter = "图像文件(*.png)|*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(dialog.FileName, $"{workSpace}assets\\{dialog.FileName.Split('\\')[dialog.FileName.Split('\\').Length - 1]}");
                textBox2.Text = $"{workSpace}assets\\{dialog.FileName.Split('\\')[dialog.FileName.Split('\\').Length - 1]}";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "选择音频";
            dialog.Filter = "Wav音频文件(*.wav)|*.wav";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(dialog.FileName, $"{workSpace}assets\\{dialog.FileName.Split('\\')[dialog.FileName.Split('\\').Length - 1]}");
                textBox2.Text = $"{workSpace}assets\\{dialog.FileName.Split('\\')[dialog.FileName.Split('\\').Length - 1]}";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Checkbox1CheckedChanged(sender, e);
            RefreshCheckBoxStatus();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Checkbox2CheckedChanged(sender, e);
            RefreshCheckBoxStatus();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Checkbox4CheckedChanged(sender, e);
            RefreshCheckBoxStatus();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Checkbox3CheckedChanged(sender, e);
            RefreshCheckBoxStatus();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Checkbox5CheckedChanged(sender, e);
            RefreshCheckBoxStatus();
        }

        private void RefreshCheckBoxStatus()
        {
            if (!checkBox1.Checked)
            {
                label1.Enabled = false;
                textBox1.Enabled = false;
                button1.Enabled = false;
            }
            else
            {
                label1.Enabled = true;
                textBox1.Enabled = true;
                button1.Enabled = true;
            }
            if (!checkBox2.Checked)
            {
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                label2.Enabled = false;
                textBox2.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                if (!checkBox4.Checked)
                {
                    label2.Enabled = false;
                    textBox2.Enabled = false;
                    button2.Enabled = false;
                }
                else
                {
                    label2.Enabled = true;
                    textBox2.Enabled = true;
                    button2.Enabled = true;
                }
            }
            if (!checkBox5.Checked)
            {
                label3.Enabled = false;
                textBox3.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                label3.Enabled = true;
                textBox3.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox1Changed(sender, e);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox2Changed(sender, e);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            TextBox3Changed(sender, e);
        }
    }
}

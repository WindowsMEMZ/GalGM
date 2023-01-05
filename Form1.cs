using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private string arg;

        public Form1(string[] args)
        {
            InitializeComponent();
            if (args.Length == 1)
            {
                arg = args[0];
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Red;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(36, 36, 36);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            WindowState = FormWindowState.Maximized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化控件
            pictureBox1.Parent = button4;
            pictureBox1.Location = new Point(5, 0);
            pictureBox2.Parent = button5;
            pictureBox2.Location = new Point(3, 0);
        }

        private void TryOpenFile(string path)
        {
            EditMain w = new EditMain(path);
            w.Show();
            this.Hide();
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            label2.BackColor = Color.FromArgb(34, 29, 70);
            button4.FlatAppearance.BorderColor = Color.FromArgb(113, 96, 232);
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            label2.BackColor = Color.FromArgb(56, 56, 56);
            button4.FlatAppearance.BorderColor = Color.FromArgb(66, 66, 66);
            button4.BackColor = Color.FromArgb(56, 56, 56);
        }

        private void label2_MouseEnter(object sender, EventArgs e)
        {
            button4_MouseEnter(sender, e);
            button4.BackColor = Color.FromArgb(34, 29, 70);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "打开项目";
            dialog.Filter = "项目文件(*.galgmproj)|*.galgmproj";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TryOpenFile(dialog.FileName);
            }
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            label3.BackColor = Color.FromArgb(34, 29, 70);
            button5.FlatAppearance.BorderColor = Color.FromArgb(113, 96, 232);
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            label3.BackColor = Color.FromArgb(56, 56, 56);
            button5.FlatAppearance.BorderColor = Color.FromArgb(66, 66, 66);
            button5.BackColor = Color.FromArgb(56, 56, 56);
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            button5_MouseEnter(sender, e);
            button5.BackColor = Color.FromArgb(34, 29, 70);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            label2_MouseEnter(sender, e);
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            label3_MouseEnter(sender, e);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            button5_Click(sender, e);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            button5_Click(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "配置新项目";
            label4.Visible = true;
            label5.Visible = true;
            label7.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            label6.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            button8.Visible = true;
            button4.Visible = false;
            button5.Visible = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            timer1.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "项目位置";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    textBox2.Text = dialog.SelectedPath;
                }
                else
                {
                    MessageBox.Show(this, "项目位置不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void label6_MouseEnter(object sender, EventArgs e)
        {
            label6.BackColor = Color.FromArgb(73, 73, 73);
            button6.BackColor = Color.FromArgb(73, 73, 73);
        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            label6.BackColor = Color.FromArgb(61, 61, 61);
            button6.BackColor = Color.FromArgb(61, 61, 61);
        }

        private void button6_MouseEnter(object sender, EventArgs e)
        {
            label6.BackColor = Color.FromArgb(73, 73, 73);
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            label6.BackColor = Color.FromArgb(61, 61, 61);
        }

        private void label6_MouseDown(object sender, MouseEventArgs e)
        {
            label6.BackColor = Color.FromArgb(142, 142, 142);
            button6.BackColor = Color.FromArgb(142, 142, 142);
        }

        private void label6_MouseUp(object sender, MouseEventArgs e)
        {
            label6.BackColor = Color.FromArgb(73, 73, 73);
            button6.BackColor = Color.FromArgb(73, 73, 73);
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            label6.BackColor = Color.FromArgb(142, 142, 142);
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            label6.BackColor = Color.FromArgb(73, 73, 73);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            button6_Click(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label1.Text = "GalGM";
            label4.Visible = false;
            label5.Visible = false;
            label7.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            label6.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            button8.Visible = false;
            button4.Visible = true;
            button5.Visible = true;
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label7.Text = $"项目将在\"{@textBox2.Text}\\{@textBox1.Text}\\\"中创建";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    string newPath = textBox2.Text + "\\" + textBox1.Text + "\\";
                    Directory.CreateDirectory(newPath);
                    Directory.CreateDirectory($"{newPath}bin");
                    Directory.CreateDirectory($"{newPath}assets");
                    Directory.CreateDirectory($"{newPath}bin\\Debug");
                    Directory.CreateDirectory($"{newPath}bin\\Release");
                    Directory.CreateDirectory($"{newPath}bin\\Debug\\data");
                    Directory.CreateDirectory($"{newPath}bin\\Release\\data");
                    Directory.CreateDirectory($"{newPath}bin\\Debug\\assets");
                    Directory.CreateDirectory($"{newPath}bin\\Release\\assets");
                    INIHelper.Write("Base", "name", textBox1.Text, newPath + textBox1.Text + ".galgmproj");
                    INIHelper.Write("Base", "workSpace", newPath, newPath + textBox1.Text + ".galgmproj");
                    File.Create(newPath + "Dialogs.gc").Close();
                    TryOpenFile(newPath + textBox1.Text + ".galgmproj");
                }
                else
                {
                    MessageBox.Show(this, "项目名称不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(this, "项目位置不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (arg != "" && arg != null)
            {
                TryOpenFile(arg);
            }
            else
            {
                //RegistryKey hkcr = Registry.ClassesRoot;
                //RegistryKey dotGalgmproj = hkcr.CreateSubKey(".galgmproj", true);
                //dotGalgmproj.SetValue("", "GalGM.Project", RegistryValueKind.String);
                //dotGalgmproj.Close();
                //RegistryKey ggmp = hkcr.CreateSubKey("GalGM.Project", true);
                //RegistryKey dI = ggmp.CreateSubKey("DefaultIcon", true);
                //dI.SetValue("", GetType().Assembly.Location, RegistryValueKind.String);
                //dI.Close();
                //RegistryKey shell = ggmp.CreateSubKey("shell", true);
                //RegistryKey open = shell.CreateSubKey("open", true);
                //RegistryKey command = open.CreateSubKey("command", true);
                //command.SetValue("", $"{GetType().Assembly.Location} %1", RegistryValueKind.String);
                //command.Close();
                //open.Close();
                //shell.Close();
                //ggmp.Close();
                //hkcr.Close();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SetWindowRegion();
        }
        public void SetWindowRegion()
        {
            System.Drawing.Drawing2D.GraphicsPath FormPath;
            FormPath = new System.Drawing.Drawing2D.GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            FormPath = GetRoundedRectPath(rect, 14);
            this.Region = new Region(FormPath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect">窗体大小</param>
        /// <param name="radius">圆角大小</param>
        /// <returns></returns>
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            path.AddArc(arcRect, 180, 90);//左上角

            arcRect.X = rect.Right - diameter;//右上角
            path.AddArc(arcRect, 270, 90);

            arcRect.Y = rect.Bottom - diameter;// 右下角
            path.AddArc(arcRect, 0, 90);

            arcRect.X = rect.Left;// 左下角
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

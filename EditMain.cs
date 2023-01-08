using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WeifenLuo.WinFormsUI.Docking;
using static GalGM.EditMain;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace GalGM
{

    public partial class EditMain : Form
    {
        [DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private string nowPath;
        private string workSpace;
        private string buildOutput = "";
        private string exeConfig;
        private List<ListViewItem> errorItems = new List<ListViewItem>();
        private List<ListViewItem> warningItems = new List<ListViewItem>();
        private List<string> imagePaths = new List<string>();
        private List<string> imageNames = new List<string>();
        private List<string> soundNames = new List<string>();
        private List<string> soundPaths = new List<string>();
        private bool isSavedLast = true;
        private bool isRunning = false;
        private bool isWatchOpened = false;
        private bool isCloseDebugging = false;
        private bool isStopDebug = false;
        private bool isFirstPicAdded = false;
        private bool isEncryption = false;
        private bool isHaveAppUpdate = false;

        //错误行号
        private List<uint> gc0003Lines = new List<uint>();
        private List<uint> gc0004Lines = new List<uint>();
        private List<uint> gc0005Lines = new List<uint>();
        private List<uint> gc0006Lines = new List<uint>();
        private bool isGC0007Throwed = false;
        private List<uint> gc0008Lines = new List<uint>();
        private bool isRES001Throwed = false;
        private bool isSP0001Throwed = false;
        private bool isSP0002Throwed = false;
        private bool isSP0003Throwed = false;

        //调试错误
        private string exitCode;
        private string errorDetail;
        private int errorIndex;
        private int currentErrorIndex;
        private bool isThrowed = false;

        public static string debugOutput = "";

        //多窗口互通
        public static ErrorList errList = new ErrorList();
        public static Dialogs dialogs1 = new Dialogs();
        public static Outputs outputs1 = new Outputs();
        public static ProjectSettings projectSettings1 = new ProjectSettings();
        public static StartPageSetting startPageSetting1 = new StartPageSetting();
        
        RichTextBox richTextBox1 = dialogs1.richTextBox1;
        ListView listView1 = errList.listView1;
        ComboBox comboBox1 = outputs1.comboBox1;
        TextBox textBox1 = outputs1.textBox1;
        ToolStripLabel toolStripButton5 = dialogs1.toolStripButton5;
        ToolStripLabel toolStripButton6 = dialogs1.toolStripButton6;
        ToolStripLabel toolStripButton7 = dialogs1.toolStripButton7;
        ToolStripLabel toolStripButton8 = dialogs1.toolStripButton8;
        ToolStripLabel toolStripButton9 = dialogs1.toolStripButton9;
        ImageList imageList1;
        ListView listView2;
        Label label9;
        ListView listView3;
        CheckBox checkBox1 = projectSettings1.checkBox1;
        Panel panel1 = dialogs1.panel1;
        CheckBox startpageCheckBox1 = startPageSetting1.checkBox1;
        CheckBox startpageCheckBox2 = startPageSetting1.checkBox2;
        CheckBox startpageCheckBox3 = startPageSetting1.checkBox3;
        CheckBox startpageCheckBox4 = startPageSetting1.checkBox4;
        CheckBox startpageCheckBox5 = startPageSetting1.checkBox5;
        TextBox startpageTextbox1 = startPageSetting1.textBox1;
        TextBox startpageTextbox2 = startPageSetting1.textBox2;
        TextBox startpageTextbox3 = startPageSetting1.textBox3;

        public EditMain(string path)
        {
            nowPath = path;
            workSpace = INIHelper.Read("Base", "workSpace", nowPath, nowPath);
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            //初始化窗口
            var dialogs = new Dialogs(workSpace) { TabText = "对话" };
            dialogs.TextChanged += new DelegateTextChanged(richTextBox1_TextChanged);
            dialogs.Button5Clicked += new DelegateButtonClicked(button5_Click);
            dialogs.Button6Clicked += new DelegateButtonClicked(button6_Click);
            dialogs.RichTextBox1VStroll += new DelegateVStrolled(richTextBox1_VStroll);
            dialogs.Show(this.dockPanel1, DockState.Document);
            var resW = new ResourcesWindow() { TabText = "资源" };
            resW.Button7Clicked += new DelegateButtonClicked(button7_Click);
            resW.Button9Clicked += new DelegateButtonClicked(button9_Click);
            resW.Button8Clicked += new DelegateButtonClicked(button8_Click);
            resW.Show(this.dockPanel1, DockState.Document);
            var projSetting = new ProjectSettings() { TabText = "项目设置" };
            projSetting.CheckBox1CheckedChanged += new DelegateCheckBoxCheckedChanged(checkBox1_CheckedChanged);
            projSetting.Show(this.dockPanel1, DockState.Document);
            var outputs = new Outputs() { TabText = "输出" };
            var errorList = new ErrorList() { TabText = "错误列表" };
            errorList.Show(this.dockPanel1, DockState.DockBottom);
            outputs.Show(this.dockPanel1, DockState.DockBottom);
            var startPageSetting = new StartPageSetting(workSpace) { TabText = "开始界面" };
            startPageSetting.Checkbox1CheckedChanged += new DelegateCheckedChanged(StartPageSetting_checkbox1_CheckedChanged);
            startPageSetting.Checkbox2CheckedChanged += new DelegateCheckedChanged(StartPageSetting_checkbox2_CheckedChanged);
            startPageSetting.Checkbox3CheckedChanged += new DelegateCheckedChanged(StartPageSetting_checkbox3_CheckedChanged);
            startPageSetting.Checkbox4CheckedChanged += new DelegateCheckedChanged(StartPageSetting_checkbox4_CheckedChanged);
            startPageSetting.Checkbox5CheckedChanged += new DelegateCheckedChanged(StartPageSetting_checkbox5_CheckedChanged);
            startPageSetting.TextBox1Changed += new DelegateTextChanged(StartPageSetting_textBox1_TextChanged);
            startPageSetting.TextBox2Changed += new DelegateTextChanged(StartPageSetting_textBox2_TextChanged);
            startPageSetting.TextBox3Changed += new DelegateTextChanged(StartPageSetting_textBox3_TextChanged);
            startPageSetting.Show(this.dockPanel1, DockState.Document);

            richTextBox1 = dialogs.GetTextBox;
            listView1 = errorList.GetListView;
            comboBox1 = outputs.GetComboBox;
            textBox1 = outputs.GetTextBox;
            toolStripButton5 = dialogs.GetToolStripButton5;
            toolStripButton6 = dialogs.GetToolStripButton6;
            toolStripButton7 = dialogs.GetToolStripButton7;
            toolStripButton8 = dialogs.GetToolStripButton8;
            toolStripButton9 = dialogs.GetToolStripButton9;
            imageList1 = resW.GetPicImageList;
            listView2 = resW.GetPicListView;
            label9 = resW.GetPicLabel;
            listView3 = resW.GetAudioListView;
            checkBox1 = projSetting.GetCheckBox1;
            panel1 = dialogs.GetPanel;
            startpageCheckBox1 = startPageSetting.GetCheckBox1;
            startpageCheckBox2 = startPageSetting.GetCheckBox2;
            startpageCheckBox3 = startPageSetting.GetCheckBox3;
            startpageCheckBox4 = startPageSetting.GetCheckBox4;
            startpageCheckBox5 = startPageSetting.GetCheckBox5;
            startpageTextbox1 = startPageSetting.GetTextBox1;
            startpageTextbox2 = startPageSetting.GetTextBox2;
            startpageTextbox3 = startPageSetting.GetTextBox3;
        }
        public ImageList GetImageList1
        {
            get { return imageList1; }
        }

        private void EditMain_Load(object sender, EventArgs e)
        {
            try
            {
                label1.Text = INIHelper.Read("Base", "name", "ProjectName", nowPath);
                exeConfig = INIHelper.Read("Base", "exeConfig", "Debug", nowPath);
                生成ProjectNameUToolStripMenuItem.Text = $"生成 {label1.Text} (&U)";
                comboBox1.SelectedIndex = 0;
                if (exeConfig == "Debug")
                {
                    toolStripComboBox1.SelectedIndex = 0;
                }
                else
                {
                    toolStripComboBox1.SelectedIndex = 1;
                }
                //读取资源文件
                if (File.Exists(workSpace + "Resources.resg"))
                {
                    int t = int.Parse(INIHelper.Read("Image", "total", "0", workSpace + "Resources.resg"));
                    for (int i = 0; i <= t; i++)
                    {
                        string path = INIHelper.Read("Image", $"path{i + 1}", "nil", workSpace + "Resources.resg");
                        if (path != "nil")
                        {
                            FileStream fs = File.OpenRead(path);
                            long fileLength = fs.Length;
                            byte[] image = new byte[fileLength];
                            fs.Read(image, 0, (int)fileLength);
                            Image result = Image.FromStream(fs);
                            imageList1.Images.Add(result);
                            string name = INIHelper.Read("Image", $"name{i + 1}", "nil", workSpace + "Resources.resg");
                            listView2.Items.Add(name, imageList1.Images.Count - 1);
                            imagePaths.Add(path);
                            imageNames.Add(name);
                        }
                    }
                    int tS = int.Parse(INIHelper.Read("Sound", "total", "0", workSpace + "Resources.resg"));
                    for (int i = 0; i <= tS; i++)
                    {
                        string path = INIHelper.Read("Sound", $"path{i + 1}", "nil", workSpace + "Resources.resg");
                        if (path != "nil")
                        {
                            string name = INIHelper.Read("Sound", $"name{i + 1}", "nil", workSpace + "Resources.resg");
                            ListViewItem item = new ListViewItem();
                            item.SubItems[0].Text = name;
                            item.SubItems.Add(path);
                            item.ForeColor = Color.White;
                            listView3.Items.Add(item);
                            soundPaths.Add(path);
                            soundNames.Add(name);
                        }
                    }
                }
                else
                {
                    File.Create(workSpace + "Resources.resg").Close();
                }
                //读取对话文件
                if (File.Exists(workSpace + "Dialogs.gc"))
                {
                    FileStream fs = new FileStream(workSpace + "Dialogs.gc", FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                }
                else
                {
                    File.Create(workSpace + "Dialogs.gc").Close();
                }
                //读取首图
                if (File.Exists(INIHelper.Read("Image", "firstPic", "nil", $"{workSpace}Resources.resg")))
                {
                    isFirstPicAdded = true;
                }
                //读取项目设置
                if (bool.Parse(INIHelper.Read("Setting", "isEncryption", "false", nowPath)))
                {
                    isEncryption = true;
                    checkBox1.Checked = true;
                }

                CheckCodes();

                isSavedLast = true;

                Thread checkUpdate = new Thread(CheckAppUpdate);
                checkUpdate.Start();
            }
            catch (Exception ex)
            {
                SaveFiles();
                string exStr = ex.ToString().Replace("\n", "{LF}").Replace("\r", "{CR}").Replace("/", "{Slash}").Replace("\\", "{Backslash}").Replace("%", "{Percent}").Replace("?", "{Question}").Replace("&", "{And}").Replace("$", "{Dollar}").Replace("@", "{At}").Replace("#", "{Hash}");
                BugReporter reporter = new BugReporter("EditMainLoading", exStr);
                reporter.ShowDialog();
            }
        }

        private void CheckAppUpdate()
        {
            string latestVer = NetGet("http://api.darock.top/galgm/getnew");
            if (latestVer != "0.3.0 Preview 1")
            {
                isHaveAppUpdate = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isSavedLast)
            {
                switch (MessageBox.Show("有更改尚未保存，是否保存？", "GalGM", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        SaveFiles();
                        Application.Exit();
                        break;
                    case DialogResult.No:
                        Application.Exit();
                        break;
                    case DialogResult.Cancel:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Application.Exit();
            }
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

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Red;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(36, 36, 36);
        }

        private void EditMain_MouseDown(object sender, MouseEventArgs e)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SaveFiles();
        }

        //要不断更新的内容
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //调试输出
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        textBox1.Text = buildOutput;
                        break;
                    case 1:
                        textBox1.Text = debugOutput;
                        break;
                    default:
                        break;
                }
                //右下角显示行数
                int lineIndex = richTextBox1.GetFirstCharIndexOfCurrentLine();
                toolStripButton8.Text = $"行: {richTextBox1.GetLineFromCharIndex(lineIndex) + 1}";
                toolStripButton9.Text = $"字符: {richTextBox1.SelectionStart - lineIndex + 1}";
                //工具栏显示错误和警告
                int eLength = errorItems.Count;
                int wLength = warningItems.Count;
                if (eLength != 0 || wLength != 0)
                {
                    toolStripButton5.Visible = false;
                    toolStripButton6.Visible = true;
                    toolStripButton7.Visible = true;
                    toolStripButton6.Text = eLength.ToString();
                    toolStripButton7.Text = wLength.ToString();
                }
                else
                {
                    toolStripButton5.Visible = true;
                    toolStripButton6.Visible = false;
                    toolStripButton7.Visible = false;
                }
                //检测是否正在运行
                if (isRunning)
                {
                    string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
                    int t = 0;
                    Process[] ps = Process.GetProcesses();
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (ps[i].ProcessName == projectName)
                        {
                            if (isStopDebug)
                            {
                                ps[i].Kill();
                                isStopDebug = false;
                            }
                            t++;
                            break;
                        }
                    }
                    if (t == 0)
                    {
                        if (INIHelper.Read("Base", "exitCode", "0", $"{workSpace}bin\\{exeConfig}\\GalGR.gds") == "0")
                        {
                            statusStrip1.BackColor = Color.FromArgb(66, 66, 66);
                            toolStripButton12.Enabled = false;
                            toolStripButton11.Enabled = false;
                            toolStripButton3.Enabled = true;
                            toolStripButton4.Enabled = true;
                            toolStripButton10.Enabled = true;
                            isRunning = false;
                            isWatchOpened = false;
                        }
                        else
                        {
                            if (!isStopDebug)
                            {
                                if (!isThrowed)
                                {
                                    //调试错误处理
                                    exitCode = INIHelper.Read("Base", "exitCode", "0", $"{workSpace}bin\\{exeConfig}\\GalGR.gds");
                                    errorDetail = INIHelper.Read("Error", "detail", "未定义的错误", $"{workSpace}bin\\{exeConfig}\\GalGR.gds");
                                    errorIndex = int.Parse(INIHelper.Read("Error", "index", "1", $"{workSpace}bin\\{exeConfig}\\GalGR.gds"));
                                    string[] lineText = richTextBox1.Text.Split('\n');
                                    int a = 0;
                                    for (int i = 0; i < lineText.Length && i < errorIndex; i++)
                                    {
                                        if (!string.IsNullOrEmpty(lineText[i]))
                                        {
                                            if (lineText[i][0] == '#' || lineText[i][0] == '^' || lineText[i][0] == '&' || lineText[i][0] == '~')
                                            {
                                                a++;
                                            }
                                        }
                                    }
                                    if (errorIndex + a - 1 >= 0)
                                    {
                                        SelectLine(errorIndex + a - 1);
                                        richTextBox1.SelectionBackColor = Color.FromArgb(124, 59, 59);
                                        currentErrorIndex = errorIndex + a - 1;
                                    }
                                    else
                                    {
                                        SelectLine(0);
                                        richTextBox1.SelectionBackColor = Color.FromArgb(124, 59, 59);
                                        currentErrorIndex = 0;
                                    }
                                    debugPrint($"引发的异常: “{@errorDetail}” (位于 对话 中)");
                                    debugPrint($"程序 “{projectName}.exe” 已退出，返回值为 {exitCode}。");
                                    isThrowed = true;
                                }
                            }
                            else
                            {
                                isStopDebug = false;
                                isThrowed = false;
                                File.Delete($"{workSpace}bin\\{exeConfig}\\GalGR.gds");
                                SelectLine(currentErrorIndex);
                                richTextBox1.SelectionBackColor = Color.FromArgb(30, 30, 30);
                            }
                        }
                    }
                    else if (!isWatchOpened && !isCloseDebugging && bool.Parse(INIHelper.Read("Debug", "openDebugTool", "true", "./editor.config")))
                    {
                        DebugWatch w = new DebugWatch(nowPath, workSpace, exeConfig) { TabText = "诊断工具" };
                        w.Show(this.dockPanel1, DockState.DockRight);
                        isWatchOpened = true;
                    }
                }
                if (isFirstPicAdded)
                {
                    label9.ForeColor = Color.Green;
                }
                else
                {
                    label9.ForeColor = Color.Red;
                }
                //更新窗口位置
            }
            catch (Exception ex)
            {
                SaveFiles();
                string exStr = ex.ToString().Replace("\n", "{LF}").Replace("\r", "{CR}").Replace("/", "{Slash}").Replace("\\", "{Backslash}").Replace("%", "{Percent}").Replace("?", "{Question}").Replace("&", "{And}").Replace("$", "{Dollar}").Replace("@", "{At}").Replace("#", "{Hash}");
                BugReporter reporter = new BugReporter("Timer1", exStr);
                reporter.ShowDialog();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Tutorial w = new Tutorial();
            w.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(richTextBox1.Text))
            {
                if (MessageBox.Show("显示示例内容会清空当前所有代码，且无法撤销，确定吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    richTextBox1.Text = """
                        #WindowsMEMZ
                        这是一个测试对话
                        这是第二个测试对话
                        这句话带有一个语音|这里是语音文件名
                        &这里是背景音乐名
                        #memz233
                        现在说话的人切为了memz233
                        ~这里是新背景图的名称
                        现在背景图切换了
                        """;
                }
            }
            else
            {
                richTextBox1.Text = """
                        #WindowsMEMZ
                        这是一个测试对话
                        这是第二个测试对话
                        这句话带有一个语音|这里是语音文件名
                        &这里是背景音乐名
                        #memz233
                        现在说话的人切为了memz233
                        ~这里是新背景图的名称
                        现在背景图切换了
                        """;
            }
        }

        private List<string> CodeClass()
        {
            List<string> list = new List<string>();
            list.Add("#");
            list.Add("&");
            list.Add("^");
            list.Add("|");
            return list;
        }
        private List<string> Keywords()
        {
            List<string> list = new List<string>();
            list.Add("end");
            return list;
        }

        private void CheckCodes()
        {
            try
            {
                if (!string.IsNullOrEmpty(richTextBox1.Text))
                {
                    if (richTextBox1.Text[0] != '#' && richTextBox1.Text[0] != '&')
                    {
                        if (errorItems.Count < 1)
                        {
                            ThrowAnError("GC0001", "代码应以\'#\'或\'&\'开始", "对话", 1);
                        }
                    }
                    else
                    {
                        if (errorItems.Count >= 1)
                        {
                            DeleteAnError(1);
                        }
                    }

                    string[] lineText = richTextBox1.Text.Split('\n');
                    for (int i = 0; i < lineText.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(lineText[i]))
                        {
                            string[] vString = lineText[i].Split('|'); //分割文本和音频
                            if (vString.Length == 2) //Length为2时则为一个音频一个对话
                            {
                                if (!string.IsNullOrEmpty(vString[1]))
                                {
                                    if (soundNames.IndexOf(vString[1]) == -1) //找不到资源时
                                    {
                                        if (imageNames.IndexOf(vString[1]) != -1) //在图片类型中找到时
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                            ThrowAnError("GC0006", "图片类型不能应用于音频处", "对话", i + 1);
                                            gc0006Lines.Add((uint)i + 1);
                                        }
                                        else //仍未找到
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                            ThrowAnError("GC0003", $"资源中未包含\"{vString[1]}\"的定义", "对话", i + 1);
                                            gc0003Lines.Add((uint)i + 1);
                                        }
                                    }
                                    else //找到资源时
                                    {
                                        if (gc0003Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                        }
                                        if (gc0006Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                        {
                                            DeleteAnError((uint)i + 1, gc0006Lines);
                                        }
                                    }
                                }
                                else
                                {
                                    if (gc0004Lines.IndexOf((uint)i + 1) == -1) //未报错时
                                    {
                                        ThrowAnError("GC0004", "表达式项\"|\"无效", "对话", i + 1);
                                        gc0004Lines.Add((uint)i + 1);
                                    }
                                }

                                if (lineText[i][0] == '#' || lineText[i][0] == '&')
                                {
                                    if (gc0005Lines.IndexOf((uint)i + 1) == -1) //未报错时
                                    {
                                        ThrowAnError("GC0005", "意外的字符\"|\"", "对话", i + 1);
                                        gc0005Lines.Add((uint)i + 1);
                                    }
                                }

                                if (gc0004Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                {
                                    DeleteAnError((uint)i + 1, gc0004Lines);
                                }
                            }
                            else if (vString.Length >= 3) //一行出现多个‘|’时
                            {
                                if (gc0004Lines.IndexOf((uint)i + 1) == -1) //未报错时
                                {
                                    ThrowAnError("GC0004", "表达式项\"|\"无效", "对话", i + 1);
                                    gc0004Lines.Add((uint)i + 1);
                                }

                                if (lineText[i][0] == '#' || lineText[i][0] == '&')
                                {
                                    if (gc0005Lines.IndexOf((uint)i + 1) == -1) //未报错时
                                    {
                                        ThrowAnError("GC0005", "意外的字符\"|\"", "对话", i + 1);
                                        gc0005Lines.Add((uint)i + 1);
                                    }
                                }
                            }
                            else if (vString.Length != 0)
                            {
                                if (gc0004Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                {
                                    DeleteAnError((uint)i + 1, gc0004Lines);
                                }
                                if (gc0003Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                {
                                    DeleteAnError((uint)i + 1, gc0003Lines);
                                }
                                if (gc0005Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                {
                                    DeleteAnError((uint)i + 1, gc0005Lines);
                                }
                            }

                            if (lineText[i][0] == '&')
                            {
                                string resName = lineText[i].TrimStart('&');
                                if (!string.IsNullOrEmpty(resName))
                                {
                                    if (soundNames.IndexOf(resName) == -1) //找不到资源时
                                    {
                                        if (imageNames.IndexOf(resName) != -1) //在图片类型中找到时
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                            ThrowAnError("GC0006", "图片类型不能应用于音频处", "对话", i + 1);
                                            gc0006Lines.Add((uint)i + 1);
                                        }
                                        else //仍未找到
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                            ThrowAnError("GC0003", $"资源中未包含\"{resName}\"的定义", "对话", i + 1);
                                            gc0003Lines.Add((uint)i + 1);
                                        }
                                    }
                                    else //找到资源时
                                    {
                                        if (gc0003Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                        }
                                        if (gc0006Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                        {
                                            DeleteAnError((uint)i + 1, gc0006Lines);
                                        }
                                    }

                                    if (gc0004Lines.IndexOf((uint)i + 1) != -1)
                                    {
                                        DeleteAnError((uint)i + 1, gc0004Lines);
                                    }
                                }
                                else
                                {
                                    if (gc0004Lines.IndexOf((uint)i + 1) == -1) //未报错时
                                    {
                                        ThrowAnError("GC0004", "表达式项\"&\"无效", "对话", i + 1);
                                        gc0004Lines.Add((uint)i + 1);
                                    }
                                }
                            }

                            if (lineText[i][0] == '~')
                            {
                                string resName = lineText[i].TrimStart('~');
                                if (!string.IsNullOrEmpty(resName))
                                {
                                    if (imageNames.IndexOf(resName) == -1) //找不到资源时
                                    {
                                        if (soundNames.IndexOf(resName) != -1) //在音频类型中找到时
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                            ThrowAnError("GC0006", "音频类型不能应用于图片处", "对话", i + 1);
                                            gc0006Lines.Add((uint)i + 1);
                                        }
                                        else //仍未找到
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                            ThrowAnError("GC0003", $"资源中未包含\"{resName}\"的定义", "对话", i + 1);
                                            gc0003Lines.Add((uint)i + 1);
                                        }
                                    }
                                    else //找到资源时
                                    {
                                        if (gc0003Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                        {
                                            DeleteAnError((uint)i + 1, gc0003Lines);
                                        }
                                        if (gc0006Lines.IndexOf((uint)i + 1) != -1) //已报错时
                                        {
                                            DeleteAnError((uint)i + 1, gc0006Lines);
                                        }
                                    }

                                    if (gc0004Lines.IndexOf((uint)i + 1) != -1)
                                    {
                                        DeleteAnError((uint)i + 1, gc0004Lines);
                                    }
                                }
                                else
                                {
                                    if (gc0004Lines.IndexOf((uint)i + 1) == -1) //未报错时
                                    {
                                        ThrowAnError("GC0004", "表达式项\"~\"无效", "对话", i + 1);
                                        gc0004Lines.Add((uint)i + 1);
                                    }
                                }
                            }

                            if (lineText[i][0] == '%')
                            {
                                if (lineText[i].Length > 1)
                                {
                                    DeleteAnError((uint)i + 1, gc0004Lines);
                                    if (lineText[i][1] != '%')
                                    {
                                        string[] codes = lineText[i].Split('|');
                                        if (codes.Length < 4 || codes.Length > 4)
                                        {
                                            DeleteAnError(i + 1);
                                            if (codes.Length == 2)
                                            {
                                                ThrowAnError("GC0008", "未提供与“显示立绘(资源名|位置|大小|id)”的所需参数“大小”对应的参数", "对话", i + 1);
                                            }
                                            else if (codes.Length == 3)
                                            {
                                                ThrowAnError("GC0008", "未提供与“显示立绘(资源名|位置|大小|id)”的所需参数“id”对应的参数", "对话", i + 1);
                                            }
                                            else
                                            {
                                                ThrowAnError("GC0009", $"“显示立绘”方法没有采用{codes.Length}个参数的重载", "对话", i + 1);
                                            }
                                        }
                                        else if (codes.Length == 4)
                                        {
                                            DeleteAnError(i + 1);
                                            if (imageNames.IndexOf(codes[0].TrimStart('%')) == -1) //找不到图片资源时
                                            {
                                                if (soundNames.IndexOf(codes[0].TrimStart('%')) != -1) //找到音频资源时
                                                {
                                                    //DeleteAnError((uint)i + 1, gc0003Lines);
                                                    ThrowAnError("GC0006", "音频类型不能应用于图片处", "对话", i + 1);
                                                    gc0006Lines.Add((uint)i + 1);
                                                }
                                                else
                                                {
                                                    //DeleteAnError((uint)i + 1, gc0003Lines);
                                                    ThrowAnError("GC0003", $"资源中未包含\"{codes[0].TrimStart('%')}\"的定义", "对话", i + 1);
                                                    gc0003Lines.Add((uint)i + 1);
                                                }
                                            }

                                            if (codes[1] != "Center")
                                            {
                                                if (codes[1] == "center")
                                                {
                                                    DeleteAnError(i + 1);
                                                    ThrowAnError("GC0010", $"类型“位置”中不包含“{codes[1]}”选项或此类格式，是否指“Center”?", "对话", i + 1);
                                                }
                                                else
                                                {
                                                    string[] xyStr = codes[1].Split(',');
                                                    if (xyStr.Length != 2)
                                                    {
                                                        DeleteAnError(i + 1);
                                                        ThrowAnError("GC0010", $"类型“位置”中不包含“{codes[1]}”选项或此类格式", "对话", i + 1);
                                                    }
                                                    else
                                                    {
                                                        int outInt;
                                                        bool status1 = int.TryParse(xyStr[0], out outInt);
                                                        bool status2 = int.TryParse(xyStr[1], out outInt);
                                                        if (status1 == false)
                                                        {
                                                            double outDouble;
                                                            if (!double.TryParse(xyStr[0], out outDouble))
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 1: 无法从文本转换为整数", "对话", i + 1);
                                                            }
                                                            else
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 1: 无法从小数转换为整数", "对话", i + 1);
                                                            }
                                                        }
                                                        if (status2 == false)
                                                        {
                                                            double outDouble;
                                                            if (!double.TryParse(xyStr[0], out outDouble))
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 2: 无法从文本转换为整数", "对话", i + 1);
                                                            }
                                                            else
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 2: 无法从小数转换为整数", "对话", i + 1);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (codes[2] != "ZoomWindow" || codes[2] != "ZoomWindow")
                                            {
                                                if (codes[2] == "zoomwindow" || codes[2] == "Zoomwindow" || codes[2] == "zoomWindow" || codes[2] == "ZoomWindows" || codes[2] == "zoomwindows")
                                                {
                                                    DeleteAnError(i + 1);
                                                    ThrowAnError("GC0010", $"类型“大小”中不包含“{codes[2]}”选项或此类格式，是否指“ZoomWindow”?", "对话", i + 1);
                                                }
                                                else if (codes[2] == "raw")
                                                {
                                                    DeleteAnError(i + 1);
                                                    ThrowAnError("GC0010", $"类型“大小”中不包含“{codes[2]}”选项或此类格式，是否指“Raw”?", "对话", i + 1);
                                                }
                                                else
                                                {
                                                    string[] xyStr = codes[2].Split(',');
                                                    if (xyStr.Length != 2)
                                                    {
                                                        DeleteAnError(i + 1);
                                                        ThrowAnError("GC0010", $"类型“大小”中不包含“{codes[2]}”选项或此类格式", "对话", i + 1);
                                                    }
                                                    else
                                                    {
                                                        int outInt;
                                                        bool status1 = int.TryParse(xyStr[0], out outInt);
                                                        bool status2 = int.TryParse(xyStr[1], out outInt);
                                                        if (status1 == false)
                                                        {
                                                            double outDouble;
                                                            if (!double.TryParse(xyStr[0], out outDouble))
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 1: 无法从文本转换为整数", "对话", i + 1);
                                                            }
                                                            else
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 1: 无法从小数转换为整数", "对话", i + 1);
                                                            }
                                                        }
                                                        if (status2 == false)
                                                        {
                                                            double outDouble;
                                                            if (!double.TryParse(xyStr[0], out outDouble))
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 2: 无法从文本转换为整数", "对话", i + 1);
                                                            }
                                                            else
                                                            {
                                                                DeleteAnError(i + 1);
                                                                ThrowAnError("GC0011", $"参数 2: 无法从小数转换为整数", "对话", i + 1);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            bool status = int.TryParse(codes[3], out int o);
                                            if (!status)
                                            {
                                                if (!double.TryParse(codes[3], out double outDouble))
                                                {
                                                    DeleteAnError(i + 1);
                                                    ThrowAnError("GC0011", $"参数 3: 无法从文本转换为整数", "对话", i + 1);
                                                }
                                                else
                                                {
                                                    DeleteAnError(i + 1);
                                                    ThrowAnError("GC0011", $"参数 3: 无法从小数转换为整数", "对话", i + 1);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        bool status = int.TryParse(lineText[i].TrimStart('%').TrimStart('%'), out int o);
                                        if (!status)
                                        {
                                            if (!double.TryParse(lineText[i].TrimStart('%').TrimStart('%'), out double outDouble))
                                            {
                                                DeleteAnError(i + 1);
                                                ThrowAnError("GC0011", $"参数 0: 无法从文本转换为整数", "对话", i + 1);
                                            }
                                            else
                                            {
                                                DeleteAnError(i + 1);
                                                ThrowAnError("GC0011", $"参数 0: 无法从小数转换为整数", "对话", i + 1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ThrowAnError("GC0004", "表达式项\"%\"无效", "对话", i + 1);
                                    gc0004Lines.Add((uint)i + 1);
                                }
                            }
                        }
                        else //行为空时
                        {
                            if (gc0003Lines.IndexOf((uint)i + 1) != -1) //已报错时
                            {
                                DeleteAnError((uint)i + 1, gc0003Lines);
                            }
                            if (gc0004Lines.IndexOf((uint)i + 1) != -1) //已报错时
                            {
                                DeleteAnError((uint)i + 1, gc0004Lines);
                            }
                            if (gc0006Lines.IndexOf((uint)i + 1) != -1) //已报错时
                            {
                                DeleteAnError((uint)i + 1, gc0006Lines);
                            }
                        }
                    }

                    if (richTextBox1.Text.EndsWith("\n\n"))
                    {
                        DeleteAnError(richTextBox1.Text.Split('\n').Length + 1);
                        DeleteAnError(richTextBox1.Text.Split('\n').Length - 1);
                        ThrowAnError("GC0007", "代码结尾最多有一个空行", "对话", richTextBox1.Text.Split('\n').Length);
                    }
                    else
                    {
                        DeleteAnError(richTextBox1.Text.Split('\n').Length + 1);
                    }
                }

                if (!isFirstPicAdded)
                {
                    if (!isRES001Throwed)
                    {
                        ThrowAnError("RES001", "必须添加起始背景图", "资源", 0);
                        isRES001Throwed = true;
                    }
                }
                else
                {
                    DeleteAnError(0);
                    isRES001Throwed = false;
                }

                if (startpageCheckBox1.Checked && string.IsNullOrEmpty(startpageTextbox1.Text))
                {
                    if (!isSP0001Throwed)
                    {
                        ThrowAnError("SP0001", "必须输入图片路径", "开始界面", -1);
                        isSP0001Throwed = true;
                    }
                }
                else
                {
                    if (isSP0001Throwed)
                    {
                        DeleteAnError(-1);
                        isSP0001Throwed = false;
                    }
                }
                if (startpageCheckBox4.Checked && string.IsNullOrEmpty(startpageTextbox2.Text))
                {
                    if (!isSP0002Throwed)
                    {
                        ThrowAnError("SP0002", "必须输入图片路径", "开始界面", -2);
                        isSP0002Throwed = true;
                    }
                }
                else
                {
                    if (isSP0002Throwed)
                    {
                        DeleteAnError(-2);
                        isSP0002Throwed = false;
                    }
                }
                if (startpageCheckBox5.Checked && string.IsNullOrEmpty(startpageTextbox3.Text))
                {
                    if (!isSP0003Throwed)
                    {
                        ThrowAnError("SP0003", "必须输入图片路径", "开始界面", -3);
                        isSP0003Throwed = true;
                    }
                }
                else
                {
                    if (isSP0003Throwed)
                    {
                        DeleteAnError(-3);
                        isSP0003Throwed = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SaveFiles();
                string exStr = ex.ToString().Replace("\n", "{LF}").Replace("\r", "{CR}").Replace("/", "{Slash}").Replace("\\", "{Backslash}").Replace("%", "{Percent}").Replace("?", "{Question}").Replace("&", "{And}").Replace("$", "{Dollar}").Replace("@", "{At}").Replace("#", "{Hash}");
                BugReporter reporter = new BugReporter("CheckCodes", exStr);
                reporter.ShowDialog();
            }
        }

        /// <summary>
        /// 显示一个错误
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <param name="des">错误说明</param>
        /// <param name="location">错误位置</param>
        /// <param name="line">错误行号</param>
        private void ThrowAnError(string code, string des, string location, int line)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems[0].Text = code;
            item.SubItems.Add(des);
            item.SubItems.Add(location);
            item.SubItems.Add(line.ToString());
            item.ForeColor = Color.White;
            listView1.Items.Add(item);
            errorItems.Add(item);
            //tabControl2.SelectedIndex = 0;
        }
        /// <summary>
        /// 删除指定错误item
        /// </summary>
        /// <param name="item">要删除的item</param>
        private void DeleteAnError(ListViewItem item)
        {
            listView1.Items.Remove(item);
            errorItems.Remove(item);
        }
        /// <summary>
        /// 从错误行号删除指定item
        /// </summary>
        /// <param name="line">错误行号</param>
        private void DeleteAnError(int line)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                ListViewItem item = listView1.Items[i];
                if (item.SubItems[3].Text == line.ToString())
                {
                    listView1.Items.Remove(item);
                    errorItems.Remove(item);
                    break;
                }
            }
        }
        /// <summary>
        /// 从错误行号删除指定item和list
        /// </summary>
        /// <param name="line">错误行号</param>
        /// <param name="list">错误列表</param>
        private void DeleteAnError(uint line, List<uint> list)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                ListViewItem item = listView1.Items[i];
                if (item.SubItems[3].Text == line.ToString())
                {
                    listView1.Items.Remove(item);
                    errorItems.Remove(item);
                    list.Remove(line);
                    break;
                }
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            //tabControl2.SelectedTab = tabPage5;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            toolStripButton6_Click(sender, e);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            CheckCodes();
            toolStripStatusLabel1.Text = " 就绪";
            isSavedLast = false;
            ShowLineNo();
        }

        private void SaveFiles()
        {
            toolStripStatusLabel1.Text = " 正在保存...";
            File.WriteAllText(workSpace + "Dialogs.gc", richTextBox1.Text);
            toolStripStatusLabel1.Text = " 已保存的项";
            isSavedLast = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            About w = new About();
            w.ShowDialog();
        }
        
        private void buildPrint(string text)
        {
            if (text.ToString() == "%Clear%")
            {
                buildOutput = "";
            }
            else
            {
                buildOutput += text.ToString() + "\r\n";
            }
        }
        public static void debugPrint(string text)
        {
            if (text.ToString() == "%Clear%")
            {
                debugOutput = "";
            }
            else
            {
                debugOutput += text.ToString() + "\r\n";
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Build();
        }
        
        private bool Build(bool isHotReload = false)
        {
            try
            {
                statusStrip1.BackColor = Color.FromArgb(64, 53, 130);

                SaveFiles();

                DateTime beforeDt = DateTime.Now;
                if (!isHotReload)
                {
                    //tabControl2.SelectedIndex = 1;
                    comboBox1.SelectedIndex = 0;
                    buildPrint("%Clear%");
                    buildPrint("已启动构建...");
                }

                if (errorItems.Count <= 0)
                {
                    string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
                    string outputPath = workSpace + $"bin\\{exeConfig}\\";
                    if (!isHotReload)
                    {
                        buildPrint($"1>------ 已启动构建: 项目: {projectName}, 配置: {exeConfig} ------");
                        ExtractFile($"GalGM.OutputDatas.base.exe", outputPath + $"{projectName}.exe");
                        ExtractFile($"GalGM.OutputDatas.base.exe.config", outputPath + $"{projectName}.exe.config");
                        ExtractFile($"GalGM.OutputDatas.Microsoft.DirectX.DirectSound.dll", outputPath + "Microsoft.DirectX.DirectSound.dll");
                        ExtractFile($"GalGM.OutputDatas.Microsoft.DirectX.dll", outputPath + "Microsoft.DirectX.dll");
                        ExtractFile($"GalGM.OutputDatas.SevenZipSharp.dll", outputPath + "SevenZipSharp.dll");
                        Directory.CreateDirectory(outputPath + "7z");
                        ExtractFile($"GalGM.OutputDatas.7z.dll", outputPath + "7z\\7z.dll");
                        if (!Directory.Exists(outputPath + "data"))
                        {
                            Directory.CreateDirectory(outputPath + "data");
                        }
                    }
                    if (richTextBox1.Text[richTextBox1.Text.Length - 1] != '\n')
                    {
                        richTextBox1.Text += "\n";
                    }
                    string[] oneCharacterDialogs = richTextBox1.Text.Split('#'); //将代码按人物名分，数组的每一项第一行为人物名，其他为对话内容
                    int lastIndex = 0;
                    string characternameCache = "";
                    string dialogCache = "";
                    string voiceStatCache = "";
                    string voicePathCache = "";
                    for (int i = 0; i < oneCharacterDialogs.Length; i++)
                    {
                        string[] characternameAndDialogs = oneCharacterDialogs[i].Split('\n'); //数组首项为人物名，其他为对话内容
                        lastIndex += characternameAndDialogs.Length - 1; //每次加上数组长度减去第一个人物名
                        for (int i2 = 0; i2 < characternameAndDialogs.Length - 1; i2++)
                        {
                            if (i2 != 0)
                            {
                                characternameCache += Base64Convert(characternameAndDialogs[0], true) + "|"; //重复写入人物名
                                if (characternameAndDialogs[i2][0] != '&' && characternameAndDialogs[i2][0] != '^' && characternameAndDialogs[i2][0] != '~' && characternameAndDialogs[i2][0] != '%')
                                {
                                    string[] vo = characternameAndDialogs[i2].Split('|');
                                    if (vo.Length == 2)
                                    {
                                        dialogCache += Base64Convert(vo[0], true) + "|"; //重复写入对话内容
                                        voicePathCache += $"./assets/{vo[1]}.wav|"; //重复写入音频路径
                                        voiceStatCache += "true|"; //重复将本次音频状态设为true
                                    }
                                    else
                                    {
                                        dialogCache += Base64Convert(characternameAndDialogs[i2], true) + "|"; //重复写入对话内容
                                        voicePathCache += "nil|"; //音频路径添加nil标记
                                        voiceStatCache += "false|"; //重复将本次音频状态设为false
                                    }
                                }
                            }
                        }
                    }
                    string[] lineText = richTextBox1.Text.Split('\n'); //每行的内容
                    string bgMusicPartCache = "";
                    string bgMusicPartPathCache = "";
                    string bgPicPartCache = "";
                    string bgPicPartPathCache = "";
                    bool isUsingBGM = false;
                    int m = 0;
                    string showMorePicIndexCache = "";
                    string showMorePicPathCache = "";
                    string showMorePicLoactionCache = "";
                    string showMorePicSizeCache = "";
                    string showMorePicDisCache = "";
                    string showMorePicIdCache = "";
                    for (int i = 0; i < lineText.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(lineText[i]))
                        {
                            if (lineText[i][0] == '#' || lineText[i][0] == '&' || lineText[i][0] == '^' || lineText[i][0] == '~' || lineText[i][0] == '%')
                            {
                                m++;
                            }
                            if (lineText[i][0] == '&') //首字符为&时
                            {
                                bgMusicPartCache += (i + 1 - m).ToString() + "|";
                                string mName = lineText[i].TrimStart('&'); //资源名
                                int index = soundNames.IndexOf(mName);
                                if (index != -1)
                                {
                                    bgMusicPartPathCache += $"./assets/{@soundNames[index]}.wav|";
                                    isUsingBGM = true;
                                }
                            }

                            if (lineText[i][0] == '~') //首字符为~时
                            {
                                bgPicPartCache += (i + 1 - m).ToString() + "|";
                                string mName = lineText[i].TrimStart('~'); //资源名
                                int index = imageNames.IndexOf(mName);
                                if (index != -1)
                                {
                                    Directory.CreateDirectory($"{workSpace}bin\\{exeConfig}\\assets");
                                    File.Copy(imagePaths[index], $"{workSpace}bin\\{exeConfig}\\assets\\{imageNames[index]}.png", true);
                                    bgPicPartPathCache += $"./assets/{imageNames[index]}.png|";
                                }
                            }

                            if (lineText[i][0] == '%') //首字母为%时
                            {
                                if (lineText[i][1] != '%')
                                {
                                    string[] codes = lineText[i].Split('|');
                                    showMorePicIndexCache += (i - m + 1).ToString() + "|";
                                    int index = imageNames.IndexOf(codes[0].TrimStart('%'));
                                    if (index != -1)
                                    {
                                        File.Copy(imagePaths[index], $"{workSpace}bin\\{exeConfig}\\assets\\{imageNames[index]}.png", true);
                                        showMorePicPathCache += $"./assets/{imageNames[index]}.png|";
                                    }
                                    showMorePicLoactionCache += $"{codes[1]}|";
                                    showMorePicSizeCache += $"{codes[2]}|";
                                    showMorePicIdCache += $"{codes[3]}|";
                                }
                                else
                                {
                                    int id = int.Parse(lineText[i].TrimStart('%').TrimStart('%'));
                                    showMorePicDisCache += $"{id}|";
                                }
                            }
                        }
                    }
                    bool isShowMainPic = startpageCheckBox1.Checked;
                    string mainPicPath = "nil";
                    bool isModifyMainButton = startpageCheckBox2.Checked;
                    bool isModifyButtonBgPic = startpageCheckBox4.Checked;
                    string mainButtonBgPicPath = "nil";
                    bool isReversalMainButtonTextColor = startpageCheckBox3.Checked;
                    bool isUsingMainBGM = startpageCheckBox5.Checked;
                    string mainBGMPath = "nil";
                    if (isShowMainPic)
                    {
                        File.Copy(startpageTextbox1.Text, $"{outputPath}assets\\{startpageTextbox1.Text.Split('\\')[startpageTextbox1.Text.Split('\\').Length - 1]}");
                        mainPicPath = $"./assets/{startpageTextbox1.Text.Split('\\')[startpageTextbox1.Text.Split('\\').Length - 1]}";
                    }
                    if (isModifyMainButton)
                    {
                        if (isModifyButtonBgPic)
                        {
                            File.Copy(startpageTextbox2.Text, $"{outputPath}assets\\{startpageTextbox2.Text.Split('\\')[startpageTextbox2.Text.Split('\\').Length - 1]}");
                            mainButtonBgPicPath = $"./assets/{startpageTextbox2.Text.Split('\\')[startpageTextbox2.Text.Split('\\').Length - 1]}";
                        }
                    }
                    if (isUsingMainBGM)
                    {
                        File.Copy(startpageTextbox3.Text, $"{outputPath}assets\\{startpageTextbox3.Text.Split('\\')[startpageTextbox3.Text.Split('\\').Length - 1]}");
                        mainBGMPath = $"./assets/{startpageTextbox3.Text.Split('\\')[startpageTextbox3.Text.Split('\\').Length - 1]}";
                    }
                    //修正
                    for (int i = 0; i < richTextBox1.Text.Length; i++)
                    {
                        if (richTextBox1.Text[i] == '#')
                        {
                            lastIndex--;
                        }
                    }
                    for (int i = 0; i < m; i++)
                    {
                        voicePathCache += "nil|";
                        voiceStatCache += "false|";
                    }
                    int lT = 0;
                    for (int i = 0; i < bgMusicPartCache.Length; i++)
                    {
                        if (bgMusicPartCache[i] == '|')
                        {
                            lT++;
                        }
                    }
                    if (lT == 1)
                    {
                        bgMusicPartCache += $"{lastIndex}|";
                    }
                    int lP = 0;
                    for (int i = 0; i < bgPicPartCache.Length; i++)
                    {
                        if (bgPicPartCache[i] == '|')
                        {
                            lP++;
                        }
                    }
                    if (lP == 1)
                    {
                        bgPicPartCache += $"{lastIndex}|";
                    }
                    if (string.IsNullOrEmpty(bgMusicPartCache))
                    {
                        bgMusicPartCache = "0|1|";
                    }
                    if (string.IsNullOrEmpty(bgPicPartCache))
                    {
                        bgPicPartCache = "0|1|";
                    }
                    lastIndex = dialogCache.Split('|').Length;
                    voiceStatCache = voiceStatCache.Remove(voiceStatCache.Length - m * 6, m * 6);
                    voicePathCache = voicePathCache.Remove(voicePathCache.Length - m * 4, m * 4);
                    //删除最后一个多余的“|”
                    characternameCache = characternameCache.TrimEnd('|');
                    dialogCache = dialogCache.TrimEnd('|');
                    voiceStatCache = voiceStatCache.TrimEnd('|');
                    voicePathCache = voicePathCache.TrimEnd('|');
                    bgMusicPartCache = bgMusicPartCache.TrimEnd('|');
                    bgMusicPartPathCache = bgMusicPartPathCache.TrimEnd('|');
                    bgPicPartCache = bgPicPartCache.TrimEnd('|');
                    bgPicPartPathCache = bgPicPartPathCache.TrimEnd('|');
                    showMorePicIndexCache = showMorePicIndexCache.TrimEnd('|');
                    showMorePicPathCache = showMorePicPathCache.TrimEnd('|');
                    showMorePicLoactionCache = showMorePicLoactionCache.TrimEnd('|');
                    showMorePicSizeCache = showMorePicSizeCache.TrimEnd('|');
                    showMorePicDisCache = showMorePicDisCache.TrimEnd('|');
                    showMorePicIdCache = showMorePicIdCache.TrimEnd('|');
                    //写入文件
                    //游戏
                    INIHelper.Write("Base", "lastIndex", lastIndex.ToString(), outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Character", "names", characternameCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Dialog", "texts", dialogCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Voice", "status", voiceStatCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Voice", "paths", voicePathCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Base", "isUsingBGM", isUsingBGM.ToString().ToLower(), outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Base", "bgMusicPart", bgMusicPartCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Base", "bgMusicPartPath", bgMusicPartPathCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Base", "bgPicPart", bgPicPartCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Base", "bgPicPartPath", bgPicPartPathCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Base", "firstBgPicPath", INIHelper.Read("Image", "firstPic", "nil", $"{workSpace}Resources.resg"), outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("Base", "isEnc", "false", outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("MorePic", "index", showMorePicIndexCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("MorePic", "path", showMorePicPathCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("MorePic", "location", showMorePicLoactionCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("MorePic", "size", showMorePicSizeCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("MorePic", "disIndex", showMorePicDisCache, outputPath + "data\\gConfig.wggproj");
                    INIHelper.Write("MorePic", "id", showMorePicIdCache, outputPath + "data\\gConfig.wggproj");
                    //主菜单
                    INIHelper.Write("Base", "isShowMainPic", isShowMainPic.ToString(), outputPath + "data\\mConfig.wggproj");
                    INIHelper.Write("Base", "mainPicPath", mainPicPath, outputPath + "data\\mConfig.wggproj");
                    INIHelper.Write("Base", "isPlayMainBGM", isUsingMainBGM.ToString(), outputPath + "data\\mConfig.wggproj");
                    INIHelper.Write("Base", "mainSoundPath", mainBGMPath, outputPath + "data\\mConfig.wggproj");
                    INIHelper.Write("Button", "isModifyBgPic", isModifyButtonBgPic.ToString(), outputPath + "data\\mConfig.wggproj");
                    INIHelper.Write("Button", "bgPicPath", mainButtonBgPicPath, outputPath + "data\\mConfig.wggproj");
                    INIHelper.Write("Button", "isReversalTextColor", isReversalMainButtonTextColor.ToString(), outputPath + "data\\mConfig.wggproj");
                    //拷贝文件
                    for (int i = 0; i < soundPaths.Count; i++)
                    {
                        File.Copy(soundPaths[i], $"{workSpace}bin\\{exeConfig}\\assets\\{@soundNames[i]}.wav", true);
                    }
                    //加密文件
                    if (isEncryption)
                    {
                        List<string> encFiles = new List<string>();
                        DirectoryInfo di = new DirectoryInfo($"{outputPath}assets");
                        FileSystemInfo[] fsInfos = di.GetFileSystemInfos();
                        foreach (FileSystemInfo fsInfo in fsInfos)
                        {
                            if (fsInfo is DirectoryInfo)
                            { //是文件夹时

                            }
                            else
                            {
                                encFiles.Add(fsInfo.FullName);
                            }
                        }
                        R7z archiver = new R7z();
                        archiver.CompressFilesEncrypted("assets.gef", "GalgameFromGalGM1145141919810%", encFiles.ToArray());
                        if (File.Exists("./assets.gef"))
                        {
                            File.Copy("./assets.gef", $"{outputPath}assets.gef", true);
                            File.Delete("./assets.gef");
                            Directory.Delete($"{outputPath}assets", true);
                            INIHelper.Write("Base", "isEnc", "true", outputPath + "data\\gConfig.wggproj");
                        }
                    }
                    if (!isHotReload)
                    {
                        buildPrint($"1>  {projectName} -> {outputPath}{projectName}.exe");
                        buildPrint("========== 构建成功 ==========");

                        DateTime afterDt = DateTime.Now;
                        TimeSpan ts = afterDt.Subtract(beforeDt);
                        buildPrint($"========= 构建开始于 {beforeDt}，花费了 {ts} ==========");
                    }
                    BuildStatus w = new BuildStatus(true);
                    w.Show();
                    statusStrip1.BackColor = Color.FromArgb(66, 66, 66);
                    return true;
                }
                else
                {
                    for (int i = 0; i < errorItems.Count; i++)
                    {
                        buildPrint($"1>{workSpace}Dialogs.gc({errorItems[i].SubItems[3].Text}): error {errorItems[i].SubItems[0].Text}: {errorItems[i].SubItems[1].Text}");
                    }
                    buildPrint("========== 构建失败 ==========");
                    DateTime afterDt = DateTime.Now;
                    TimeSpan ts = afterDt.Subtract(beforeDt);
                    buildPrint($"========= 构建开始于 {beforeDt}，花费了 {ts} ==========");
                    BuildStatus w = new BuildStatus(false);
                    w.Show();
                    statusStrip1.BackColor = Color.FromArgb(66, 66, 66);
                    return false;
                }
            }
            catch (Exception ex)
            {
                SaveFiles();
                string exStr = ex.ToString().Replace("\n", "{LF}").Replace("\r", "{CR}").Replace("/", "{Slash}").Replace("\\", "{Backslash}").Replace("%", "{Percent}").Replace("?", "{Question}").Replace("&", "{And}").Replace("$", "{Dollar}").Replace("@", "{At}").Replace("#", "{Hash}");
                BugReporter reporter = new BugReporter("Build", exStr);
                reporter.ShowDialog();
                return false;
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 0)
            {
                exeConfig = "Debug";
                INIHelper.Write("Base", "exeConfig", "Debug", nowPath);
            }
            else
            {
                exeConfig = "Release";
                INIHelper.Write("Base", "exeConfig", "Release", nowPath);
            }
        }

        /// <summary>
        /// 释放内嵌资源至指定位置
        /// </summary>
        /// <param name="resource">嵌入的资源，此参数写作：命名空间.文件夹名.文件名.扩展名</param>
        /// <param name="path">释放到位置</param>
        private void ExtractFile(String resource, String path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            BufferedStream input = new BufferedStream(assembly.GetManifestResourceStream(resource));
            FileStream output = new FileStream(path, FileMode.Create);
            byte[] data = new byte[1024];
            int lengthEachRead;
            while ((lengthEachRead = input.Read(data, 0, data.Length)) > 0)
            {
                output.Write(data, 0, lengthEachRead);
            }
            output.Flush();
            output.Close();
        }

        /// <summary>
        /// Base64编解码
        /// </summary>
        /// <param name="text">原文本或Base64文本</param>
        /// <param name="isEncode">转换方向，true为编码到Base64</param>
        /// <returns></returns>
        private string Base64Convert(string text, bool isEncode)
        {
            if (isEncode)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
            }
            else
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(text));
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                isCloseDebugging = false;

                INIHelper.Write("Base", "status", "true", $"{workSpace}bin\\{exeConfig}\\GalGR.gds");

                string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
                string exePath = workSpace + $"bin\\{exeConfig}\\";
                if (Build())
                {
                    Process pc = new Process();
                    pc.StartInfo.FileName = "cmd.exe";
                    pc.StartInfo.CreateNoWindow = true; //隐藏窗口运行
                    pc.StartInfo.RedirectStandardError = true; //重定向错误流
                    pc.StartInfo.RedirectStandardInput = true; //重定向输入流
                    pc.StartInfo.RedirectStandardOutput = true; //重定向输出流
                    pc.StartInfo.UseShellExecute = false;
                    pc.Start();
                    pc.StandardInput.WriteLine($"cd {exePath}");
                    pc.StandardInput.WriteLine($"{projectName}.exe");
                    statusStrip1.BackColor = Color.FromArgb(134, 27, 45);
                    toolStripButton12.Enabled = true;
                    toolStripButton11.Enabled = true;
                    toolStripButton3.Enabled = false;
                    toolStripButton4.Enabled = false;
                    toolStripButton10.Enabled = false;
                    //tabControl2.SelectedIndex = 1;
                    comboBox1.SelectedIndex = 1;
                    Thread.Sleep(200);
                    isRunning = true;
                }
                else
                {
                    if (File.Exists(exePath))
                    {
                        if (MessageBox.Show("发生构建错误。是否继续并运行上次的成功生成?", "GalGM", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            Process pc = new Process();
                            pc.StartInfo.FileName = "cmd.exe";
                            pc.StartInfo.CreateNoWindow = true; //隐藏窗口运行
                            pc.StartInfo.RedirectStandardError = true; //重定向错误流
                            pc.StartInfo.RedirectStandardInput = true; //重定向输入流
                            pc.StartInfo.RedirectStandardOutput = true; //重定向输出流
                            pc.StartInfo.UseShellExecute = false;
                            pc.Start();
                            pc.StandardInput.WriteLine($"cd {exePath}");
                            pc.StandardInput.WriteLine($"{projectName}.exe");
                            statusStrip1.BackColor = Color.FromArgb(134, 27, 45);
                            toolStripButton12.Enabled = true;
                            toolStripButton11.Enabled = true;
                            toolStripButton3.Enabled = false;
                            toolStripButton4.Enabled = false;
                            toolStripButton10.Enabled = false;
                            //tabControl2.SelectedIndex = 1;
                            comboBox1.SelectedIndex = 1;
                            Thread.Sleep(200);
                            isRunning = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SaveFiles();
                string exStr = ex.ToString().Replace("\n", "{LF}").Replace("\r", "{CR}").Replace("/", "{Slash}").Replace("\\", "{Backslash}").Replace("%", "{Percent}").Replace("?", "{Question}").Replace("&", "{And}").Replace("$", "{Dollar}").Replace("@", "{At}").Replace("#", "{Hash}");
                BugReporter reporter = new BugReporter("StartDebugging", exStr);
                reporter.ShowDialog();
            }
        }

        public string getRoot()
        {
            string rootfilePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            int iIndex = rootfilePath.LastIndexOf("\\");
            rootfilePath = rootfilePath.Substring(0, iIndex + 1);
            return rootfilePath;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            toolStripButton3_Click(sender, e);
            isCloseDebugging = true;
            Thread.Sleep(200);
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            Build(true);
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            isStopDebug = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "打开";
            dialog.Filter = "所有图像文件(*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                if (File.Exists(path))
                {
                    string fName = path.Split('\\')[path.Split('\\').Length - 1].Split('.')[0];
                    string fBack = path.Split('\\')[path.Split('\\').Length - 1].Split('.')[1];
                    EnterFileName w = new EnterFileName(fName);
                    if (w.ShowDialog() == DialogResult.OK)
                    {
                        string name = w.GetName;
                        string newPath = $"{workSpace}assets\\{name}.{fBack}";
                        File.Copy(path, newPath, true);
                        FileStream fs = File.OpenRead(newPath);
                        long fileLength = fs.Length;
                        byte[] image = new byte[fileLength];
                        fs.Read(image, 0, (int)fileLength);
                        Image result = Image.FromStream(fs);
                        imageList1.Images.Add(result);
                        listView2.Items.Add(name, imageList1.Images.Count - 1);
                        int total = int.Parse(INIHelper.Read("Image", "total", "0", $"{workSpace}Resources.resg")) + 1;
                        INIHelper.Write("Image", "total", total.ToString(), $"{workSpace}Resources.resg");
                        INIHelper.Write("Image", $"path{total}", newPath, $"{workSpace}Resources.resg");
                        INIHelper.Write("Image", $"name{total}", name, $"{workSpace}Resources.resg");
                        imagePaths.Add(newPath);
                        imageNames.Add(name);
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "打开";
            dialog.Filter = "Wav音频文件(*.wav)|*.wav";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                if (File.Exists(path))
                {
                    string fName = path.Split('\\')[path.Split('\\').Length - 1].Split('.')[0];
                    string fBack = "wav";
                    EnterFileName w = new EnterFileName(fName);
                    if (w.ShowDialog() == DialogResult.OK)
                    {
                        string name = w.GetName;
                        string newPath = $"{workSpace}assets\\{name}.{fBack}";
                        File.Copy(path, newPath, true);
                        ListViewItem item = new ListViewItem();
                        item.SubItems[0].Text = name;
                        item.SubItems.Add(newPath);
                        item.ForeColor = Color.White;
                        listView3.Items.Add(item);
                        int total = int.Parse(INIHelper.Read("Sound", "total", "0", $"{workSpace}Resources.resg")) + 1;
                        INIHelper.Write("Sound", "total", total.ToString(), $"{workSpace}Resources.resg");
                        INIHelper.Write("Sound", $"path{total}", newPath, $"{workSpace}Resources.resg");
                        INIHelper.Write("Sound", $"name{total}", name, $"{workSpace}Resources.resg");
                        soundPaths.Add(newPath);
                        soundNames.Add(name);
                    }
                }
            }
        }

        /// <summary>
        /// 选中行
        /// </summary>
        /// <param name="line">行号,从0开始</param>
        private void SelectLine(int line)
        {
            int a = richTextBox1.GetFirstCharIndexFromLine(line);
            int b = richTextBox1.GetFirstCharIndexFromLine(++line);
            if (a == -1)
                return;
            else if (b == -1)
                b = richTextBox1.TextLength - a;
            else
                b = b - a;
            richTextBox1.Select(a, b);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "打开";
            dialog.Filter = "所有图像文件(*.bmp, *.jpg, *.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                if (File.Exists(path))
                {
                    INIHelper.Write("Image", $"firstPic", path, $"{workSpace}Resources.resg");
                    isFirstPicAdded = true;
                }
            }
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFiles();
        }

        private void 启动窗口WToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 w = new Form1(new string[0]);
            w.Show();
            this.Close();
        }

        private void 生成ProjectNameUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Build();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                isEncryption = true;
                INIHelper.Write("Setting", "isEncryption", "true", nowPath);
            }
            else
            { 
                isEncryption = false;
                INIHelper.Write("Setting", "isEncryption", "false", nowPath);
            }
        }

        private void EditMain_Resize(object sender, EventArgs e)
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

        private void 反编译到CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
            string outputPath = workSpace + $"bin\\{exeConfig}\\";
            ILSDecompile decompiler = new ILSDecompile(outputPath + $"{projectName}.exe") { TabText = "反编译" };
            decompiler.Show(this.dockPanel1, DockState.Document);
        }

        private void ShowLineNo()
        {
            //获得当前坐标信息
            Point p = this.richTextBox1.Location;
            int crntFirstIndex = this.richTextBox1.GetCharIndexFromPosition(p);

            int crntFirstLine = this.richTextBox1.GetLineFromCharIndex(crntFirstIndex);

            Point crntFirstPos = this.richTextBox1.GetPositionFromCharIndex(crntFirstIndex);

            p.Y += this.richTextBox1.Height;

            int crntLastIndex = this.richTextBox1.GetCharIndexFromPosition(p);

            int crntLastLine = this.richTextBox1.GetLineFromCharIndex(crntLastIndex);
            Point crntLastPos = this.richTextBox1.GetPositionFromCharIndex(crntLastIndex);

            //准备画图
            Graphics g = this.panel1.CreateGraphics();

            Font font = new Font(this.richTextBox1.Font, this.richTextBox1.Font.Style);

            SolidBrush brush = new SolidBrush(Color.Green);

            //画图开始

            //刷新画布

            Rectangle rect = this.panel1.ClientRectangle;
            brush.Color = this.panel1.BackColor;

            g.FillRectangle(brush, 0, 0, this.panel1.ClientRectangle.Width, this.panel1.ClientRectangle.Height);

            brush.Color = Color.FromArgb(123, 138, 122); //重置画笔颜色

            //绘制行号

            int lineSpace = 0;

            if (crntFirstLine != crntLastLine)
            {
                lineSpace = (crntLastPos.Y - crntFirstPos.Y) / (crntLastLine - crntFirstLine);

            }

            else
            {
                lineSpace = Convert.ToInt32(this.richTextBox1.Font.Size);

            }

            int brushX = this.panel1.ClientRectangle.Width - Convert.ToInt32(font.Size * 3);

            int brushY = crntLastPos.Y + Convert.ToInt32(font.Size * 0.21f);
            for (int i = crntLastLine; i >= crntFirstLine; i--)
            {
                g.DrawString((i + 1).ToString(), font, brush, brushX, brushY);

                brushY -= lineSpace;
            }

            g.Dispose();

            font.Dispose();

            brush.Dispose();
        }

        private void richTextBox1_VStroll(object sender, EventArgs e)
        {
            ShowLineNo();
        }

        private void 启动调试SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton3_Click(sender, e);
        }

        private void 开始执行不调试HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton10_Click(sender, e);
        }

        private void 查看帮助VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://galgmdoc.darock.top/");
        }

        private void 关于GalGMAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About w = new About();
            w.ShowDialog();
        }

        private void StartPageSetting_checkbox1_CheckedChanged(object sender, EventArgs e)
        {
            if (startpageCheckBox1.Checked)
            {
                INIHelper.Write("Profile", "isUseStartPic", "true", $"{workSpace}StartPage.prof");
            }
            else
            {
                INIHelper.Write("Profile", "isUseStartPic", "false", $"{workSpace}StartPage.prof");
            }
            CheckCodes();
        }

        private void StartPageSetting_checkbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (startpageCheckBox2.Checked)
            {
                INIHelper.Write("Profile", "isModifyButton", "true", $"{workSpace}StartPage.prof");
            }
            else
            {
                INIHelper.Write("Profile", "isModifyButton", "false", $"{workSpace}StartPage.prof");
            }
        }

        private void StartPageSetting_checkbox3_CheckedChanged(object sender, EventArgs e)
        {
            if (startpageCheckBox3.Checked)
            {
                INIHelper.Write("Profile", "isReversalTextColor", "true", $"{workSpace}StartPage.prof");
            }
            else
            {
                INIHelper.Write("Profile", "isReversalTextColor", "false", $"{workSpace}StartPage.prof");
            }
        }

        private void StartPageSetting_checkbox4_CheckedChanged(object sender, EventArgs e)
        {
            if (startpageCheckBox4.Checked)
            {
                INIHelper.Write("Profile", "isModifyButtonPic", "true", $"{workSpace}StartPage.prof");
            }
            else
            {
                INIHelper.Write("Profile", "isModifyButtonPic", "false", $"{workSpace}StartPage.prof");
            }
            CheckCodes();
        }

        private void StartPageSetting_checkbox5_CheckedChanged(object sender, EventArgs e)
        {
            if (startpageCheckBox5.Checked)
            {
                INIHelper.Write("Profile", "isUsingBGM", "true", $"{workSpace}StartPage.prof");
            }
            else
            {
                INIHelper.Write("Profile", "isUsingBGM", "false", $"{workSpace}StartPage.prof");
            }
            CheckCodes();
        }

        private void StartPageSetting_textBox1_TextChanged(object sender, EventArgs e)
        {
            INIHelper.Write("Profile", "picPath", startpageTextbox1.Text, $"{workSpace}StartPage.prof");
            CheckCodes();
        }

        private void StartPageSetting_textBox2_TextChanged(object sender, EventArgs e)
        {
            INIHelper.Write("Profile", "buttonBgPath", startpageTextbox1.Text, $"{workSpace}StartPage.prof");
            CheckCodes();
        }

        private void StartPageSetting_textBox3_TextChanged(object sender, EventArgs e)
        {
            INIHelper.Write("Profile", "bgmPath", startpageTextbox1.Text, $"{workSpace}StartPage.prof");
            CheckCodes();
        }

        private void 选项OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorConfig w = new EditorConfig();
            w.ShowDialog();
        }

        //窗体缩放
        const int Guying_HTLEFT = 10;
        const int Guying_HTRIGHT = 11;
        const int Guying_HTTOP = 12;
        const int Guying_HTTOPLEFT = 13;
        const int Guying_HTTOPRIGHT = 14;
        const int Guying_HTBOTTOM = 15;
        const int Guying_HTBOTTOMLEFT = 0x10;
        const int Guying_HTBOTTOMRIGHT = 17;
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0084:
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMLEFT;
                        else m.Result = (IntPtr)Guying_HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMRIGHT;
                        else m.Result = (IntPtr)Guying_HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)Guying_HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)Guying_HTBOTTOM;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override CreateParams CreateParams   //防止改变窗口大小时控件闪烁
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void EditMain_MouseUp(object sender, MouseEventArgs e)
        {
            Point ms = Control.MousePosition;
            if (ms.Y <= 4)
            {
                button2_Click(sender, e);
            }
            Debug.WriteLine(ms);
        }

        /// <summary>
        /// 指定Url地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求链接地址</param>
        /// <param name="isFix">是否进行Darock Api修正</param>
        /// <returns></returns>
        public static string NetGet(string url, bool isFix = true)
        {
            string result = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                try
                {
                    //获取内容
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                finally
                {
                    stream.Close();
                }
                if (isFix)
                {
                    result = result.TrimStart('\"').TrimEnd('\"');
                }
            }
            catch
            { 
                
            }
            return result;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            string newVerLink = NetGet("http://api.darock.top/galgm/latest");
            Process.Start(newVerLink);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (isHaveAppUpdate)
            {
                groupBox1.Visible = true;
                timer2.Enabled = false;
            }
        }
    }
}
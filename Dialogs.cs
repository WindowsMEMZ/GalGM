using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    public delegate void DelegateTextChanged(object sender, EventArgs e);
    public delegate void DelegateVStrolled(object sender, EventArgs e);

    public partial class Dialogs : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event DelegateTextChanged TextChanged;
        public event DelegateButtonClicked Button5Clicked;
        public event DelegateButtonClicked Button6Clicked;
        public event DelegateVStrolled RichTextBox1VStroll;

        private int textBoxSecStart;
        private string workSpace;
        private int textBoxCcSecStart;
        private int ccTimes;

        public Dialogs(string wS = "")
        {
            workSpace = wS;

            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }

        public string GetText
        {
            get { return richTextBox1.Text; }
            set { richTextBox1.Text = value; }
        }
        public RichTextBox GetTextBox
        {
            get { return richTextBox1; }
        }
        public ToolStripLabel GetToolStripButton5
        {
            get { return toolStripButton5; }
        }
        public ToolStripLabel GetToolStripButton6
        {
            get { return toolStripButton6; }
        }
        public ToolStripLabel GetToolStripButton7
        {
            get { return toolStripButton7; }
        }
        public ToolStripLabel GetToolStripButton8
        {
            get { return toolStripButton8; }
        }
        public ToolStripLabel GetToolStripButton9
        {
            get { return toolStripButton9; }
        }
        public Panel GetPanel
        {
            get { return panel1; }
        }

        private void Dialogs_Load(object sender, EventArgs e)
        {
            KeywordsInitialize();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            TextChanged(sender, e);
            textBoxSecStart = richTextBox1.SelectionStart;
            RichHighlight(0);
            //richTextBox1.SelectionStart = textBoxSecStart;
            richTextBox1.Select(textBoxSecStart, 0);
            richTextBox1.ScrollToCaret();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Button5Clicked(sender, e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Button6Clicked(sender, e);
        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            RichTextBox1VStroll(sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            RichTextBox1VStroll(sender, e);
        }

        Hashtable keywordsDef = new Hashtable();
        private void KeywordsInitialize()
        {
            keywordsDef.Add("#", "1");
            keywordsDef.Add("&", "1");
            keywordsDef.Add("~", "1");
        }
        /// <summary>
        /// C#语法高亮着色器
        /// </summary>
        /// <param name="start">起始行号</param>
        private void RichHighlight(int start)
        {
            string[] ln = richTextBox1.Text.Split('\n');
            int pos = 0;
            int lnum = 0;
            foreach (string lv in ln)
            {
                if (lnum >= start)
                {
                    string ts = lv.Replace("(", " ").Replace(")", " ");
                    ts = ts.Replace("[", " ").Replace("]", " ");
                    ts = ts.Replace("{", " ").Replace("}", " ");
                    ts = ts.Replace(".", " ").Replace("=", " ").Replace(";", " ");
                    if (lv.Trim().StartsWith("^"))
                    {
                        richTextBox1.Select(pos, lv.Length);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(96, 139, 78);
                        pos += lv.Length + 1;
                        continue;
                    }
                    if (lv.Trim().StartsWith("#"))
                    {
                        richTextBox1.Select(pos, 1);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(86, 156, 214);

                        richTextBox1.Select(pos + 1, lv.Length);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(220, 220, 170);
                        pos += lv.Length + 1;
                        continue;
                    }
                    if (lv.Trim().StartsWith("&") || lv.Trim().StartsWith("~"))
                    {
                        richTextBox1.Select(pos, 1);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(86, 156, 214);

                        richTextBox1.Select(pos + 1, lv.Length);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(156, 220, 254);
                        pos += lv.Length + 1;
                        continue;
                    }
                    int voiceTimes = 0;
                    for (int i = 0; i < lv.Trim().Length; i++)
                    {
                        if (lv.Trim()[i] == '|')
                        {
                            voiceTimes++;
                            break;
                        }
                    }
                    if (voiceTimes >= 1)
                    {
                        int[] l = { lv.Split('|')[0].Length, lv.Split('|')[1].Length };

                        richTextBox1.Select(pos, l[0]);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(57, 190, 163);

                        richTextBox1.Select(pos + l[0], 1);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(86, 156, 214);

                        richTextBox1.Select(pos + l[0] + 1, l[1] + 1);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.FromArgb(156, 220, 254);
                        pos += lv.Length + 1;
                        continue;
                    }
                    ArrayList marks = new ArrayList();
                    string smark = "";
                    string last = "";
                    bool inmark = false;
                    for (int i = 0; i < ts.Length; i++)
                    {
                        if (ts.Substring(i, 1) == "\"" && last != "\\")
                        {
                            if (inmark)
                            {
                                marks.Add(smark + "," + i);
                                smark = "";
                                inmark = false;
                            }
                            else
                            {
                                smark += i;
                                inmark = true;
                            }
                        }
                        last = ts.Substring(i, 1);
                    }
                    if (inmark)
                    {
                        marks.Add(smark + "," + ts.Length);
                    }
                    string[] ta = ts.Split(' ');
                    int x = 0;
                    foreach (string tv in ta)
                    {
                        if (tv.Length < 2)
                        {
                            x += tv.Length + 1;
                            continue;
                        }
                        else
                        {
                            bool find = false;
                            foreach (string px in marks)
                            {
                                string[] pa = px.Split(',');
                                if (x >= Int32.Parse(pa[0]) && x < Int32.Parse(pa[1]))
                                {
                                    find = true;
                                    break;
                                }
                            }
                            if (!find)
                            {
                                if (keywordsDef[tv] != null)
                                {
                                    richTextBox1.Select(pos + x, tv.Length);
                                    richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                                    richTextBox1.SelectionColor = Color.FromArgb(86, 156, 214);
                                }
                            }
                            x += tv.Length + 1;
                        }
                    }
                    foreach (string px in marks)
                    {
                        string[] pa = px.Split(',');
                        richTextBox1.Select(pos + Int32.Parse(pa[0]), Int32.Parse(pa[1]) - Int32.Parse(pa[0]) + 1);
                        richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
                        richTextBox1.SelectionColor = Color.DarkRed;
                    }
                }
                pos += lv.Length + 1;
                lnum++;
            }
            // 设置一下，才能恢复；后续正确！
            richTextBox1.Select(0, 1);
            richTextBox1.SelectionFont = new Font("Consolas", 10.5f, (FontStyle.Regular));
            richTextBox1.SelectionColor = Color.White;
        }

        private void CodeCompletion(object sender, KeyEventArgs e)
        {
            RichTextBox tb = (RichTextBox)sender;
            if (e.KeyValue == 192)
            {
                Control[] c = tb.Controls.Find("cc", false);
                if (c.Length > 0)
                {
                    ((ListBox)c[0]).Dispose();
                }
                ListBox lb = new ListBox();
                lb.Name = "cc";
                lb.BackColor = Color.FromArgb(31, 31, 31);
                lb.ForeColor = Color.White;
                lb.Size = new Size(448, 203);
                lb.Cursor = Cursors.Default;
                string path = $"{workSpace}assets";
                string[] files = Directory.GetFiles(path, "*.png");
                List<string> fileNames = new List<string>();
                foreach (var file in files)
                {
                    string name = file.Split('\\')[file.Split('\\').Length - 1].Split('.')[0];
                    fileNames.Add(name);
                }
                for (int i = 0; i < fileNames.Count; i++)
                {
                    lb.Items.Add(fileNames[i]);
                }
                lb.SelectedIndex = 0;
                lb.Show();
                lb.Location = new Point(tb.GetPositionFromCharIndex(tb.SelectionStart).X, tb.GetPositionFromCharIndex(tb.SelectionStart).Y + 20);
                tb.Controls.Add(lb);
                //lb.Focus();
                lb.KeyDown += new KeyEventHandler(ccListBox_KeyDown);
                textBoxCcSecStart = richTextBox1.SelectionStart;
            }
            if (e.KeyValue == 55 || e.KeyValue == 220)
            {
                Control[] c = tb.Controls.Find("cc", false);
                if (c.Length > 0)
                {
                    ((ListBox)c[0]).Dispose();
                }
                ListBox lb = new ListBox();
                lb.Name = "cc";
                lb.BackColor = Color.FromArgb(31, 31, 31);
                lb.ForeColor = Color.White;
                lb.Size = new Size(448, 203);
                lb.Cursor = Cursors.Default;
                string path = $"{workSpace}assets";
                string[] files = Directory.GetFiles(path, "*.wav");
                List<string> fileNames = new List<string>();
                foreach (var file in files)
                {
                    string name = file.Split('\\')[file.Split('\\').Length - 1].Split('.')[0];
                    fileNames.Add(name);
                }
                for (int i = 0; i < fileNames.Count; i++)
                {
                    lb.Items.Add(fileNames[i]);
                }
                lb.SelectedIndex = 0;
                lb.Show();
                lb.Location = new Point(tb.GetPositionFromCharIndex(tb.SelectionStart).X, tb.GetPositionFromCharIndex(tb.SelectionStart).Y + 20);
                tb.Controls.Add(lb);
                //lb.Focus();
                lb.KeyDown += new KeyEventHandler(ccListBox_KeyDown);
                textBoxCcSecStart = richTextBox1.SelectionStart;
            }
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            CodeCompletion(sender, e);

            RichTextBox tb = (RichTextBox)sender;
            if (e.KeyCode == Keys.Enter)
            {
                Control[] c = tb.Controls.Find("cc", false);
                if (c.Length > 0)
                {
                    ((ListBox)c[0]).Dispose();
                }
            }
        }

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            RichTextBox tb = (RichTextBox)sender;
            Control[] c = tb.Controls.Find("cc", false);
            if (c.Length > 0)
            {
                ((ListBox)c[0]).Dispose();
            }
        }

        private void ccListBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Tab)
            //{
            //    Control[] c = richTextBox1.Controls.Find("cc", false);
            //    if (c.Length > 0)
            //    {
            //        ListBox lb = (ListBox)c[0];
            //        for (int i = 0; i < lb.Items.Count; i++)
            //        {
            //            if (i == lb.SelectedIndex)
            //            {
            //                richTextBox1.Select(textBoxCcSecStart, ccTimes);
            //                richTextBox1.SelectedText = lb.Items[i].ToString();
            //                richTextBox1.Select(textBoxCcSecStart + lb.Items[i].ToString().Length, 0);
            //                break;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    ccTimes++;
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Control[] c = richTextBox1.Controls.Find("cc", false);
            if (c.Length > 0)
            {
                ListBox lb = (ListBox)c[0];
                lb.Location = new Point(richTextBox1.GetPositionFromCharIndex(richTextBox1.SelectionStart).X, richTextBox1.GetPositionFromCharIndex(richTextBox1.SelectionStart).Y + 20);
            }
        }
    }
}

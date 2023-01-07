using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    public partial class BugReporter : Form
    {
        private string errorLocation;
        private string exceptionStr;

        public BugReporter(string errLocation, string ex)
        {
            errorLocation = errLocation;
            exceptionStr = ex;
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.Enabled = true;
            }
            else
            {
                textBox2.Enabled = false;
            }
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
            return result;
        }

        private void BugReporter_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string decStr = textBox1.Text.Replace("\n", "{LF}").Replace("\r", "{CR}").Replace("/", "{Slash}").Replace("\\", "{Backslash}").Replace("%", "{Percent}").Replace("?", "{Question}").Replace("&", "{And}").Replace("$", "{Dollar}").Replace("@", "{At}").Replace("#", "{Hash}");
            NetGet($"http://api.darock.top/galgm/bugreport/0.3.0%20Preview%201/{errorLocation}/{decStr}/{exceptionStr}/{textBox2.Text}");
            Environment.Exit(0);
        }
    }
}

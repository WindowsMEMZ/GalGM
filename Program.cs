using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string copyrightWarning1 = "你能看到这段文字大概率是你正在尝试反编译此程序";
            string copyrightWarning2 = "请注意！未经授权的反编译是违法行为";
            string copyrightWarning3 = "如果为研究目的，请主动联系我获取源码";
            string copyrightWarning4 = "请勿继续进行反编译！";
            Debug.WriteLine(copyrightWarning1);
            Debug.WriteLine(copyrightWarning2);
            Debug.WriteLine(copyrightWarning3);
            Debug.WriteLine(copyrightWarning4);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartWindow(args));
        }
    }
}

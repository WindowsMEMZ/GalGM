using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GalGM
{
    public partial class DebugWatch : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private string nowPath;
        private string workSpace;
        private string exeConfig;
        private int sec = 0;
        private int lastMemUdSec = 0;
        private float lastMem = 0.0f;
        private float nowMem = 0.0f;
        private int lastCPUUdSec = 0;
        private float lastCPU = 0.0f;
        private float nowCPU = 0.0f;
        private bool isHanging = false;
        private float lastGCColl = 0.0f;
        private float nowGCColl = 0.0f;

        public DebugWatch(string path, string ws, string ec)
        {
            nowPath = path;
            workSpace = ws;
            exeConfig = ec;
            InitializeComponent();
        }

        private void DebugWatch_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //检测是否正在运行
            string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
            int t = 0;
            Process[] ps = Process.GetProcesses();
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].ProcessName == projectName)
                {
                    t++;
                    break;
                }
            }
            if (t == 0)
            {
                if (INIHelper.Read("Base", "exitCode", "0", $"{workSpace}bin\\{exeConfig}\\GalGR.gds") == "0")
                {
                    INIHelper.Write("Base", "status", "false", $"{workSpace}bin\\{exeConfig}\\GalGR.gds");
                    this.Close();
                }
                else
                {
                    isHanging = true;
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (!isHanging)
            {
                nowMem = GetMemory();
                nowCPU = GetCPU();
                chart1.Series[0].Points.AddXY(sec, nowMem);
                chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.Last);
                chart2.Series[0].Points.AddXY(sec, nowCPU);
                chart2.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.Last);
                lastGCColl = nowGCColl;
                nowGCColl = GetGCColl();
                if (nowGCColl > lastGCColl)
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems[0].Text = "GC";
                    item.SubItems.Add("内存回收事件");
                    item.SubItems.Add($"{sec} 秒");
                    
                    listView1.Items.Add(item);
                }
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (!isHanging) {
                sec++;
                lastMemUdSec++;
                lastCPUUdSec++;
                label2.Text = $"诊断会话: {sec} 秒";
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            if (!isHanging)
            {
                lastMem = GetMemory();
                lastCPU = GetCPU();
                lastMemUdSec = 0;
                lastCPUUdSec = 0;
            }
        }

        private float GetMemory()
        {
            try
            {
                string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
                PerformanceCounter pf = new PerformanceCounter("Process", "Working Set - Private", projectName);
                return pf.NextValue() / 1024 / 1024;
            }
            catch (Exception error)
            {
                EditMain.debugPrint(error.ToString());
                return -1.0f;
            }
        }
        private float GetCPU()
        {
            try
            {
                string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
                PerformanceCounter pf = new PerformanceCounter("Process", "% Processor Time", projectName);
                return pf.NextValue();
            }
            catch (Exception error)
            {
                EditMain.debugPrint(error.ToString());
                return -1.0f;
            }
        }
        private float GetGCColl()
        {
            try
            {
                string projectName = INIHelper.Read("Base", "name", "ProjectName", nowPath);
                PerformanceCounter pf = new PerformanceCounter(".NET CLR Memory", "# Gen 0 Collections", projectName);
                return pf.NextValue();
            }
            catch (Exception error)
            {
                EditMain.debugPrint(error.ToString());
                return -1.0f;
            }
        }
    }
}

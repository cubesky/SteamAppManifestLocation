using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteamAppManifestLocation
{
    public partial class Form1 : Form
    {
        string path;
        bool isdone=false;
        public Form1()
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();
            dilog.Description = "请选择SteamApp目录";
            DialogResult dr = dilog.ShowDialog();
            if ( dr == DialogResult.OK || dr == DialogResult.Yes)
            {
                path = dilog.SelectedPath;
                path = path + "\\";
            }
            else
            {
                MessageBox.Show("你没有选择SteamApp目录", "选择SteamApp目录");
                System.Environment.Exit(0);
            }
            bool isStreamApp = File.Exists(path + "libraryfolders.vdf");
            if (!isStreamApp)
            {
                MessageBox.Show("SteamApp目录无效", "目录无效");
                System.Environment.Exit(0);
            }
            InitializeComponent();
            SteamAppLocate.Text = path;
            SetWindowPos(this.Handle, -1, 0, 0, 0, 0, 1 | 2);
            SetWindowPos(this.Handle, -2, 0, 0, 0, 0, 1 | 2);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            isdone = false;
            DirectoryInfo dirinfo = new DirectoryInfo(path);
            bool found = false;
            foreach (FileInfo f in dirinfo.GetFiles("*.acf"))
            {
                string[] acf=File.ReadAllLines(path + f.ToString());
                foreach (string line in acf)
                {
                    string linel=line.Trim();
                    string[] spl = linel.Replace("\t\t","\t").Split('\t');
                    if (spl.Length > 1)
                    {
                        if (spl[0].Equals("\"name\""))
                        {
                            string tr = spl[1].Replace('\"', ' ').Trim();
                            if (tr.Equals(txtGN.Text.ToString(),StringComparison.OrdinalIgnoreCase))
                            {
                                lbl_Result.Text = path + f.ToString();
                                found = true;
                                isdone = true;
                                break;
                            }
                        }
                    }
                }
                if (found) break;
            }
            if (!found) lbl_Result.Text = "没有找到对应的appmanifest文件，请确认游戏名正确。";
        }

        private void txtGN_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar != (char)13)
            {
                return;
            }
            else
            {
                btnSearch_Click(sender, null);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
        /// <summary>
        /// 得到当前活动的窗口
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern System.IntPtr GetForegroundWindow();

        private void btn_select_Click(object sender, EventArgs e)
        {
            if (isdone)
            {
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                info.Arguments = "/n,/select," + lbl_Result.Text;
                System.Diagnostics.Process.Start(info);
            }
            else
            {
                MessageBox.Show("请搜索文件后再试！","无法获取");
            }
        }
    }
}

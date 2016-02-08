using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static string fpath;
        static string binpath;
        static string dTime;

        [STAThread]
        static void Main(string[] args)
        {
            string html0 = Properties.Resources.html0+"\r\n";
            string html1 = Properties.Resources.html1;
            string html = html0;

            binpath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            folderBrowserDialog1.Description = "フォルダを選択";
            folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.ShowNewFolderButton = false;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                fpath = folderBrowserDialog1.SelectedPath;
            }
            else
            {
                return;
            }
            folderBrowserDialog1.Dispose();

            //DirectoryInfo di = new System.IO.DirectoryInfo(fpath+"\\thumbs");
            //di.Create();

            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Adobe\CSXS.5");
            regkey.SetValue("PlayerDebugMode", "1");
            regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Adobe\CSXS.6");
            regkey.SetValue("PlayerDebugMode", "1");
            regkey.Close();

            string ol = "";
            bool header = false;
            int menu_count = 0;
            IEnumerable<string> files = Directory.EnumerateFiles( fpath, "*.*", SearchOption.AllDirectories);
            foreach (string f in files)
            {
                if (header && ol != Path.GetFileName(Path.GetDirectoryName(f)))
                {
                    html += "</div>";
                    header = false;
                }
                if (!header)
                {
                    header = true;
                    html += "<div id=\"menu\" onclick=\"obj=document.getElementById('open" + menu_count + "').style; obj.display=(obj.display=='none')?'block':'none';\"><a style=\"cursor:pointer;\">" + Path.GetFileName(Path.GetDirectoryName(f)) + "</a></div><div id=\"open" + menu_count + "\" style=\"display:none;clear:both;\">";
                    ol = Path.GetFileName(Path.GetDirectoryName(f));
                    menu_count++;
                }
                DateTime targetTime = DateTime.Now;
                long unixTime = GetUnixTime(targetTime);
                dTime = GetUnixTime(targetTime).ToString();
                genThumb(f);
                html += "	" + Path.GetFileName(Path.GetDirectoryName(f)) + " - " + Path.GetFileName(f) + "<a href=\"javascript:evalScript('" + f.Replace("\\", "/") + "')\"><img src=\".\\thumbs\\" + dTime + ".gif\" onmouseover=\"this.src='thumbs/" + dTime + ".gif'\" onmouseout=\"this.src='thumbs/" + dTime + ".gif'\" width=\"320\" border=\"0\"></a><br>\r\n";
            }
            html += html1;

            StreamWriter sw = new StreamWriter("index.html", false, System.Text.Encoding.GetEncoding("UTF-8"));
            sw.Write(html);
            sw.Close();
        }

        static void genThumb(string path)
        {
            Console.WriteLine("サムネイル生成: "+ path.Replace(fpath, ""));

            string param = "-i \"" + path + "\" -an -r 15 -s 320x180 -t 2 \""+ binpath + "\\thumbs\\" + dTime + ".gif\"";
            var process = Process.Start(new ProcessStartInfo(".\\lib\\ffmpeg.exe", param)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            });
            process.WaitForExit();
            return;
        }

        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long GetUnixTime(DateTime targetTime)
        {
            targetTime = targetTime.ToUniversalTime();
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;
            return (long)elapsedTime.TotalMilliseconds;
        }
    }
}

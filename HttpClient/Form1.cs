using HttpServer;
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

namespace HttpClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ConfigManager.Refresh();
            textBox1.Text = ConfigManager.Host;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigManager.Host = textBox1.Text;
            ConfigManager.Save();


            try
            {
                Get($"http://{textBox1.Text}/");
                Log($"Get {textBox1.Text} success.");
            }
            catch (Exception ex)
            {
                Log($"Exception: {ex.Message}");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();

        }

        public static string Get(string url, string param = "")
        {
            string json = string.Empty;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format(string.IsNullOrEmpty(param) ? "{0}" : "{0}?{1}", url, param));
            Encoding encoding = Encoding.UTF8;

            req.Headers.Add("XinYunHui-DeviceId", "WXA1AA595DJ6");
            req.Headers.Add("XinYunHui-DeviceType", "Microsoft Windows NT 6.2.9200.0");
            req.Headers.Add("XinYunHui-Version", "1.0.8.0");

            req.Method = "Get";

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    json = reader.ReadToEnd();
                }

            }
            return json;
        }


        void Log(string message)
        {
            textBox2.BeginInvoke(new Action(() =>
            {
                textBox2.AppendText(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + message + Environment.NewLine);
                textBox2.ScrollToCaret();
            }));
        }
    }
}

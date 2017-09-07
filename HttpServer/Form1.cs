using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpServer
{
    public partial class Form1 : Form
    {
        HttpListener listener;

        public Form1()
        {
            InitializeComponent();

            ConfigManager.Refresh();
            textBox1.Text = ConfigManager.Host;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listener == null)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        button1.BeginInvoke(new Action(() =>
                        {
                            button1.Text = "停止";
                            textBox1.Enabled = false;
                        }));

                        ConfigManager.Host = textBox1.Text;
                        ConfigManager.Save();

                        Run();
                    }
                    catch (Exception ex)
                    {

                        button1.BeginInvoke(new Action(() =>
                        {
                            button1.Text = "启动";
                            textBox1.Enabled = true;
                        }));

                        Log(ex.Message);
                    }
                });

            }
            else
            {
                button1.Text = "启动";
                textBox1.Enabled = true;

                if (listener.IsListening)
                {
                    listener.Stop();
                }
                listener = null;
            }
        }

        void Log(string message)
        {
            textBox2.BeginInvoke(new Action(() =>
            {
                textBox2.AppendText(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + message + Environment.NewLine);
                textBox2.ScrollToCaret();
            }));
        }

        public void Run()
        {
            // 检查系统是否支持
            if (!HttpListener.IsSupported)
            {
                throw new System.InvalidOperationException(
                    "使用 HttpListener 必须为 Windows XP SP2 或 Server 2003 以上系统！");
            }

            // 注意前缀必须以 / 正斜杠结尾
            string[] prefixes = new string[] { $"http://{textBox1.Text}/" };

            // 创建监听器.
            listener = new HttpListener();
            // 增加监听的前缀.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            // 开始监听
            listener.Start();
            Console.WriteLine("监听中...");
            while (true)
            {
                // 注意: GetContext 方法将阻塞线程，直到请求到达
                HttpListenerContext context = listener.GetContext();
                // 取得请求对象
                HttpListenerRequest request = context.Request;

                var builder = new StringBuilder();

                builder.Append($"Headers:{Environment.NewLine}");
                foreach (string header in request.Headers)
                {
                    builder.AppendLine($"{header}: {request.Headers[header]}");
                }
                builder.AppendLine();
                builder.AppendLine($"RemoteEndPoint: {request.RemoteEndPoint}");
                
                Log(builder.ToString());
                //builder.AppendLine($"{}");
                //builder.AppendLine($"{}");
                //builder.AppendLine($"{}");

                // 取得回应对象
                HttpListenerResponse response = context.Response;
                // 构造回应内容
                string responseString
                    = @"<html>
                <head><title>From HttpListener Server</title></head>
                <body><h1>Hello, world.</h1></body>
            </html>";
                // 设置回应头部内容，长度，编码
                response.ContentLength64
                    = System.Text.Encoding.UTF8.GetByteCount(responseString);
                response.ContentType = "text/html; charset=UTF-8";
                // 输出回应内容
                System.IO.Stream output = response.OutputStream;
                System.IO.StreamWriter writer = new System.IO.StreamWriter(output);
                writer.Write(responseString);
                // 必须关闭输出流
                writer.Close();

                //if (Console.KeyAvailable)
                //    break;
            }
            // 关闭服务器
            //listener.Stop();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
        }
    }
}

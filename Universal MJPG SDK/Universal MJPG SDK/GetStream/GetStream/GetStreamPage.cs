using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using Bend.Util;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GetStream
{

    public partial class GetStreamPage : Form
    {

        WebClient webClient;               // Our WebClient that will be doing the downloading for us
        Stopwatch sw = new Stopwatch();    // The stopwatch which we will be using to calculate the download speed


        public GetStreamPage()
        {
            InitializeComponent();
            label8.Text = "2017/02/21";
            label9.Text = "ver 1.0.0";
            label10.Text = "Grace Yang";


            this.Initialize();

            RADIOBUTTON_IPCAM.Select();
        }

        public GetStreamPage(string[] args)
        {
            InitializeComponent();


            Ip = args[0];
            Port = args[1];
            Username = args[2];
            Password = args[3];
            int type = Int32.Parse(args[4]);
            if (type == 0)
            {
                device = DeviceType.IPCAM;
                RADIOBUTTON_IPCAM.Select();
            }
            else if (type == 1)
            {
                device = DeviceType.NAV;
                RADIOBUTTON_NAV.Select();
            }
            else if (type == 2)
            {
                device = DeviceType.NVR_DVR;
                RADIOBUTTON_NVR_DVR.Select();
            }
            else
            {
                device = DeviceType.IPCAM;
                RADIOBUTTON_IPCAM.Select();
            }

            TEXTBOX_IP.Text = Ip;
            TEXTBOX_PORT.Text = Port;
            TEXTBOX_USERNAME.Text = Username;
            TEXTBOX_PASSWORD.Text = Password;

            this.Initialize();
        }


        GetStream getstream = new GetStream();


        public string Ip { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UrlCommand { get; set; }
        public bool IsSuspend { get; set; }
        public bool IsRun { get; set; }
        public bool IsStop { get; set; }
        private static int bufferSize = 3840 * 2160;
        private MemoryStream msBuffer = new MemoryStream();
        private int NextBufferSize = 0;
        private Thread updateThread = null;
        private Thread CMXThread = null;
        private Thread ANPRThread = null;
        private Stream imageStream = null;
        private WebResponse resp = null;
        private HttpWebRequest req = null;
        private DeviceType device;
        private static byte[] boundry = Encoding.ASCII.GetBytes("--myboundary");
        private void Initialize()
        {
            this.IsSuspend = false;
            this.IsRun = false;
            this.IsStop = false;

            BUTTON_STOP.Enabled = false;
            BUTTON_SUSPEND.Enabled = false;
            PICTUREBOX_STREAM.SizeMode = PictureBoxSizeMode.Zoom;

        }



        private void START_Click(object sender, EventArgs e)
        {

            try
            {
                getstream.IpAddress = TEXTBOX_IP.Text;
                getstream.Port = TEXTBOX_PORT.Text;
                getstream.Username = TEXTBOX_USERNAME.Text;
                getstream.Password = TEXTBOX_PASSWORD.Text;
                getstream.UrlCommand = TEXTBOX_URLCOMMAND.Text;

                QuickTest.CSendCGI sendCGI = new QuickTest.CSendCGI();
                ParseCGI parseCGI = new ParseCGI();
                sendCGI.Init(TEXTBOX_IP.Text, TEXTBOX_USERNAME.Text, TEXTBOX_PASSWORD.Text);
                string cgi_port = ":" + this.TEXTBOX_PORT.Text + "/";
                //string cgi = cgi_port + TB_getmotion.Text;
                string cgi = cgi_port + "server";
                string req = null;

                if (sendCGI.Send(cgi, ref req))
                {
                    //while (true) {
                    RTB_Message.Text = req;
                    // }                    

                }
                else
                {
                    RTB_Message.Text = "Error:" + ToString();
                }


            }

            catch (Exception ex)
            {
                TEXTBOX_URLCOMMAND.Text = "Error:" + ex.ToString();
                return;
            }


            RADIOBUTTON_IPCAM.Enabled = false;
            RADIOBUTTON_NAV.Enabled = false;
            RADIOBUTTON_NVR_DVR.Enabled = false;
            COMBOBOX_OPTIONS.Enabled = false;
            TEXTBOX_IP.Enabled = false;
            TEXTBOX_PORT.Enabled = false;
            TEXTBOX_USERNAME.Enabled = false;
            TEXTBOX_PASSWORD.Enabled = false;
            BUTTON_START.Enabled = false;
            BUTTON_SUSPEND.Enabled = true;
            BUTTON_STOP.Enabled = true;
            BUTTON_SUSPEND.Text = "Suspend";

            this.IsRun = true;
            this.IsStop = false;
            this.IsSuspend = false;

            if (RADIOBUTTON_NAV.Checked)
            {
                getstream.Run_NAV();
            }
            else
            {
                getstream.Run();
            }


            updateThread = new Thread(() =>
            {
                while (!this.IsStop)
                {
                    this.Invoke(new EventHandler((obj, args) =>
                    {
                        PICTUREBOX_STREAM.Image = getstream.Image;
                    }));
                }
            });

            updateThread.IsBackground = true;
            updateThread.Start();

        }


        private void RADIOBUTTON_IPCAM_CheckedChanged(object sender, EventArgs e)
        {
            if (RADIOBUTTON_IPCAM.Checked)
            {
                device = DeviceType.IPCAM;
                TEXTBOX_URLCOMMAND.Text = "";
                this.SetUsernameAndPassword(device);
                this.SetDeviceOptions(device);
            }
        }

        private void RADIOBUTTON_NAV_CheckedChanged(object sender, EventArgs e)
        {
            if (RADIOBUTTON_NAV.Checked)
            {
                device = DeviceType.NAV;
                TEXTBOX_URLCOMMAND.Text = "";
                this.SetUsernameAndPassword(device);
                this.SetDeviceOptions(device);
            }
        }

        private void RADIOBUTTON_NVR_DVR_CheckedChanged(object sender, EventArgs e)
        {
            if (RADIOBUTTON_NVR_DVR.Checked)
            {
                device = DeviceType.NVR_DVR;
                TEXTBOX_URLCOMMAND.Text = "";
                this.SetUsernameAndPassword(device);
                this.SetDeviceOptions(device);
            }
        }

        private void SetUsernameAndPassword(DeviceType type)
        {
            switch (type)
            {
                case DeviceType.IPCAM:
                    TEXTBOX_USERNAME.Text = "admin";
                    TEXTBOX_PASSWORD.Text = "pass";
                    break;
                case DeviceType.NAV:
                    TEXTBOX_USERNAME.Text = "admin";
                    TEXTBOX_PASSWORD.Text = "";
                    break;
                case DeviceType.NVR_DVR:
                    TEXTBOX_USERNAME.Text = "admin";
                    TEXTBOX_PASSWORD.Text = "1111";
                    break;
                default:
                    break;
            }
        }


        private void SetDeviceOptions(DeviceType type)
        {
            COMBOBOX_OPTIONS.Text = "";
            COMBOBOX_OPTIONS.Items.Clear();
            switch (type)
            {
                case DeviceType.IPCAM:
                    COMBOBOX_OPTIONS.Items.Add("1080P");
                    COMBOBOX_OPTIONS.Items.Add("720P");
                    COMBOBOX_OPTIONS.Items.Add("480P");
                    COMBOBOX_OPTIONS.Items.Add("cif");
                    break;
                case DeviceType.NAV:
                    for (int iChannel = 0; iChannel <= 108; iChannel++)
                    {
                        COMBOBOX_OPTIONS.Items.Add(iChannel.ToString("000"));
                    }
                    break;
                case DeviceType.NVR_DVR:
                    for (int iChannel = 0; iChannel <= 16; iChannel++)
                    {
                        COMBOBOX_OPTIONS.Items.Add(iChannel.ToString("00"));
                    }
                    break;
                default:
                    break;
            }
            COMBOBOX_OPTIONS.SelectedIndex = 0;
        }

        private void COMBOBOX_OPTIONS_SelectedIndexChanged(object sender, EventArgs e)
        {
            TEXTBOX_URLCOMMAND.Text = "";
            switch (device)
            {
                case DeviceType.IPCAM:
                    TEXTBOX_URLCOMMAND.Text = "getimage" + COMBOBOX_OPTIONS.Text;
                    start_Listening.Enabled = false;
                    dateTimePicker1.Enabled = false;
                    dateTimePicker2.Enabled = false;
                    dateTimePicker3.Enabled = false;
                    dateTimePicker4.Enabled = false;
                    dateTimePicker5.Enabled = false;
                    radio_get5sec.Enabled = false;
                    radio_getFromTo.Enabled = false;
                    radio_framelimit.Enabled = false;
                    Channel_number.Enabled = false;
                    Frame_limit.Enabled = false;
                    Apply_download.Enabled = false;
                    ReadFile.Enabled = false;
                    tB_FilePath.Enabled = true;
                    btn_snap.Enabled = true;
                    snap_to_JPG.Enabled = true;
                    btn_getCMXalarm.Enabled = false;

                    break;
                case DeviceType.NAV:
                    TEXTBOX_URLCOMMAND.Text = "getimage" + COMBOBOX_OPTIONS.Text;
                    start_Listening.Enabled = true;
                    dateTimePicker1.Enabled = true;
                    dateTimePicker2.Enabled = true;
                    dateTimePicker3.Enabled = true;
                    dateTimePicker4.Enabled = true;
                    dateTimePicker5.Enabled = true;
                    radio_get5sec.Enabled = true;
                    radio_getFromTo.Enabled = true;
                    radio_framelimit.Enabled = true;
                    Channel_number.Enabled = true;
                    Frame_limit.Enabled = true;
                    Apply_download.Enabled = true;
                    ReadFile.Enabled = true;
                    tB_FilePath.Enabled = false;
                    btn_snap.Enabled = false;
                    snap_to_JPG.Enabled = false;
                    btn_getCMXalarm.Enabled = true;
                    break;
                case DeviceType.NVR_DVR:
                    TEXTBOX_URLCOMMAND.Text = "getimage" + COMBOBOX_OPTIONS.Text;
                    start_Listening.Enabled = false;
                    dateTimePicker1.Enabled = false;
                    dateTimePicker2.Enabled = false;
                    dateTimePicker3.Enabled = false;
                    dateTimePicker4.Enabled = false;
                    dateTimePicker5.Enabled = false;
                    radio_get5sec.Enabled = false;
                    radio_getFromTo.Enabled = false;
                    radio_framelimit.Enabled = false;
                    Channel_number.Enabled = false;
                    Frame_limit.Enabled = false;
                    Apply_download.Enabled = false;
                    ReadFile.Enabled = false;
                    tB_FilePath.Enabled = false;
                    btn_snap.Enabled = false;
                    snap_to_JPG.Enabled = false;
                    btn_getCMXalarm.Enabled = false;

                    break;
                default:
                    break;
            }
        }

        private void BUTTON_SUSPEND_Click(object sender, EventArgs e)
        {
            this.IsSuspend = !this.IsSuspend;

            if (this.IsSuspend)
            {
                BUTTON_SUSPEND.Text = "Continue";
            }
            else
            {
                BUTTON_SUSPEND.Text = "Suspend";
            }
        }


        private void BUTTON_STOP_Click(object sender, EventArgs e)
        {
            this.IsStop = true;
            getstream.Stop();
            CMXThread.Abort();

            int waitTime = 0;

            while (this.updateThread.IsAlive)
            {
                Thread.Sleep(100);
                waitTime += 100;
                if (waitTime >= 30000)
                {
                    this.updateThread.Abort();
                }
            }

            this.updateThread = null;
            BUTTON_START.Enabled = true;
            BUTTON_STOP.Enabled = false;
            BUTTON_SUSPEND.Enabled = false;
            RADIOBUTTON_IPCAM.Enabled = true;
            RADIOBUTTON_NAV.Enabled = true;
            RADIOBUTTON_NVR_DVR.Enabled = true;
            COMBOBOX_OPTIONS.Enabled = true;
            TEXTBOX_IP.Enabled = true;
            TEXTBOX_PORT.Enabled = true;
            TEXTBOX_USERNAME.Enabled = true;
            TEXTBOX_PASSWORD.Enabled = true;
        }

        private delegate void d_ObjectText(string Str, object ObjVal);
        private delegate void ANPRThreadProcess(string snap, string jpg);
        //==================
        // If can't direct display text on object, can using this function.
        //
        // Display and override info on object.
        private void ObjectText(string Str, object ObjVal)
        {
            if (this.InvokeRequired)
            {
                //Console.WriteLine("Class: {0}, FUNC: {1}, this.InvokeRequired", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                d_ObjectText LabelShow = new d_ObjectText(ObjectText);
                this.BeginInvoke(LabelShow, Str, ObjVal);
            }
            else
            {
                //Console.WriteLine("Class: {0}, FUNC: {1}, !this.InvokeRequired", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                //LabelObject.Text = Str;
                if (ObjVal is Label)
                    ((Label)ObjVal).Text = Str;
                else if (ObjVal is RichTextBox)
                    ((RichTextBox)ObjVal).Text = Str;
            }
        }

        //==================
        // If can't direct display text on object, can using this function.
        //
        // Added info on object.
        private void ObjectText_Added(string Str, object ObjVal)
        {
            if (this.InvokeRequired)
            {
                //Console.WriteLine("Class: {0}, FUNC: {1}, this.InvokeRequired", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                d_ObjectText LabelShow = new d_ObjectText(ObjectText_Added);
                this.BeginInvoke(LabelShow, Str, ObjVal);
            }
            else
            {
                //Console.WriteLine("Class: {0}, FUNC: {1}, !this.InvokeRequired", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                //LabelObject.Text = Str;
                if (ObjVal is Label)
                    ((Label)ObjVal).Text += Str;
                else if (ObjVal is RichTextBox)
                    ((RichTextBox)ObjVal).Text += Str;
            }
        }

        //==================
        // If can't direct display text on object, can using this function.
        //
        // Clear info on object.
        private void ObjectText_Clear(object ObjVal)
        {
            if (this.InvokeRequired)
            {
                //Console.WriteLine("Class: {0}, FUNC: {1}, this.InvokeRequired", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                d_ObjectText LabelShow = new d_ObjectText(ObjectText);
                //this.BeginInvoke(LabelShow, "", ObjVal);
                this.BeginInvoke(LabelShow, "", ObjVal);
            }
            else
            {
                //Console.WriteLine("Class: {0}, FUNC: {1}, !this.InvokeRequired", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                //LabelObject.Text = Str;
                if (ObjVal is Label)
                    ((Label)ObjVal).Text = "";
                else if (ObjVal is RichTextBox)
                    ((RichTextBox)ObjVal).Text = "";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Setting settingForm = new Setting(TEXTBOX_IP.Text, TEXTBOX_USERNAME.Text, TEXTBOX_PASSWORD.Text);
            settingForm.Show();

        }

        //System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        public delegate void newDelegate();
        public newDelegate myDelegate;
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                RTB_Message.AppendText(Environment.NewLine + " >> " + readData);
            //textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + readData;
        }

        private Image ReadImageFromUrl(string urlImagePath)
        {
            Uri uri = new Uri(urlImagePath);
            WebRequest webRequest = WebRequest.Create(uri);
            Stream stream = webRequest.GetResponse().GetResponseStream();
            Image res = Image.FromStream(stream);
            return res;

        }
        //==================
        // Test Process
        
        public void CMXThreadProcess(object objVal)
        {
            string Out = String.Empty;
            string url = TEXTBOX_IP.Text;
            string port = TEXTBOX_PORT.Text;
            string data = "http://" + url + ":" + port + "/getcmxalarmmotion";
            string snap = "http://" + url + ":" + port + "/cmd=getplatejpeg?file=";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(data);
            //string Out = String.Empty;
            req.Method = "GET";
            req.ContentType = "text/plain";
            //req.ContentLength = byteArray.Length;
            req.Credentials = new NetworkCredential("admin", "");
            req.UserAgent = "ActiveX(VideoCam)";
            req.Headers.Add("Authorization: Basic YWRtaW46");
            req.Connection = "KeepAlive";
            ObjectText("Get CMX alarm motion informations....", RTB_Message);
            ObjectText_Clear(this.RTB_Message);

            string s2 = "platech38=[";    // platech38=[6952B5:243002:16777215:16777215:78:-1:20170522151230_ch038_6952B5.jpg]
            //  try
            //  {
            WebResponse resp = req.GetResponse();
            //Console.WriteLine(resp);
            using (Stream stream = resp.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {

                    while (Out != null)
                    {
                        Out = sr.ReadLine();
                        //Console.WriteLine(Out);
                        ObjectText_Added(Out + "\n", this.RTB_Message);
                        bool b = Out.Contains(s2);
                        //Console.WriteLine("'{0}' is in the string '{1}': {2}", s2, Out, b);
                        if (b == true)
                        {
                            // for (int i = 0; i < strArray.Length; i++)        //透過迴圈將陣列值取出 也可用foreach
                            {
                                string[] strArray = Out.Split(':');
                                string[] str_jpg = strArray[6].Split(']');
                                //ObjectText_Added("車牌[" + b + "]\n", this.show_anpr_info);
                                ObjectText_Added("車牌[" + str_jpg[0].ToString() + "]\n", this.show_anpr_info);
                                
                                //ReadImageFromUrl(snap + str_jpg[0].ToString());
                                //System.Diagnostics.Process.Start("", snap + str_jpg[0].ToString());
                                webBrowser1.Navigate(snap + str_jpg[0].ToString());
                                /*HttpWebRequest req_snap = (HttpWebRequest)WebRequest.Create(snap + str_jpg[0].ToString());
                                HttpWebResponse sanpResponse = (HttpWebResponse)req_snap.GetResponse();

                                System.IO.Stream dataStream = sanpResponse.GetResponseStream();
                                byte[] buffer = new byte[8192];
                                string path = Environment.CurrentDirectory + @"\ANPR\snapshot";

                                //string path = Environment.CurrentDirectory;
                                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                                int size = 0;
                                do
                                {
                                    size = dataStream.Read(buffer, 0, buffer.Length);
                                    if (size > 0)
                                        fs.Write(buffer, 0, size);
                                } while (size > 0);
                                fs.Close();

                                sanpResponse.Close();     
                                 * */
                                //this.show_ANPR_snapshot.Image = req_snap;
                                //http://admin:@192.168.0.200:8021/cmd=getplatejpeg?file=20141201181706_ch004_S6133.jpg


                            }
                        }
                        //Console.ReadLine();
                    }
                    sr.Close();
                }

            }
            /*   }
               catch (ArgumentException ex)
               {
                   Out = string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message);
               }
               catch (WebException ex)
               {
                   Out = string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message);
               }
               catch (Exception ex)
               {
                   Out = string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message);
                   Console.WriteLine("Source :{0} ", ex.Source);
                   Console.WriteLine("Message :{0} ", ex.Message);
               }
               */
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ParameterizedThreadStart getCMX = new ParameterizedThreadStart(CMXThreadProcess);
            CMXThread = new Thread(getCMX);
            CMXThread.Start(this);


        }

        private void start_Listening_Click(object sender, EventArgs e)
        {
            HttpServer httpServer;
            httpServer = new MyHttpServer(8080);
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            thread.Start();
            AllocConsole();
            start_Listening.Enabled = false;

        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        private void RADIOBUTTON_time_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_get5sec.Checked)
            {
                Frame_limit.Text = "0";
                Frame_limit.Enabled = false;
            }
            else if (radio_getFromTo.Checked)
            {
                Frame_limit.Text = "0";
                Frame_limit.Enabled = false;
            }
            else if (radio_framelimit.Checked)
            {
                Frame_limit.Enabled = true;
                Frame_limit.Text = "";
            }
        }

        private void Apply_download_Click(object sender, EventArgs e)
        {
            getstream.IpAddress = TEXTBOX_IP.Text;
            getstream.Port = TEXTBOX_PORT.Text;
            getstream.Username = TEXTBOX_USERNAME.Text;
            getstream.Password = TEXTBOX_PASSWORD.Text;
            getstream.UrlCommand = TEXTBOX_URLCOMMAND.Text;

            string Date = dateTimePicker1.Value.ToString("yyyyMMdd");
            string Time5sec = dateTimePicker2.Value.ToString("HHmmss");
            int i = Convert.ToInt32(Time5sec) + 5;

            string Time_From = dateTimePicker3.Value.ToString("HHmmss");
            string Time_To = dateTimePicker4.Value.ToString("HHmmss");
            string Time_framelimit = dateTimePicker5.Value.ToString("HHmmss");
            string Channel = Channel_number.Text;
            string Framelimit = Frame_limit.Text;
            string width = "1920";
            string height = "1080";
            string speed = "2";
            string playdirection = "1";
            string iframeonly = "0";

            RTB_Message.Text = "Date=" + Date + "\n";
            RTB_Message.Text += "Time5sec=" + i.ToString("000000") + "\n";
            RTB_Message.Text += "Time_From=" + Time_From + "\n";
            RTB_Message.Text += "Time_To=" + Time_To + "\n";
            RTB_Message.Text += "Channel=" + Channel + "\n";
            RTB_Message.Text += "Framelimit=" + Framelimit + "\n";

            //download?jpeg=20141212-122259&ch=021&framelimit=0&endtime=20141212-132259&width=720&height=480&speed=2&playdirection=1&iframeonly=1

            QuickTest.CSendCGI sendCGI = new QuickTest.CSendCGI();
            ParseCGI parseCGI = new ParseCGI();
            sendCGI.Init(TEXTBOX_IP.Text, TEXTBOX_USERNAME.Text, TEXTBOX_PASSWORD.Text);
            string cgi_port = ":" + this.TEXTBOX_PORT.Text + "/";
            string cgi = "";
            if (radio_get5sec.Checked)
            {
                cgi = TEXTBOX_IP.Text + cgi_port + "download?jpeg=" + Date + "-" + Time5sec + "&ch=" + Channel + "&framelimit=" + Framelimit + "&endtime=" + Date + "-" + i.ToString("000000") + "&width=" + width + "&height=" + height + "&speed=" + speed + "&playdirection=" + playdirection + "&iframeonly=" + iframeonly;
                //radio_getFromTo.Checked = false;

            }
            else if (radio_getFromTo.Checked)
            {
                cgi = TEXTBOX_IP.Text + cgi_port + "download?jpeg=" + Date + "-" + Time_From + "&ch=" + Channel + "&framelimit=" + Framelimit + "&endtime=" + Date + "-" + Time_To + "&width=" + width + "&height=" + height + "&speed=" + speed + "&playdirection=" + playdirection + "&iframeonly=" + iframeonly;
                //radio_get5sec.Checked = false;


            }
            else if (radio_framelimit.Checked)
            {
                cgi = TEXTBOX_IP.Text + cgi_port + "download?jpeg=" + Date + "-" + Time_framelimit + "&ch=" + Channel + "&framelimit=" + Framelimit + "&width=" + width + "&height=" + height + "&speed=" + speed + "&playdirection=" + playdirection + "&iframeonly=" + iframeonly;

            }
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("http://" + cgi);
            httpRequest.Credentials = new NetworkCredential(getstream.Username, getstream.Password);
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            Console.WriteLine("httpRequest==" + cgi);

            System.IO.Stream dataStream = httpResponse.GetResponseStream();
            byte[] buffer = new byte[8192];
            string path = Environment.CurrentDirectory + @"\image\download";
            //string path = Environment.CurrentDirectory;
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            int size = 0;
            do
            {
                size = dataStream.Read(buffer, 0, buffer.Length);
                if (size > 0)
                    fs.Write(buffer, 0, size);
            } while (size > 0);
            fs.Close();

            httpResponse.Close();

            RTB_Message.Text = "下載完畢 " + DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine("Done at " + DateTime.Now.ToString("HH:mm:ss.fff"));

        }
        public static Image ConvertByteToImage(Byte[] IMAGE_SRC, out bool bolLoadOK)
        {
            // 不同的圖片格式會有不同的 offset 
            // sol 1, JPG: 174, BMP: 150, PGN: 156, GIF: 156
            // sol 2, JPG 的 HEX 是以 FF D8 FF 開頭
            //        BMP             42 4D 36
            //        GIF             47 49 46
            //        PNG             89 50 4E

            int intOffset = 0;

            MemoryStream myStream = new MemoryStream();
            List<byte> slImageByte1 = new List<byte>();
            List<byte> slImageByte2 = new List<byte>();
            List<byte> slImageByte3 = new List<byte>();

            slImageByte1.Add(0xFF); slImageByte2.Add(0xD8); slImageByte3.Add(0xFF); // JPG
            //slImageByte1.Add(0x42); slImageByte2.Add(0x4D); slImageByte3.Add(0x36); // BMP
            //slImageByte1.Add(0x47); slImageByte2.Add(0x49); slImageByte3.Add(0x46); // GIF
            //slImageByte1.Add(0x89); slImageByte2.Add(0x50); slImageByte3.Add(0x4E); // PNG
            while (IMAGE_SRC.Length != 0)
            {
                for (int j = 0; j < slImageByte1.Count && intOffset == 0; j++)
                {
                    for (int i = 0; i < IMAGE_SRC.Length - 2; i++)
                    {
                        if (IMAGE_SRC[i] == slImageByte1[j] && IMAGE_SRC[i + 1] == slImageByte2[j] && IMAGE_SRC[i + 2] == slImageByte3[j])
                        {
                            intOffset = i;
                            Console.WriteLine("intOffset=" + intOffset);
                            //break;
                        }
                    }
                }
            }

            myStream.Write(IMAGE_SRC, intOffset, IMAGE_SRC.Length - intOffset);

            Image imgRet = null;

            try
            {
                imgRet = Image.FromStream(myStream);
                //imgRet.Save("D:\\111.jpeg");
                imgRet.Save("D:\\" + intOffset + ".jpeg");
                bolLoadOK = true;
            }
            catch (Exception ex)
            {
                bolLoadOK = false;
            }


            myStream.Close();

            return imgRet;
        }

        //GraceYang

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static int StringtoInteger(string str)
        {
            int i = 0;
            int sign = 0;
            int val = 0;

            while (i < str.Length && ((str[i] >= '0' && str[i] <= '9') || str[i] == ' ' || str[i] == '-' || str[i] == '+'))
            {
                if ((val == 0 && sign == 0) && str[i] == ' ')
                    i++;
                else if (str[i] == '-' && sign == 0)
                {
                    sign = -1;
                    i++;
                }
                else if (str[i] == '+' && sign == 0)
                {
                    sign = 1;
                    i++;
                }
                else if (str[i] >= '0' && str[i] <= '9')
                {
                    //handle overflow, val * 10 + n > int.MaxValue
                    if (val > (int.MaxValue - (str[i] - '0')) / 10)
                    {
                        if (sign == 0 || sign == 1)
                            return int.MaxValue;
                        return int.MinValue;
                    }
                    val = val * 10 + str[i] - '0';
                    i++;
                }
                else
                {
                    if (sign == 0)
                        return val;
                    return val * sign;
                }
            }
            if (sign == 0)
                return val;
            return val * sign;
        }

        public static void ConvertByteToJPG(Byte[] IMAGE_SRC)
        {
            MemoryStream myStream = new MemoryStream();
            MemoryStream myStream1 = new MemoryStream();
            MemoryStream myStream2 = new MemoryStream();
            List<byte> S1 = new List<byte>();
            List<byte> S2 = new List<byte>();
            List<byte> S3 = new List<byte>();
            List<byte> S4 = new List<byte>();
            List<byte> S5 = new List<byte>();

            S1.Add(0xFF); S1.Add(0xD8); //JPG Start
            S2.Add(0xFF); S2.Add(0xD9);// JPG End
            S3.Add(0x74); S3.Add(0x69); S3.Add(0x6D); S3.Add(0x65); S3.Add(0x3D);// time
            S4.Add(0x20); S4.Add(0x66); S4.Add(0x72); S4.Add(0x61); S4.Add(0x6D);// frame
            S5.Add(0x74); S5.Add(0x69); S5.Add(0x63); S5.Add(0x6B); S5.Add(0x3D);// time tick (ms)

            int jpgStart = 0;
            int jpgEnd = 0;
            int time_line = 0;
            int time_end = 0;
            int time_tick = 0;
            int time_tick_end = 0;
            int frame = 0;
            string path = Environment.CurrentDirectory + @"\image\";
            DateTime getTime;

            for (int i = 0; i < IMAGE_SRC.Length - 2; i++)
            {


                if (IMAGE_SRC[i] == S1[0] && IMAGE_SRC[i + 1] == S1[1])
                {
                    jpgStart = i;
                    //Console.WriteLine("jpgStart=" + jpgStart);
                }
                if (IMAGE_SRC[i] == S2[0] && IMAGE_SRC[i + 1] == S2[1])
                {
                    jpgEnd = i;
                    //Console.WriteLine("jpgEnd=" + jpgEnd);

                }

                if (IMAGE_SRC[i] == S3[0] && IMAGE_SRC[i + 1] == S3[1] && IMAGE_SRC[i + 2] == S3[2] && IMAGE_SRC[i + 3] == S3[3] && IMAGE_SRC[i + 4] == S3[4])
                {
                    time_line = i;
                    time_end = i + 15;

                    //Console.WriteLine("time_line=" + time_line);

                }

                if (IMAGE_SRC[i] == S5[0] && IMAGE_SRC[i + 1] == S5[1] && IMAGE_SRC[i + 2] == S5[2] && IMAGE_SRC[i + 3] == S5[3] && IMAGE_SRC[i + 4] == S5[4])
                {
                    time_tick = i;
                    time_tick_end = i + 9;

                }

                if (IMAGE_SRC[i] == S4[0] && IMAGE_SRC[i + 1] == S4[1] && IMAGE_SRC[i + 2] == S4[2] && IMAGE_SRC[i + 3] == S4[3] && IMAGE_SRC[i + 4] == S4[4])
                {
                    frame = i;

                }

                if ((time_tick > time_line) && (jpgEnd > jpgStart))
                {
                    //Get time
                    myStream1.Write(IMAGE_SRC, time_line, time_end - time_line); // get timeline
                    var result = BitConverter.ToString(myStream1.ToArray());
                    string new_result = result.Remove(0, 15); // remove 
                    string new_result1 = new_result.Remove(0, 1); // remove first 3
                    string AA = new_result1.Replace("-3", string.Empty);

                    double timestampv = Convert.ToDouble(AA);
                    // change unixTime to human datetime
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    getTime = epoch.AddSeconds(timestampv);
                    string GMT_Time = getTime.ToString();  //2016/08/30 08:30:00  GMT
                    DateTime LocalTime = TimeZoneInfo.ConvertTimeFromUtc(getTime, TimeZoneInfo.Local); // Local Time

                    //filename format YYYYMMDD_HHMMSS
                    string time_0 = GMT_Time.Replace("/", string.Empty);
                    string time_1 = time_0.Replace(":", string.Empty);
                    string time_2 = time_1.Replace(" ", "_");
                    myStream1.Position = 0;

                    string aa = "";
                    //Get time tick
                    myStream2.Write(IMAGE_SRC, time_tick, time_tick_end - time_tick); // get timeline
                    var time_tick_v = BitConverter.ToString(myStream2.ToArray());
                    string new_time_tick = time_tick_v.Remove(0, 14); // remove

                    //string new_time_tick_0 = new_time_tick.Remove(0, 1); // remove first 3                   
                    string time_tick_num = new_time_tick.Replace("-3", string.Empty);

                    //Console.WriteLine(time_tick_num);

                    if (time_tick_num.Length == 4)
                    {
                        aa = (time_tick_num.Substring(time_tick_num.Length - 3));

                    }
                    else if ((time_tick_num.Substring(time_tick_num.Length - 6) == "-20-66"))
                    {
                        aa = time_tick_num.Substring(0, 2);
                    }
                    else if ((time_tick_num.Substring(time_tick_num.Length - 6) == "-0D-0A"))
                    {
                        aa = time_tick_num.Substring(0, 2);

                    }
                    else
                    {
                        aa = time_tick_num.Substring(0, 3);
                    }

                    myStream2.Position = 0;


                    // Get JPG (FFD8~FFD9)
                    myStream.Write(IMAGE_SRC, jpgStart, jpgEnd - jpgStart);
                    Image imgRet = null;
                    imgRet = Image.FromStream(myStream);
                    // imgRet.Save(path + jpgStart + ".jpg");
                    //Display watermark time
                    // string display_GMT_time = "GMT "+GMT_Time;
                    //string display_Local_time = "Local Time " + LocalTime;

                    //watermark start
                    Bitmap bmp = new Bitmap(myStream);
                    Graphics canvas = Graphics.FromImage(bmp);
                    try
                    {
                        Bitmap bmpNew = new Bitmap(bmp.Width, bmp.Height);
                        canvas = Graphics.FromImage(bmpNew);
                        canvas.DrawImage(bmp, new Rectangle(0, 0,
                        bmpNew.Width, bmpNew.Height), 0, 0, bmp.Width, bmp.Height,
                        GraphicsUnit.Pixel);
                        bmp = bmpNew;
                    }
                    catch (Exception ee) // Catch exceptions 
                    {
                        Console.WriteLine(ee.Message);
                    }
                    // Here replace "Text" with your text and you also can assign Font Family, Color, Position Of Text etc. 
                    Color myColor;
                    myColor = Color.FromArgb(255, 255, 255);

                    // Create string to draw.
                    //String display_GMT_time = "GMT " + GMT_Time + "." + time_tick_num_1;
                    String display_GMT_time = GMT_Time + "." + aa.PadLeft(3, '0');
                    String display_Local_time = "Local Time " + LocalTime;

                    // Create font and brush.
                    Font drawFont = new Font("Vera", 24, FontStyle.Bold);
                    SolidBrush drawBrush = new SolidBrush(myColor);

                    // Create rectangle for drawing.
                    float x = 30.0F;
                    float y = 1020.0F;
                    float width = 500.0F;
                    float height = 300.0F;
                    RectangleF drawRect = new RectangleF(x, y, width, height);

                    // canvas.DrawString(display_GMT_time, new Font("Vera", 24,
                    //FontStyle.Bold), new SolidBrush(myColor), (bmp.Width / 2), (bmp.Height / 2));

                    canvas.DrawString(display_GMT_time, drawFont, drawBrush, drawRect);
                    // Save or display the image where you want. 
                    //bmp.Save(path + time_2 + new_time_tick_1 + "_new.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    bmp.Save(path + time_2 + "_" + aa.PadLeft(3, '0') + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    //watermark end

                    myStream.Position = 0;

                }
            }


            myStream.Close();
            myStream1.Close();

            Console.WriteLine("轉換完畢 " + DateTime.Now.ToString("HH:mm:ss"));

        }
        public static void ConvertSnapToJPG(Byte[] IMAGE_SRC)
        {
            MemoryStream myStream = new MemoryStream();
            List<byte> S1 = new List<byte>();
            List<byte> S2 = new List<byte>();

            S1.Add(0xFF); S1.Add(0xD8); //JPG Start
            S2.Add(0xFF); S2.Add(0xD9);// JPG End

            int jpgStart = 0;
            int jpgEnd = 0;
            string pathfile = Environment.CurrentDirectory + @"\snap\path.txt";
            string pathfile2 = Environment.CurrentDirectory + @"\snap\";
            string path;
            StreamReader sr = new StreamReader(pathfile);
            path = sr.ReadLine();
            sr.Close();
            //string path = Environment.CurrentDirectory + @"\image\";
            //string path = ;
            var localTime = DateTime.Now.ToString("HH:mm:ss tt");

            for (int i = 0; i < IMAGE_SRC.Length; i++)
            {
                if (IMAGE_SRC[i] == S1[0] && IMAGE_SRC[i + 1] == S1[1])
                {
                    jpgStart = i;
                    Console.WriteLine("jpgStart=" + jpgStart);
                }
                if (IMAGE_SRC[i] == S2[0] && IMAGE_SRC[i + 1] == S2[1])
                {
                    jpgEnd = i;
                    Console.WriteLine("jpgEnd=" + jpgEnd);

                }

                if (jpgEnd > jpgStart)
                {

                    // Get JPG (FFD8~FFD9)
                    myStream.Write(IMAGE_SRC, jpgStart, jpgEnd - jpgStart);
                    Image imgRet = null;
                    imgRet = Image.FromStream(myStream);
                    // imgRet.Save(path + jpgStart + ".jpg");
                    //Display watermark time
                    //string display_GMT_time = "GMT "+GMT_Time;
                    //string display_Local_time = "Local Time " + LocalTime;

                    //watermark start
                    Bitmap bmp = new Bitmap(myStream);
                    Graphics canvas = Graphics.FromImage(bmp);
                    try
                    {
                        Bitmap bmpNew = new Bitmap(bmp.Width, bmp.Height);
                        canvas = Graphics.FromImage(bmpNew);
                        canvas.DrawImage(bmp, new Rectangle(0, 0,
                        bmpNew.Width, bmpNew.Height), 0, 0, bmp.Width, bmp.Height,
                        GraphicsUnit.Pixel);
                        bmp = bmpNew;
                    }
                    catch (Exception ee) // Catch exceptions 
                    {
                        Console.WriteLine(ee.Message);
                    }

                    // Create string to draw.
                    //String display_GMT_time = "GMT " + GMT_Time + "." + time_tick_num_1;
                    // String display_GMT_time = GMT_Time + "." + aa.PadLeft(3, '0');
                    //String display_Local_time = "Local Time " + LocalTime;

                    // Save or display the image where you want. 
                    //filename format YYYYMMDD_HHMMSS
                    string time_0 = localTime.Replace(":", string.Empty);
                    string time_1 = time_0.Replace(" ", "_");
                    //bmp.Save(path + time_2 + new_time_tick_1 + "_new.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    bmp.Save(path + "snap_" + time_1 + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    bmp.Save(pathfile2 + "snap_" + time_1 + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    //watermark end

                    myStream.Position = 0;

                }
            }


            myStream.Close();

            Console.WriteLine("轉換完畢 " + DateTime.Now.ToString("HH:mm:ss"));

        }
        public static void DisplayArray(Array arr, string name)
        {
            // Get the array element width; format the formatting string.
            int elemWidth = Buffer.ByteLength(arr) / arr.Length;
            string format = String.Format(" {{0:X{0}}}", 2 * elemWidth);
            // Display the array elements from right to left.
            Console.Write("{0,7}:", name);
            for (int loopX = arr.Length - 1; loopX >= 0; loopX--)
                Console.Write(format, arr.GetValue(loopX));
            Console.WriteLine();
        }


        private void ReadFile_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\image\download";
            //Convert to JPG
            byte[] _bytes = File.ReadAllBytes(path);
            ConvertByteToJPG(_bytes);
            RTB_Message.Text = "圖片轉換完畢 " + DateTime.Now.ToString("HH:mm:ss");

            /*
            StreamReader sr = System.IO.File.OpenText(path);
            StreamReader sr2 = new StreamReader(path, System.Text.Encoding.Default);
            Console.WriteLine(sr2.ReadLine().ToString());
            string s = sr.ReadToEnd();
            sr.Close();
            string[] temp = s.Split('\n');
            string AA;
            DateTime getTime;

            for (int i = 0; i < temp.Length; i++)
            {
                //Console.WriteLine(temp.Length);


                if (temp[i].IndexOf("time=") != -1)
                {
                    //Console.WriteLine(string.Format("Found in: {0}\n{1}\nLine: {2} \n", "D:\\download", temp[i].Trim(), i + 1));
                    //Console.WriteLine(temp[i].Trim(),i);
                    AA = temp[i].Trim();
                    string[] BB = AA.Split('=');
                    string[] CC = BB[2].Split(' ');
                    double timestampv = Convert.ToDouble(CC[0]);
                    StreamWriter sw0 = new StreamWriter(path + "_timestamp.txt", true);
                    sw0.WriteLine(timestampv);
                    sw0.Close();
                    // change unixTime to human datetime
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    getTime = epoch.AddSeconds(timestampv);
                    string value = getTime.ToString();
                    //Console.WriteLine(getTime);
                    //System.IO.File.WriteAllText(path + "timelist.txt", value);
                    StreamWriter sw = new StreamWriter(path + "_timelist.txt", true);
                    sw.WriteLine(value);
                    sw.Close();

                }
            }
             */

        }

        public static byte[] ReadFile_(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading

                //Console.WriteLine(sum);
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        public static byte[] ReadFileBytes(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                ///while ((count = fileStream.ReadByte(buffer, sum, length - sum)) > 0)
                //    sum += count;  // sum is a buffer offset for next reading
                int i = 0;
                while ((i = fileStream.ReadByte()) != -1)
                {
                    buffer[i] = (byte)fileStream.ReadByte();
                }

                //Console.WriteLine(sum);
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        public static byte[] ReadFileBinary(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            BinaryReader reader = new BinaryReader(fileStream);
            //int sum = 0, length = 0;
            //int count = fileStream.Read(buffer, sum, length - sum);
            //int intRead = reader.ReadInt32();
            //string stringRead = reader.ReadString();
            //byte[] byteRead = reader.ReadBytes(20);
            //char charRead = reader.ReadChar();
            
            int dl = System.Convert.ToInt32(fileStream.Length);
            byte[] myData = reader.ReadBytes(dl);

            //for (int i = 0; i < 20; i++)
            //{
            //    byte number = (byte)fileStream.ReadByte();
            //    Console.WriteLine(number);
            //}
            buffer = new byte[10];
            /*
                try
                {
                    int length = (int)fileStream.Length;  // get file length
                    buffer = new byte[length];            // create buffer
                    int count;                            // actual number of bytes read
                    int sum = 0;                          // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    ///while ((count = fileStream.ReadByte(buffer, sum, length - sum)) > 0)
                    //    sum += count;  // sum is a buffer offset for next reading
                    int i = 0;
                    while ((i = fileStream.ReadByte()) != -1)
                    {
                        buffer[i] = (byte)fileStream.ReadByte();
                    }

                    //Console.WriteLine(sum);
                }
                finally
                {
                    fileStream.Close();
                }
            */
            return buffer;
        }

        public Image byteArrayToImage(byte[] bytesArr)
        {
            MemoryStream memstr = new MemoryStream(bytesArr);
            Image img = Image.FromStream(memstr);

            // img.Save("@D:/output.jpg", ImageFormat.Jpeg);  // Or Png
            return img;
        }
        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
        private void btn_snap_Click(object sender, EventArgs e)
        {
            try
            {
                getstream.IpAddress = TEXTBOX_IP.Text;
                getstream.Port = TEXTBOX_PORT.Text;
                getstream.Username = TEXTBOX_USERNAME.Text;
                getstream.Password = TEXTBOX_PASSWORD.Text;
                getstream.UrlCommand = TEXTBOX_URLCOMMAND.Text;
                string filePath_temp = tB_FilePath.Text;

                QuickTest.CSendCGI sendCGI = new QuickTest.CSendCGI();
                ParseCGI parseCGI = new ParseCGI();
                sendCGI.Init(TEXTBOX_IP.Text, TEXTBOX_USERNAME.Text, TEXTBOX_PASSWORD.Text);
                string cgi_port = ":" + this.TEXTBOX_PORT.Text + "/";
                string cgi = cgi_port + "snap";
                string req = null;

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("http://" + getstream.IpAddress + cgi);
                httpRequest.Credentials = new NetworkCredential(getstream.Username, getstream.Password);
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                System.IO.Stream dataStream = httpResponse.GetResponseStream();
                byte[] buffer = new byte[8192];
                string path = Environment.CurrentDirectory + @"\snap\snap";

                //string path = Environment.CurrentDirectory;
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                int size = 0;
                do
                {
                    size = dataStream.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                        fs.Write(buffer, 0, size);
                } while (size > 0);
                fs.Close();

                httpResponse.Close();

                RTB_Message.Text = "擷取圖片完畢 " + DateTime.Now.ToString("HH:mm:ss");
                using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\snap\" + "path.txt"))   //小寫TXT     
                {
                    sw.Write(filePath_temp);
                    sw.Close();
                }
                //Console.WriteLine("Done at " + DateTime.Now.ToString("HH:mm:ss.fff"));
                /*
                if (sendCGI.Send(cgi, ref req))
                {
                    var str = req;

                    byte[] myByteArray = new byte[str.Length];
                    string hex = ConvertStringToHex(str);

                    string path = Environment.CurrentDirectory + @"\image\snap";
                    using (StreamWriter sw = new StreamWriter(path))     
                    {
                        // Add some text to the file.
                        sw.Write(hex);
                        RTB_Message.Text = "Snap OK:" + ToString();
                    }
                }
                else
                {
                    RTB_Message.Text = "Error:" + ToString();
                }
               */


            }
            catch
            {

            }
        }

        private void snap_to_JPG_Click(object sender, EventArgs e)
        {
            /*string path = Environment.CurrentDirectory + @"\image\snap";
            //Convert to JPG
            byte[] _bytes = File.ReadAllBytes(path);
            ConvertSnapToJPG(_bytes);
            RTB_Message.Text = "圖片轉換完畢 " + DateTime.Now.ToString("HH:mm:ss");
             */
            string path = Environment.CurrentDirectory + @"\snap\snap";
            //Convert to JPG
            byte[] _bytes = File.ReadAllBytes(path);
            ConvertSnapToJPG(_bytes);
            RTB_Message.Text = "圖片轉換完畢 " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void SendAudio_Click(object sender, EventArgs e)
        {  
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();

            string datapath = "";

            if (file.SafeFileName != "")
            {
                datapath = file.FileName;
            }
            else
            {
                datapath = "";
            }
            
            byte[] audiobuff = ReadFile_(datapath);
                     
            string ipaddr = TEXTBOX_IP.Text;
            string port = TEXTBOX_PORT.Text;
            string url = "http://" + ipaddr + ":" + port + "/sendaudio";


            //string url = "http://192.168.10.32/sendaudio";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "admin", "pass")));

            req.Credentials = new NetworkCredential("admin", "pass");

            req.Timeout = 1000; //Timeout have be setting, or can't send audio again.
                        
            //req.Headers.Add("Accept-Language", "zh-tw");
            //req.UserAgent = "ActiveX(VideoCam)";
            //req.Headers.Add("Accept-Encoding", "gzip, deflate");
            //req.Host = "192.168.3.250";
            //req.Headers.Add("Authorization: Basic YWRtaW46");
            //req.UserAgent = "RTSPAUDIO/4.0";

            req.KeepAlive = true;
            req.Connection = "KeepAlive";
            //req.Headers.Add("Cookie", "LANG=0");
            req.Headers.Add("Authorization", "Basic " + code);
            req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = audiobuff.Length;

            try
            {
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(audiobuff, 0, audiobuff.Length);
                requestStream.Close();                
            }
            catch
            {

            }

            /*
            Stream requestStream;
            
            //WebResponse response;
            //var response;
            

            byte[] sendbuff = new byte[1500];

            requestStream = req.GetRequestStream();
            requestStream.Write(audiobuff, 0, audiobuff.Length);
            requestStream.Close();

            var response = req.GetResponse();
            //reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseStr = "";
            
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8, false))
            {
                responseStr = reader.ReadToEnd();
                reader.Close();
            }
            response.Close();
            */
        }        
    }
}


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

namespace GetStream
{

    public enum DeviceType
    {
        IPCAM,
        NAV,
        NVR_DVR,
        NONE
    };

    public class GetStream
    {

        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UrlCommand { get; set; }
        public bool IsSuspend { get; set; }
        public bool IsRun { get; set; }
        public bool IsStop { get; set; }


        public Bitmap Image { get; set; }
        private static int bufferSize = 3840 * 2160;
        private MemoryStream msBuffer = new MemoryStream();
        private int NextBufferSize = 0;
        public Thread updateThread { get; set; }
        private Stream imageStream = null;
        private WebResponse resp = null;
        private HttpWebRequest req = null;
        private DeviceType device;
        private static byte[] boundry = Encoding.ASCII.GetBytes("--myboundary");

        public GetStream()
        {
            this.initialize(DeviceType.NONE);
        }

        public GetStream(string IpAddress, string Port, string Username, string Password, DeviceType type)
        {
            this.device = type;
            this.initialize(type);
            this.IpAddress = IpAddress;
            this.Port = Port;
            this.Username = Username;
            this.Password = Password;
        }

        private void initialize(DeviceType type)
        {
            IpAddress = "192.168.0.200";
            Port = "80";

            switch (type)
            {
                case DeviceType.IPCAM:
                    Username = "admin";
                    Password = "pass";
                    break;
                case DeviceType.NAV:
                    Username = "admin";
                    Password = "";
                    break;
                case DeviceType.NVR_DVR:
                    Username = "admin";
                    Password = "1111";
                    break;
                default:
                    Username = "admin";
                    Password = "pass";
                    break;
            }
        }
        public void Stop()
        {
            this.IsStop = true;

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
        }

        private string ReadAsciiLine(MemoryStream ms)
        {
            StringBuilder result = new StringBuilder();
            long orginalPos = ms.Position;

            while (true)
            {
                int r = ms.ReadByte();
                if (r == -1)
                {
                    ms.Position = orginalPos;
                    return null;
                }
                if (r == 13)
                {
                    int n = ms.ReadByte();
                    if (n != 10)
                    {
                        ms.Position = ms.Position - 1;
                    }
                    return result.ToString();
                }
                result.Append((char)r);
            }
        }


        public byte[] GetNextBitmap()
        {
            int readed = 0;
            byte[] buf = new byte[bufferSize];

            while ((readed = imageStream.Read(buf, 0, buf.Length)) != 0)
            {
                msBuffer.Write(buf, 0, readed);

                if (NextBufferSize == 0)
                {
                    if (msBuffer.Length < 100)
                    {
                        continue;
                    }
                    byte[] scanbuf = msBuffer.GetBuffer();

                    int i = 0;
                    int j = 0;

                    while (i < scanbuf.Length)
                    {
                        if (scanbuf[i] == boundry[j])
                        {
                            j++;
                            if (j == boundry.Length)
                            {
                                break;
                            }
                        }
                        else
                        {
                            j = 0;
                        }
                        i++;
                    }

                    if (j == boundry.Length)
                    {
                        msBuffer.Position = i;
                        string boundrystr = ReadAsciiLine(msBuffer);
                        string contenttype = ReadAsciiLine(msBuffer);
                        string contentlength = ReadAsciiLine(msBuffer);
                        string emptyline = ReadAsciiLine(msBuffer);

                        if (string.IsNullOrEmpty(contentlength))
                        {
                            continue;
                        }

                        //Grace Add: parse contentLength 
                        string[] content = contentlength.Split(':');
                        string[] content_1 = content[1].Split(' ');
                        string content_v = content_1[1].Trim(" ".ToCharArray());
                        NextBufferSize = Int32.Parse(content_v);
                        //Grace End

                        //NextBufferSize = Int32.Parse(contentlength.Substring(15, 7));  // old parse error for IPCAM/NAV

                        if ((msBuffer.Length - msBuffer.Position) > NextBufferSize)
                        {
                            return ReadImageBytes((int)msBuffer.Position);
                        }
                        else
                        {
                            msBuffer = CopyStream(msBuffer);
                            continue;
                        }
                    } // end of (j == boundry.Length)
                    else
                    {
                        // give up all content.
                        msBuffer = new MemoryStream();
                        continue;
                    }
                } // end of if (NextBufferSize == 0)
                else if (NextBufferSize > msBuffer.Length)
                {
                    continue;
                }
                return ReadImageBytes(0);
            } // end of while ((readed = imageStream.Read(buf, 0, buf.Length)) != 0)

            return null;
        }


        private byte[] ReadImageBytes(int beginpos)
        {
            byte[] r = new byte[NextBufferSize];
            msBuffer.Seek(beginpos, SeekOrigin.Begin);
            msBuffer.Read(r, 0, r.Length);
            msBuffer = CopyStream(msBuffer);
            NextBufferSize = 0;
            return r;
        }

        private MemoryStream CopyStream(MemoryStream ms)
        {
            int size = (int)(ms.Length - ms.Position);
            MemoryStream newStream = new MemoryStream(size);
            byte[] buf = new byte[bufferSize];
            while (true)
            {
                int readed = ms.Read(buf, 0, buf.Length);
                if (readed == 0)
                {
                    break;
                }
                newStream.Write(buf, 0, readed);
            }
            return newStream;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public void Run()
        {

            this.IsRun = true;
            this.IsStop = false;
            this.IsSuspend = false;

            updateThread = new Thread(() =>
            {
                try
                {
                    req = (HttpWebRequest)WebRequest.Create("http://" + IpAddress + ":" + Port + "/" + UrlCommand);
                    req.Credentials = new NetworkCredential(Username, Password);
                    resp = req.GetResponse();
                    imageStream = resp.GetResponseStream();

                    byte[] imagedata;

                    while (((imagedata = GetNextBitmap()) != null) && (!this.IsStop))
                    {

                        if (!this.IsSuspend)
                        {
                            MemoryStream imagems = new MemoryStream(imagedata);
                            this.Image = new Bitmap(imagems);
                        }
                        Cursor.Current = Cursors.Default;

                    } // end of if (!this.IsSuspend)    
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Failed: please stop and retry again. " + ex.Message.ToString());
                }

            this.IsRun = false;
        }); // end of new Thread()

            updateThread.IsBackground = true;
            updateThread.Start();
        }


        public void Run_NAV()
        {

            this.IsRun = true;
            this.IsStop = false;
            this.IsSuspend = false;

            updateThread = new Thread(() =>
            {

                try
                {
                    req = (HttpWebRequest)WebRequest.Create("http://" + IpAddress + ":" + Port + "/" + UrlCommand);
                    req.Credentials = new NetworkCredential(Username, Password);
                    resp = req.GetResponse();
                    imageStream = resp.GetResponseStream();

                    byte[] imagedata;

                    while (((imagedata = GetNextBitmap()) != null) && (!this.IsStop))
                    {
                        if (!this.IsSuspend)
                        {
                            string image_hexString = ByteArrayToString(imagedata);
                            string newImage = image_hexString.Remove(0, 4); // remove '0d0a'
                            byte[] newimagedata = StringToByteArray(newImage);
                            MemoryStream imagems = new MemoryStream(newimagedata);
                            this.Image = new Bitmap(imagems);
                         
                            }
                            Cursor.Current = Cursors.Default;

                        } // end of if (!this.IsSuspend)   
                } // end of try
                catch (Exception ex)
                {
                    //MessageBox.Show("Failed: please stop and retry again. " + ex.Message.ToString());
                }


                this.IsRun = false;
            }); // end of new Thread()

            updateThread.IsBackground = true;
            updateThread.Start();
        }

        public void Run_SendAudio()
        {

        }
    }
}

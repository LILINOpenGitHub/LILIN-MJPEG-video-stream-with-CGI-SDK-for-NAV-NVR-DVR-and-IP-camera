using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GetStream
{
    public partial class Setting : Form
    {
        public QuickTest.CSendCGI sendCGI = new QuickTest.CSendCGI();
        public ParseCGI parseCGI = new ParseCGI();
        public Setting()
        {
            InitializeComponent();
            this.Initialize();

        }

        public Setting(string ip, string username, string password)
        {
            InitializeComponent();
            this.Initialize();
            sendCGI.Init(ip, username, password);
            string req = null;
            sendCGI.Send("camctrl?cmd=get&DayNight=0", ref req);
            parseCGI.KeyValueSeparator = "=";
            parseCGI.CommandSeparator = "\n";
            parseCGI.Parse(req);

            foreach (var item in parseCGI.Commands2)
            {
                switch (item.Key)
                {
                    case "bright":
                        this.textBox_Basic_Day_Brightness.Text = item.Value.ToString();
                        this.trackBar_Basic_Day_Brightness.Value = Int32.Parse(item.Value);
                        break;
                    case "contrast":
                        this.textBox_Basic_Day_Contrast.Text = item.Value.ToString();
                        this.trackBar_Basic_Day_Contrast.Value = Int32.Parse(item.Value);
                        break;
                    case "hue":
                        this.textBox_Basic_Day_Hue.Text = item.Value.ToString();
                        this.trackBar_Basic_Day_Hue.Value = Int32.Parse(item.Value);
                        break;
                    case "Saturation":
                        this.textBox_Basic_Day_Saturation.Text = item.Value.ToString();
                        this.trackBar_Basic_Day_Saturation.Value = Int32.Parse(item.Value);
                        break;
                    case "Sharpness":
                        //this.textBox_Basic_Day_Sharpness.Text = item.Value.ToString();
                        //this.trackBar_Basic_Day_Sharpness.Value = Int32.Parse(item.Value);
                        break;
                    case "camstat":
                        this.textBox_Basic_Day_Camstat.Text = item.Value.ToString();
                        this.trackBar_Basic_Day_Camstat.Value = Int32.Parse(item.Value);
                        break;
                    default:
                        break;
                }
                
            }
            this.GetAdvanceCGIContent(false);
            return;
        }

        private void Initialize()
        {
            this.tabControl1.TabPages[0].Text = @"Basic";
            this.tabControl1.TabPages[1].Text = @"Advance";
            this.panel_Basic_Night.Visible = false;
            this.panel_Advance_Night.Visible = false;

            this.comboBox_Advance_Day_WhiteBalanceControl.Items.Add("Auto");
            this.comboBox_Advance_Day_WhiteBalanceControl.Items.Add("Tungsten");
            this.comboBox_Advance_Day_WhiteBalanceControl.Items.Add("Fixed Indoor");
            this.comboBox_Advance_Day_WhiteBalanceControl.Items.Add("Fixed Fluorescents 1");
            this.comboBox_Advance_Day_WhiteBalanceControl.Items.Add("Fixed Fluorescents 2");
            this.comboBox_Advance_Day_WhiteBalanceControl.Items.Add("Fixed Outdoor 1");
            this.comboBox_Advance_Day_WhiteBalanceControl.Items.Add("Fixed Outdoor 2");

            this.comboBox_Advance_Day_Mirror.Items.Add("Off");
            this.comboBox_Advance_Day_Mirror.Items.Add("On");

            this.comboBox_Advance_Day_Flip.Items.Add("Off");
            this.comboBox_Advance_Day_Flip.Items.Add("On");

            this.comboBox_Advance_Day_ExposureValue.Items.Add("1");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("2");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("3");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("4");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("5");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("6");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("7");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("8");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("9");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("10");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("11");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("12");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("13");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("14");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("15");
            this.comboBox_Advance_Day_ExposureValue.Items.Add("16");

            this.comboBox_Advance_Day_WDR.Items.Add("Off");
            this.comboBox_Advance_Day_WDR.Items.Add("Auto");
            this.comboBox_Advance_Day_WDR.Items.Add("x2");
            this.comboBox_Advance_Day_WDR.Items.Add("x3");
            this.comboBox_Advance_Day_WDR.Items.Add("x4");

            this.comboBox_Advance_Day_BackLightCompensation.Items.Add("Off");
            this.comboBox_Advance_Day_BackLightCompensation.Items.Add("On");

            this.comboBox_Advance_Day_DCIrisMode.Items.Add("Off");
            this.comboBox_Advance_Day_DCIrisMode.Items.Add("On");

            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/8000");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/6000");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/4000");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/3000");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/2000");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/1600");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/1000");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/800");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/640");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/480");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/400");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/320");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/240");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/200");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/160");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/120");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/100");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/50");
            this.comboBox_Advance_Day_ShutterLimitMin.Items.Add("1/25");

            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/25");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/50");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/100");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/120");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/160");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/200");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/240");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/320");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/400");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/480");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/640");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/800");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/1000");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/1600");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/2000");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/3000");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/4000");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/6000");
            this.comboBox_Advance_Day_ShutterLimitMax.Items.Add("1/8000");

            this.comboBox_Advance_Day_AutoGainControl.Items.Add("6dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("12dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("18dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("24dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("30dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("36dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("42dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("48dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("54dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("60dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("66dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("72dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("78dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("84dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("90dB");
            this.comboBox_Advance_Day_AutoGainControl.Items.Add("96dB");

            this.comboBox_Advance_Day_3DNoiseReduction.Items.Add("Off");
            for (int i = 1; i <= 32; i++)
            {
                this.comboBox_Advance_Day_3DNoiseReduction.Items.Add(i.ToString());
            }

            this.comboBox_Advance_Day_SenseUp.Items.Add("Off");
            this.comboBox_Advance_Day_SenseUp.Items.Add("1 Frame");
            this.comboBox_Advance_Day_SenseUp.Items.Add("2 Frames");
            this.comboBox_Advance_Day_SenseUp.Items.Add("3 Frames");
            this.comboBox_Advance_Day_SenseUp.Items.Add("7 Frames");

            this.comboBox_Advance_Day_ColorMode.Items.Add("Color");
            this.comboBox_Advance_Day_ColorMode.Items.Add("Black/White");

            this.comboBox_Advance_Day_IRLED.Items.Add("Off");
            this.comboBox_Advance_Day_IRLED.Items.Add("On");

            this.comboBox_Advance_Day_IRCutFilter.Items.Add("Off");
            this.comboBox_Advance_Day_IRCutFilter.Items.Add("On");

            this.comboBox_Advance_Day_VideoQualityMode.Items.Add("General");
            this.comboBox_Advance_Day_VideoQualityMode.Items.Add("Low Light Color (ANPR, Color at Night)");
            this.comboBox_Advance_Day_VideoQualityMode.Items.Add("Extreme Low Light (Mono at Night)");
            this.comboBox_Advance_Day_VideoQualityMode.Items.Add("Hight Contrast Indoor (Mono at Night)");
//****//

            this.comboBox_Advance_Night_WhiteBalanceControl.Items.Add("Auto");
            this.comboBox_Advance_Night_WhiteBalanceControl.Items.Add("Tungsten");
            this.comboBox_Advance_Night_WhiteBalanceControl.Items.Add("Fixed Indoor");
            this.comboBox_Advance_Night_WhiteBalanceControl.Items.Add("Fixed Fluorescents 1");
            this.comboBox_Advance_Night_WhiteBalanceControl.Items.Add("Fixed Fluorescents 2");
            this.comboBox_Advance_Night_WhiteBalanceControl.Items.Add("Fixed Outdoor 1");
            this.comboBox_Advance_Night_WhiteBalanceControl.Items.Add("Fixed Outdoor 2");

            this.comboBox_Advance_Night_Mirror.Items.Add("Off");
            this.comboBox_Advance_Night_Mirror.Items.Add("On");

            this.comboBox_Advance_Night_Flip.Items.Add("Off");
            this.comboBox_Advance_Night_Flip.Items.Add("On");

            this.comboBox_Advance_Night_ExposureValue.Items.Add("1");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("2");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("3");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("4");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("5");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("6");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("7");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("8");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("9");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("10");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("11");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("12");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("13");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("14");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("15");
            this.comboBox_Advance_Night_ExposureValue.Items.Add("16");

            this.comboBox_Advance_Night_WDR.Items.Add("Off");
            this.comboBox_Advance_Night_WDR.Items.Add("Auto");
            this.comboBox_Advance_Night_WDR.Items.Add("x2");
            this.comboBox_Advance_Night_WDR.Items.Add("x3");
            this.comboBox_Advance_Night_WDR.Items.Add("x4");

            this.comboBox_Advance_Night_BackLightCompensation.Items.Add("Off");
            this.comboBox_Advance_Night_BackLightCompensation.Items.Add("On");
            
            this.comboBox_Advance_Night_DCIrisMode.Items.Add("Off");
            this.comboBox_Advance_Night_DCIrisMode.Items.Add("On");

            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/8000");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/6000");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/4000");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/3000");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/2000");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/1600");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/1000");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/800");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/640");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/480");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/400");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/320");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/240");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/200");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/160");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/120");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/100");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/50");
            this.comboBox_Advance_Night_ShutterLimitMin.Items.Add("1/25");

            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/25");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/50");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/100");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/120");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/160");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/200");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/240");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/320");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/400");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/480");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/640");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/800");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/1000");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/1600");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/2000");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/3000");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/4000");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/6000");
            this.comboBox_Advance_Night_ShutterLimitMax.Items.Add("1/8000");

            this.comboBox_Advance_Night_AutoGainControl.Items.Add("6dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("12dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("18dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("24dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("30dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("36dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("42dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("48dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("54dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("60dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("66dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("72dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("78dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("84dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("90dB");
            this.comboBox_Advance_Night_AutoGainControl.Items.Add("96dB");

            this.comboBox_Advance_Night_3DNoiseReduction.Items.Add("Off");
            for (int i = 1; i <= 32; i++)
            {
                this.comboBox_Advance_Night_3DNoiseReduction.Items.Add(i.ToString());
            }

            this.comboBox_Advance_Night_SenseUp.Items.Add("Off");
            this.comboBox_Advance_Night_SenseUp.Items.Add("1 Frame");
            this.comboBox_Advance_Night_SenseUp.Items.Add("2 Frames");
            this.comboBox_Advance_Night_SenseUp.Items.Add("3 Frames");
            this.comboBox_Advance_Night_SenseUp.Items.Add("7 Frames");

            this.comboBox_Advance_Night_ColorMode.Items.Add("Color");
            this.comboBox_Advance_Night_ColorMode.Items.Add("Black/White");

            this.comboBox_Advance_Night_IRLED.Items.Add("Off");
            this.comboBox_Advance_Night_IRLED.Items.Add("On");

            this.comboBox_Advance_Night_IRCutFilter.Items.Add("Off");
            this.comboBox_Advance_Night_IRCutFilter.Items.Add("On");

            this.comboBox_Advance_Night_VideoQualityMode.Items.Add("General");
            this.comboBox_Advance_Night_VideoQualityMode.Items.Add("Low Light Color (ANPR, Color at Night)");
            this.comboBox_Advance_Night_VideoQualityMode.Items.Add("Extreme Low Light (Mono at Night)");
            this.comboBox_Advance_Night_VideoQualityMode.Items.Add("Hight Contrast Indoor (Mono at Night)");


        }

        private void button_ToNight_Click(object sender, EventArgs e)
        {
            //this.panel_Basic_Day.Visible = false;
            this.panel_Basic_Night.Visible = true;
        }

        private void button_ToDay_Click(object sender, EventArgs e)
        {
            this.panel_Basic_Night.Visible = false;
            //this.panel_Basic_Day.Visible = true;
        }

        private void button_Goto_Advance_Day_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "quality?cmd=get&DayNight=0";
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
            this.GetAdvanceCGIContent(false);
            this.panel_Advance_Night.Visible = false;
        }

        private void button_Goto_Advance_Night_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "quality?cmd=get&DayNight=1";
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
            this.GetAdvanceCGIContent(true);
            this.panel_Advance_Night.Visible = true;
        }

        private void button_Basic_Day_Brightness_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=0";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "bright")
                {
                    textBox_Basic_Day_Brightness.Text = item.Value;
                    trackBar_Basic_Day_Brightness.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Day_Contrast_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=0";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "contrast")
                {
                    textBox_Basic_Day_Contrast.Text = item.Value;
                    trackBar_Basic_Day_Contrast.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;  
        }

        private void button_Basic_Day_Hue_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=0";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "Hue")
                {
                    textBox_Basic_Day_Hue.Text = item.Value;
                    trackBar_Basic_Day_Hue.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;  
        }

        private void button_Basic_Day_Saturation_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=0";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "Saturation")
                {
                    textBox_Basic_Day_Saturation.Text = item.Value;
                    trackBar_Basic_Day_Saturation.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;  
        }

        private void button_Basic_Day_Sharpness_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=0";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "Sharpness")
                {
                    textBox_Basic_Day_Sharpness.Text = item.Value;

                    int val = 0;
                    try
                    {
                        val = Int32.Parse(item.Value);
                    }
                    catch (Exception ex)
                    {
                        val = 0; 
                    }
                    trackBar_Basic_Day_Sharpness.Value = val;
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;  
        }

        private void button_Basic_Day_Cam_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=0";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "camstat")
                {
                    textBox_Basic_Day_Camstat.Text = item.Value;

                    int val = 0;
                    try
                    {
                        val = Int32.Parse(item.Value);
                    }
                    catch (Exception ex)
                    { 
                        val = 0;
                    }
                    trackBar_Basic_Day_Camstat.Value = val;
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;  
        }

        private void button_Basic_Day_Brightness_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=0&bright=" + textBox_Basic_Day_Brightness.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Day_Contrast_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=0&contrast=" + textBox_Basic_Day_Contrast.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Day_Hue_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=0&hue=" + textBox_Basic_Day_Hue.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Day_Saturation_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=0&Saturation=" + textBox_Basic_Day_Saturation.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Day_Sharpness_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=0&Sharpness=" + textBox_Basic_Day_Sharpness.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Day_Camstat_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=0&camstat=" + textBox_Basic_Day_Camstat.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Brightness_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=1";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "bright")
                {
                    textBox_Basic_Night_Brightness.Text = item.Value;
                    trackBar_Basic_Night_Brightness.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Contrast_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=1";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "contrast")
                {
                    textBox_Basic_Night_Contrast.Text = item.Value;
                    trackBar_Basic_Night_Contrast.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Hue_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=1";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "hue")
                {
                    textBox_Basic_Night_Hue.Text = item.Value;
                    trackBar_Basic_Night_Hue.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Saturation_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=1";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "Saturation")
                {
                    textBox_Basic_Night_Saturation.Text = item.Value;
                    trackBar_Basic_Night_Saturation.Value = Int32.Parse(item.Value);
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Sharpness_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=1";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "Sharpness")
                {
                    textBox_Basic_Night_Sharpness.Text = item.Value;
                    int val = 0;
                    try
                    {
                        val = Int32.Parse(item.Value);
                    }
                    catch (Exception ex)
                    {
                        val = 0;
                    }
                    trackBar_Basic_Night_Sharpness.Value = val;
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Camstat_Get_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=get&DayNight=1";
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            foreach (var item in parseCGI.Commands2)
            {
                if (item.Key == "camstat")
                {
                    textBox_Basic_Night_Camstat.Text = item.Value;
                    int val = 0;
                    try
                    {
                        val = Int32.Parse(item.Value);
                    }
                    catch (Exception ex)
                    { 
                        val = 0;
                    }
                    trackBar_Basic_Night_Camstat.Value = val;
                }
            }
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Brightness_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=1&bright=" + textBox_Basic_Night_Brightness.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Contrast_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=1&contrast=" + textBox_Basic_Night_Contrast.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Hue_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=1&Hue=" + textBox_Basic_Night_Hue.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Saturation_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=1&Saturation=" + textBox_Basic_Night_Saturation.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Sharpness_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=1&Sharpness=" + textBox_Basic_Night_Sharpness.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }

        private void button_Basic_Night_Camstat_Set_Click(object sender, EventArgs e)
        {
            string req = null;
            string command = "camctrl?cmd=set&DayNight=1&camstat=" + textBox_Basic_Night_Camstat.Text;
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command.Text = command;
            textBox_CGI_Command_Response.Text = req;
        }
        //***//
        private void comboBox_Advance_Night_WhiteBalanceControl_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&AWB=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_Mirror_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&MIRROR=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_Flip_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&FLIP=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_ExposureValue_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&EV=" + (senderComboBox.SelectedIndex + 1).ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_WDR_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&WDR=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_BackLightXCompensation_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&BLC=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_DCIrisMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&IrisMode=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_ShutterLimitMin_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string value = senderComboBox.Items[senderComboBox.SelectedIndex].ToString();
            value = value.Substring(2);
            string req = null;
            string command = "quality?cmd=set&DayNight=1&ShutterLimitmin=" + value.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_ShutterLimitMax_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string value = senderComboBox.Items[senderComboBox.SelectedIndex].ToString();
            value = value.Substring(2);
            string req = null;
            string command = "quality?cmd=set&DayNight=1&ShutterLimitmax=" + value.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_AutoGainControl_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&AGC=" + (senderComboBox.SelectedIndex + 1).ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_3DNoiseReduction_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&NoiseReduction=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_SenseUp_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&SenseUp=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_ColorMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&MW_DayNight_N=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_IRLED_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&IR_LED_N=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_IRCutFilter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=1&IR_CUT_N=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Night_VideoQualityMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "defquality.cgi?def=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_WhiteBalanceComtrol_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&AWB=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_Mirror_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&MIRROR=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_Flip_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&FLIP=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_ExposureValue_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&EV=" + (senderComboBox.SelectedIndex + 1).ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_WDR_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&WDR=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_BackLightCompensation_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&BLC=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_DCIrisMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&IrisMode=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_ShutterLimitMin_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string value = senderComboBox.Items[senderComboBox.SelectedIndex].ToString();
            value = value.Substring(2);
            string req = null;
            string command = "quality?cmd=set&DayNight=0&ShutterLimitmin=" + value.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_ShutterLimitMax_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string value = senderComboBox.Items[senderComboBox.SelectedIndex].ToString();
            value = value.Substring(2);
            string req = null;
            string command = "quality?cmd=set&DayNight=0&ShutterLimitmax=" + value.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_AutoGainControl_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&AGC=" + (senderComboBox.SelectedIndex + 1).ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_3DNoiseReduction_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&NoiseReduction=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_SenseUp_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&SenseUp=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_ColorMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&MW_DayNight_N=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_IRLED_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&IR_LED_N=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_IRCutFilter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "quality?cmd=set&DayNight=0&IR_CUT_N=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void comboBox_Advance_Day_VideoQualityMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            string req = null;
            string command = "defquality.cgi?def=" + senderComboBox.SelectedIndex.ToString();
            sendCGI.Send(command, ref req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
        }

        private void GetAdvanceCGIContent(bool bDayNight)
        {
            string req = null;
            string command = "quality?cmd=get&DayNight=" + (bDayNight ? "1" : "0");
            sendCGI.Send(command, ref req);
            parseCGI.Parse(req);
            textBox_Sended_CGI_Command_Advance.Text = command;
            textBox_CGI_Command_Response_Advance.Text = req;
            foreach (var item in parseCGI.Commands2)
            {
                switch (item.Key)
                {
                    case "DayNight":
                        break;
                    case "EV":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_ExposureValue.SelectedIndex = Int32.Parse(item.Value) - 1;
                        }
                        else
                        {
                            this.comboBox_Advance_Day_ExposureValue.SelectedIndex = Int32.Parse(item.Value) - 1;
                        }
                        break;
                    case "WDR":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_WDR.SelectedIndex = Int32.Parse(item.Value) - 1;
                        }
                        else
                        {
                            this.comboBox_Advance_Day_WDR.SelectedIndex = Int32.Parse(item.Value) - 1;
                        }
                        break;
                    case "BLC":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_BackLightCompensation.SelectedIndex = Int32.Parse(item.Value);   
                        }
                        else
                        {
                            this.comboBox_Advance_Day_BackLightCompensation.SelectedIndex = Int32.Parse(item.Value);
                        }
                        break;
                    case "ShutterLimitmin":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_ShutterLimitMin.SelectedText = ("1/" + item.Value);
                        }
                        else
                        {
                            this.comboBox_Advance_Day_ShutterLimitMin.SelectedText = ("1/" + item.Value);
                        }
                        break;
                    case "ShutterLimitmax":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_ShutterLimitMax.SelectedText = ("1/" + item.Value);
                        }
                        else
                        {
                            this.comboBox_Advance_Day_ShutterLimitMax.SelectedText = ("1/" + item.Value);
                        }
                        break;
                    case "AGC":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_AutoGainControl.SelectedIndex = Int32.Parse(item.Value) - 1;
                        }
                        else
                        {
                            this.comboBox_Advance_Day_AutoGainControl.SelectedIndex = Int32.Parse(item.Value) - 1;
                        }
                        break;
                    case "NoiseReduction":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_3DNoiseReduction.SelectedIndex = Int32.Parse(item.Value);
                        }
                        else
                        {
                            this.comboBox_Advance_Day_3DNoiseReduction.SelectedIndex = Int32.Parse(item.Value);
                        }
                        break;
                    case "SenseUp":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_SenseUp.SelectedIndex = Int32.Parse(item.Value);
                        }
                        else
                        {
                            this.comboBox_Advance_Day_SenseUp.SelectedIndex = Int32.Parse(item.Value);
                        }
                        break;
                    case "AWB":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_WhiteBalanceControl.SelectedIndex = Int32.Parse(item.Value);
                        }
                        else
                        {
                            this.comboBox_Advance_Day_WhiteBalanceControl.SelectedIndex = Int32.Parse(item.Value);
                        }
                        break;
                    case "MIRROR":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_Mirror.SelectedIndex = Int32.Parse(item.Value);
                        }
                        else
                        {
                            this.comboBox_Advance_Day_Mirror.SelectedIndex = Int32.Parse(item.Value);
                        }
                        break;
                    case "FLIP":
                        if (bDayNight)
                        {
                            this.comboBox_Advance_Night_Flip.SelectedIndex = Int32.Parse(item.Value);
                        }
                        else 
                        {
                            this.comboBox_Advance_Day_Flip.SelectedIndex = Int32.Parse(item.Value);
                        }
                        break;
                    default:
                        break;
                }

            }

        }
    }
}

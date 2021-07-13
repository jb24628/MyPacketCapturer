using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;
using SharpPcap;

namespace MyPackedCapturer
{
    public partial class FormCapture : Form
    {
        CaptureDeviceList devices;
        public static ICaptureDevice iCapDevice;
        public static string stringPackets = "";
        static int numPackets = 0;
        FormSend formSend; //send form

        public FormCapture()
        {
            devices = CaptureDeviceList.Instance;
            InitializeComponent();

            if (devices.Count < 1)
            {
                MessageBox.Show("No Capture Devices Found!");
                Application.Exit();
            }

            foreach (ICaptureDevice device in devices)
            {
                comboBoxDevices.Items.Add(device.Description);
            }

            iCapDevice = devices[2];
            comboBoxDevices.Text = iCapDevice.Description;

            iCapDevice.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeOutMillis = 1000;
           iCapDevice.Open(DeviceMode.Promiscuous, readTimeOutMillis);
        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {
            numPackets++;
            byte[] data = packet.Packet.Data;

            stringPackets += "Packet " + numPackets + ":" + Environment.NewLine;

            int byteCounter = 0;

            stringPackets += "Destination MAC Address";

            foreach (byte b in data)
            {
                if (byteCounter<=13)
                {
                    stringPackets += b.ToString("X2") + " ";
                }

                
                byteCounter++;

                switch(byteCounter)
                {
                    case 6: 
                        stringPackets += Environment.NewLine;
                        stringPackets += "Source MAC Address:";
                        break;
                    case 12:
                        stringPackets += Environment.NewLine;
                        stringPackets += "EtherType:";
                        break;
                    case 14:
                        if (data[12]==8)
                        {
                            if(data[13] ==0)
                            {
                                stringPackets += "(IP)";
                            }
                            if (data[13] == 6)
                            {
                                stringPackets += "(ARP)";
                            }
                        }
                        stringPackets += "(IP)";
                        break;
                }
            }

            stringPackets += Environment.NewLine + "" + Environment.NewLine +"Raw Data" + Environment.NewLine;

            foreach (byte b in data)
            {
                stringPackets += b.ToString("X2") + " ";
                    byteCounter++;

                if (byteCounter % 16 == 0 && byteCounter > 0)
                {
                    stringPackets += Environment.NewLine;
                }
            }

            stringPackets += Environment.NewLine;
            stringPackets += Environment.NewLine;

        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnStartStop.Text.Equals("Start"))
                {
                    iCapDevice.StartCapture();
                    timer1.Enabled = true;
                    btnStartStop.Text = "Stop";
                }
                else
                {
                    iCapDevice.StopCapture();
                    timer1.Enabled = false;
                    btnStartStop.Text = "Start";
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBoxCapturedData.AppendText(stringPackets);
            stringPackets = "";
            textBoxNumOfPackets.Text = Convert.ToString(numPackets);
        }

        private void comboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            iCapDevice = devices[comboBoxDevices.SelectedIndex];
            comboBoxDevices.Text = iCapDevice.Description;

            iCapDevice.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeOutMillis = 1000;
            iCapDevice.Open(DeviceMode.Promiscuous, readTimeOutMillis);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName !="")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, textBoxCapturedData.Text);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textBoxCapturedData.Text =System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void sendWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormSend.instantiations == 0)
            {
                formSend = new FormSend();
                formSend.Show();
            }
            
        }
    }
}

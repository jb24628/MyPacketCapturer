using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPackedCapturer
{
    public partial class FormSend : Form
    {

        public static int instantiations = 0;

        public FormSend()
        {
            InitializeComponent();
            instantiations++;
        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                textBoxPacket.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
            
        }
    

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
        saveFileDialog1.Title = "Save the Captured Packets";
        saveFileDialog1.ShowDialog();

        if (saveFileDialog1.FileName != "")
        {
            System.IO.File.WriteAllText(saveFileDialog1.FileName, textBoxPacket.Text);
        }
    }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string stringBytes = "";

            foreach(string s in textBoxPacket.Lines) {
                string[] noComments = s.Split('#');
                string s1 = noComments[0];
                stringBytes += s1 + Environment.NewLine;

            }

            string[] strBytes = stringBytes.Split(new string[] { "\n", "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries);

            byte[] packet = new byte[strBytes.Length];

            int i = 0;

            foreach(string s in strBytes)
            {
                packet[i] = Convert.ToByte(s, 16);
                i++;
            }

            try
            {
                FormCapture.iCapDevice.SendPacket(packet);
            }
            catch(Exception ex)
            {

            }
        }

        private void FormSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            instantiations--;
        }
    }
}

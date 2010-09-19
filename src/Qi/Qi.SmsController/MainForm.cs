using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Qi.Sms.DeviceConnections;
using Qi.Sms;
using Qi.Sms.Protocol;
using System.Threading;

namespace Qi.SmsController
{
    public partial class MainForm : Form
    {
        SmsService smsService;
        ComConnection com;
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.richTextBoxOutput.Text = "";
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.comboBoxBIT.SelectedIndex = 0;
            this.comboBoxCOMList.SelectedIndex = 0;


        }

        void com_ReceivedOrReceiveEvent(object sender, Qi.Sms.DeviceCommandEventHandlerArgs e)
        {
            if (!InvokeRequired)
            {
                this.richTextBoxOutput.Text = e.Command + this.richTextBoxOutput.Text;
            }
            else
            {
                this.Invoke(new Action<object, DeviceCommandEventHandlerArgs>(com_ReceivedOrReceiveEvent), sender, e);
            }
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            com.Send(new DirectCommand(textBoxCommandLine.Text));
        }

        private void ButtonConnect_Click_1(object sender, EventArgs e)
        {
            if (this.ButtonConnect.Tag == null)
            {
                try
                {
                    com = new ComConnection(this.comboBoxCOMList.SelectedItem.ToString(), Convert.ToInt32(this.comboBoxBIT.SelectedItem.ToString()));
                    com.ReceivedEvent += new EventHandler<Qi.Sms.DeviceCommandEventHandlerArgs>(com_ReceivedOrReceiveEvent);
                    com.SendingEvent += new EventHandler<Qi.Sms.DeviceCommandEventHandlerArgs>(com_ReceivedOrReceiveEvent);
                    com.Open();

                    smsService = new SmsService(com);
                    this.ButtonConnect.Text = "Disconnect";
                    this.ButtonConnect.Tag = true;
                    this.tabControl1.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    com.Dispose();
                }

            }
            else
            {
                smsService.Dispose();
                this.ButtonConnect.Tag = null;
                this.ButtonConnect.Text = "Connect";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pc = new ProgressControl();
            var action = new Action<object>(delegate(object progressControl1)
            {
                var progressControl = (ProgressControl)progressControl1;
                for (int i = 1; i <= 50; i++)
                {
                    if (progressControl.Cancel)
                        break;
                    smsService.Delete(i);
                }
            });

            ThreadPool.QueueUserWorkItem(new WaitCallback(action), pc);
            ProgressForm form = new ProgressForm(pc);
            form.ShowDialog();
        }


    }
}

using System;
using System.Threading;
using System.Windows.Forms;
using Qi.Sms;
using Qi.Sms.DeviceConnections;
using Qi.Sms.Protocol;

namespace Qi.SmsController
{
    public partial class MainForm : Form
    {
        private ComConnection com;
        private SmsService smsService;

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            richTextBoxOutput.Text = "";
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxBIT.SelectedIndex = 0;
            comboBoxCOMList.SelectedIndex = 0;
        }

        private void com_ReceivedOrReceiveEvent(object sender, DeviceCommandEventHandlerArgs e)
        {
            if (!InvokeRequired)
            {
                richTextBoxOutput.Text = e.Command + richTextBoxOutput.Text;
            }
            else
            {
                Invoke(new Action<object, DeviceCommandEventHandlerArgs>(com_ReceivedOrReceiveEvent), sender, e);
            }
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            com.Send(new DirectCommand(textBoxCommandLine.Text));
        }

        private void ButtonConnect_Click_1(object sender, EventArgs e)
        {
            if (ButtonConnect.Tag == null)
            {
                try
                {
                    com = new ComConnection(comboBoxCOMList.SelectedItem.ToString(),
                                            Convert.ToInt32(comboBoxBIT.SelectedItem.ToString()));
                    com.ReceivedEvent += com_ReceivedOrReceiveEvent;
                    com.SendingEvent += com_ReceivedOrReceiveEvent;
                    com.Open();

                    smsService = new SmsService(com);
                    ButtonConnect.Text = "Disconnect";
                    ButtonConnect.Tag = true;
                    tabControl1.Enabled = true;
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
                ButtonConnect.Tag = null;
                ButtonConnect.Text = "Connect";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pc = new ProgressControl();
            var action = new Action<object>(delegate(object progressControl1)
                                                {
                                                    var progressControl = (ProgressControl) progressControl1;
                                                    for (int i = 1; i <= 50; i++)
                                                    {
                                                        if (progressControl.Cancel)
                                                            break;
                                                        smsService.Delete(i);
                                                    }
                                                });

            ThreadPool.QueueUserWorkItem(new WaitCallback(action), pc);
            var form = new ProgressForm(pc);
            form.ShowDialog();
        }
    }
}
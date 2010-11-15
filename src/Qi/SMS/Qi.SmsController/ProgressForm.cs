using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Qi.SmsController
{
    public partial class ProgressForm : Form
    {
        ProgressControl _progressControl;
        public ProgressForm(ProgressControl pc)
        {
            _progressControl = pc;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _progressControl.Cancel = true;
            this.Close();
        }
    }
}

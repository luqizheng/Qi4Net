namespace Qi.SmsController
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ButtonSend = new System.Windows.Forms.Button();
            this.textBoxCommandLine = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClear = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxBIT = new System.Windows.Forms.ComboBox();
            this.comboBoxCOMList = new System.Windows.Forms.ComboBox();
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBoxSmsCenter = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Enabled = false;
            this.tabControl1.Location = new System.Drawing.Point(0, 41);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1011, 229);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBoxSmsCenter);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1003, 203);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "BaseCommand";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Delete All Message";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ButtonSend);
            this.tabPage2.Controls.Add(this.textBoxCommandLine);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(598, 137);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "AT Commander";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ButtonSend
            // 
            this.ButtonSend.Location = new System.Drawing.Point(278, 18);
            this.ButtonSend.Name = "ButtonSend";
            this.ButtonSend.Size = new System.Drawing.Size(75, 23);
            this.ButtonSend.TabIndex = 1;
            this.ButtonSend.Text = "Send";
            this.ButtonSend.UseVisualStyleBackColor = true;
            this.ButtonSend.Click += new System.EventHandler(this.ButtonSend_Click);
            // 
            // textBoxCommandLine
            // 
            this.textBoxCommandLine.Location = new System.Drawing.Point(8, 18);
            this.textBoxCommandLine.Name = "textBoxCommandLine";
            this.textBoxCommandLine.Size = new System.Drawing.Size(264, 21);
            this.textBoxCommandLine.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBoxOutput);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 270);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1011, 180);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxOutput.Location = new System.Drawing.Point(3, 52);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.Size = new System.Drawing.Size(1005, 125);
            this.richTextBoxOutput.TabIndex = 6;
            this.richTextBoxOutput.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonClear);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1005, 35);
            this.panel1.TabIndex = 5;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(4, 6);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 5;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxBIT);
            this.groupBox2.Controls.Add(this.comboBoxCOMList);
            this.groupBox2.Controls.Add(this.ButtonConnect);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1011, 41);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connection";
            // 
            // comboBoxBIT
            // 
            this.comboBoxBIT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBIT.FormattingEnabled = true;
            this.comboBoxBIT.Items.AddRange(new object[] {
            "9600",
            "38400",
            "57600"});
            this.comboBoxBIT.Location = new System.Drawing.Point(131, 15);
            this.comboBoxBIT.Name = "comboBoxBIT";
            this.comboBoxBIT.Size = new System.Drawing.Size(121, 20);
            this.comboBoxBIT.TabIndex = 5;
            // 
            // comboBoxCOMList
            // 
            this.comboBoxCOMList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCOMList.FormattingEnabled = true;
            this.comboBoxCOMList.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.comboBoxCOMList.Location = new System.Drawing.Point(3, 15);
            this.comboBoxCOMList.Name = "comboBoxCOMList";
            this.comboBoxCOMList.Size = new System.Drawing.Size(121, 20);
            this.comboBoxCOMList.TabIndex = 4;
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Location = new System.Drawing.Point(258, 12);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(75, 23);
            this.ButtonConnect.TabIndex = 3;
            this.ButtonConnect.Text = "Connect";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(235, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "GetSmsCenter";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxSmsCenter
            // 
            this.textBoxSmsCenter.Location = new System.Drawing.Point(8, 8);
            this.textBoxSmsCenter.Name = "textBoxSmsCenter";
            this.textBoxSmsCenter.Size = new System.Drawing.Size(221, 21);
            this.textBoxSmsCenter.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Sms控制台";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button ButtonSend;
        private System.Windows.Forms.TextBox textBoxCommandLine;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBoxBIT;
        private System.Windows.Forms.ComboBox comboBoxCOMList;
        private System.Windows.Forms.Button ButtonConnect;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxSmsCenter;
        private System.Windows.Forms.Button button2;
    }
}


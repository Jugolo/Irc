using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Forms
{
    public partial class ChannelButton : UserControl
    {
        private Form1 Main;

        public ChannelButton(Form1 main)
        {
            this.Main = main;
            InitializeComponent();
        }

        public void Write(string line)
        {
            this.richTextBox1.Text = line;
        }

        public void Send()
        {
            this.button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.richTextBox1.Text.TrimStart().Length > 0)
            {
                this.Main.GetSendMessage(this.richTextBox1.Text.Trim());
            }
            this.richTextBox1.Text = "";
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.button1.PerformClick();
            }
        }
    }
}

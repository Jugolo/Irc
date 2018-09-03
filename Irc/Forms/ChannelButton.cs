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
        public delegate void OnSend(string msg);

        private OnSend events;

        public ChannelButton()
        {
            InitializeComponent();
            this.changeSize();
        }

        public void AddEvent(OnSend send)
        {
            this.events = send;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChannelButton_SizeChanged(object sender, EventArgs e)
        {
            this.changeSize();
        }

        private void changeSize()
        {
        }

        private void ChannelButton_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.richTextBox1.Text.TrimStart().Length > 0 && this.events != null)
            {
                this.events(this.richTextBox1.Text);
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

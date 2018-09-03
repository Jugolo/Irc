using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Forms
{
    public partial class NewServer : Form
    {
        private Form1 main;

        public NewServer(Form1 main)
        {
            this.main = main;
            InitializeComponent();
        }

        private void NewServer_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string server = textBox2.Text.Trim();
            string Sport = textBox3.Text.Trim();
            string nick = textBox4.Text.Trim();
            string[] channels = richTextBox1.Text.Trim().Split(',');
            int port;

            if (name.Length == 0 || server.Length == 0 || Sport.Length == 0 || !int.TryParse(Sport, out port) || nick.Length == 0 || channels.Length == 0)
            {
                MessageBox.Show("Not all forms is filled!", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                this.main.SaveServer(name, server, port, nick, channels);
                this.Close();
            }
        }
    }
}

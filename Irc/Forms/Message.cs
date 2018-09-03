using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irc.Irc;

namespace Irc.Forms
{
    public partial class Message : UserControl
    {
        private Dictionary<string, Dictionary<string, ChannelInfo>> MP = new Dictionary<string, Dictionary<string, ChannelInfo>>();
        private string Identify = "";
        private string Channel = "";
        private delegate void RP();
        private int StartLine = 0;//start at line 0

        public Message()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
        }

        private void Message_Load(object sender, EventArgs e)
        {

            vScrollBar1.Maximum = 0;
            vScrollBar1.Minimum = 0;
        }

        public string GetSelectedIdentify()
        {
            return this.Identify;
        }

        public string GetSelectedChannel()
        {
            return this.Channel;
        }

        public void Select(string identify, string channel)
        {
            this.Identify = identify;
            this.Channel = channel;
            this.ChangeLines(false);
            if (this.MP.ContainsKey(identify) && this.MP[identify].ContainsKey(channel))
            {
                vScrollBar1.Value = this.MP[identify][channel].CurrentLine;
                this.StartLine = vScrollBar1.Value;
            }
            this.RePaint();
        }

        public bool ChannelExists(string identify, string channel)
        {
            return this.MP.ContainsKey(identify) && this.MP[identify].ContainsKey(channel);
        }

        public void ShowLine(string identify, string channel, string from, string message)
        {
            if (!this.MP.ContainsKey(identify))
                this.MP.Add(identify, new Dictionary<string, ChannelInfo>());

            if (!this.MP[identify].ContainsKey(channel))
                this.MP[identify].Add(channel, new ChannelInfo());

            this.MP[identify][channel].MessageParts.Add(new MessageParts(from, message));

            this.RePaint();
        }

        public void RemoveServer(string identify)
        {
            if (this.MP.ContainsKey(identify))
            {
                this.MP.Remove(identify);
                this.RePaint();
            }
        }

        public bool RemoveChannel(string identify, string channel)
        {
            if(MP.ContainsKey(identify) && MP[identify].ContainsKey(channel))
            {
                this.MP[identify].Remove(channel);
                this.RePaint();
                return true;
            }

            return false;
        }

        public string GetTopServer()
        {
            if (this.MP.Count == 0)
                return null;

            return this.MP.Keys.ToArray()[0];
        }

        public string GetTopChannel(string identify)
        {
            if (!this.MP.ContainsKey(identify) || this.MP[identify].Count == 0)
                return null;

            return this.MP[identify].Keys.ToArray()[0];
        }

        private void Message_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            List<MessageParts> messages = this.GetMessageParts();
            if (messages == null)
                return;
            TextSize size = new TextSize(this.StartLine);
            
            //g.DrawString("Dette er en test", new Font("Arial", 8), new SolidBrush(Color.Black), 0, 0);

            for(int i = 0; i < messages.Count; i++)
            {
                if (!size.ShouldShow())
                {
                    size.NextLine();
                    continue;
                }
                messages[i].Draw(g, size, this.ClientSize.Width - this.vScrollBar1.Width);
                size.NextLine();
            }

        }

        private List<MessageParts> GetMessageParts()
        {
            if (!this.MP.ContainsKey(this.Identify))
            {
                return null;
            }

            Dictionary<string, ChannelInfo> sub = this.MP[this.Identify];
            if (sub.ContainsKey(this.Channel))
                return sub[this.Channel].MessageParts;
            return null;
        }

        private void RePaint()
        {
            this.BeginInvoke(new RP(DoRePaint), new object[0]);
        }

        private void DoRePaint()
        {
            this.Refresh();
        }

        private void Message_Resize(object sender, EventArgs e)
        {
            this.ChangeLines(true);
        }

        private void ChangeLines(bool resize) { 
            List<MessageParts> parts = this.GetMessageParts();
            if(parts == null)
            {
                return;
            }
            int line = 0;

            for(int i = 0; i < parts.Count; i++)
            {
                if (resize)
                    parts[i].OnResize();
                line += parts[i].Lines;
            }
            this.RePaint();
            vScrollBar1.Maximum = line;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.StartLine = e.NewValue;
            this.MP[this.GetSelectedIdentify()][this.GetSelectedChannel()].CurrentLine = this.StartLine;
            this.RePaint();
        }
    }
}

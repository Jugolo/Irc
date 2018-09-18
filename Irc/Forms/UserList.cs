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
    public partial class UserList : UserControl
    {

        private Dictionary<string, Dictionary<string, List<UserInfo>>> user = new Dictionary<string, Dictionary<string, List<UserInfo>>>();
        private List<UserInfo> Current;
        private Form1 Main;
        private Channel Channel;
        private delegate void RP();
        private delegate void D(bool value);

        public UserList(Form1 main, Channel channel)
        {
            Main = main;
            Channel = channel;
            InitializeComponent();
        }

        public void AppendUser(string identify, string channel, UserInfo info)
        {
            if (!user.ContainsKey(identify))
                user.Add(identify, new Dictionary<string, List<UserInfo>>());

            if (!user[identify].ContainsKey(channel))
                user[identify].Add(channel, new List<UserInfo>());

            user[identify][channel].Add(info);
            this.RePaint();
        }

        public bool RemoveUser(string identify, string channel, string nick)
        {
            if(user.ContainsKey(identify) && user[identify].ContainsKey(channel))
            {
                List<UserInfo> info = user[identify][channel];
                for(int i = 0; i < info.Count; i++)
                {
                    if(info[i].Nick == nick)
                    {
                        info.Remove(info[i]);
                        return true;
                    }
                }
            }
            return false;
        }

        public string[] UpdateNick(string identify, string oldnick, string newnick)
        {
            List<string> array = new List<string>();
            if (user.ContainsKey(identify))
            {
                Dictionary<string, List<UserInfo>> channels = this.user[identify];
                foreach(string channel in channels.Keys)
                {

                }
            }
            return array.ToArray();
        }

        public void RemoveChannel(string identify, string channel)
        {
            if(this.user.ContainsKey(identify) && this.user.ContainsKey(channel))
            {
                if (this.Current == this.user[identify][channel])
                    this.Current = null;
                this.user[identify].Remove(channel);
                this.RePaint();
            }
        }

        public void RemoveServer(string identify)
        {
            if (this.user.ContainsKey(identify))
            {
                this.user.Remove(identify);
            }
        }

        public void Select(string identify, string channel)
        {
            bool d = channel.IndexOf("#") == 0;
            this.Display(d);

            if (!d)
                return;

            if(this.user.ContainsKey(identify) && this.user[identify].ContainsKey(channel))
            {
                this.Current = this.user[identify][channel];
            }
            else
            {
                this.Current = null;
            }

            this.RePaint();
        }

        private void UserList_Paint(object sender, PaintEventArgs e)
        {
            if (this.Current == null)
                return;

            Graphics g = e.Graphics;
            for(int i = 0; i < Current.Count; i++)
            {
                Color color = Color.Black;
                string prefix = "";
                if (Current[i].Op)
                {
                    prefix = "@";
                    color = Color.Red;
                }else if (Current[i].Voice)
                {
                    prefix = "+";
                    color = Color.Blue;
                }
                g.DrawString(prefix+Current[i].Nick, new Font("Arial", 8), new SolidBrush(color), 5, i*10);
            }
        }

        private void RePaint()
        {
            this.BeginInvoke(new RP(DoRePaint), new object[0]);
        }

        private void DoRePaint()
        {
            this.Refresh();
        }

        private void Display(bool value)
        {
            this.BeginInvoke(new D(HandleDisplay), new object[] { value });
        }

        private void HandleDisplay(bool value)
        {
            if (!value)
                this.Hide();
            else
                this.Show();
        }

        private void UserList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int line = e.Y / 10;
                if (line >= 0 && line < Current.Count)
                {
                    contextMenuStrip1.Items.Clear();
                    contextMenuStrip1.Items.Add(Current[line].Nick).Enabled = false;
                    contextMenuStrip1.Items.Add("Query").Click += QueryUserClick;
                    contextMenuStrip1.Items.Add("Slap").Click += SlqpUserClick;
                    contextMenuStrip1.Show(this, e.X, e.Y);
                }
                else
                    MessageBox.Show("Line: "+line);
            }
        }

        private void SlqpUserClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            int line = (item.Owner.Location.Y / 10) - 8;
            
        }

        private void QueryUserClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            int line = (item.Owner.Location.Y / 10) - 8;
            this.Main.AppendChannel(this.Channel.SelectedIdentify(), Current[line].Nick);
        }
    }
}

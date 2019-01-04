using Irc.Irc;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Irc.Forms
{
    public partial class UserList : UserControl
    {
        private ChannelButton Input;
        private delegate void Dummy();
        private List<UserInfo> CurrentUsers = new List<UserInfo>();
        private Form1 Main;

        public UserList(ChannelButton input, Form1 Main)
        {
            InitializeComponent();
            this.Input = input;
            this.Main = Main;
        }

        public void Empty()
        {
            this.listBox1.BeginInvoke(new Dummy(() => {
                this.listBox1.Items.Clear();
            }));
            this.CurrentUsers = new List<UserInfo>();
        }

        public void AppendUsers(List<UserInfo> info)
        {
            this.listBox1.BeginInvoke(new Dummy(() =>
            {
                this.listBox1.BeginUpdate();
                for (int i = 0; i < info.Count; i++)
                {
                    this.listBox1.Items.Add(GetPrefix(info[i]) + info[i].Nick);
                }
                this.listBox1.EndUpdate();
            }));
            this.CurrentUsers.AddRange(info);
        }

        private string GetPrefix(UserInfo info)
        {
            if (info.Op)
                return "@";
            if (info.Voice)
                return "+";
            return "";
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Brush color = Brushes.Black;

            string context = this.listBox1.Items[e.Index].ToString();
            char prefix = context.ToCharArray()[0];
            if (prefix == '@')
                color = Brushes.Red;
            if (prefix == '+')
                color = Brushes.Blue;
            
            e.Graphics.DrawString(context, e.Font, color, e.Bounds, StringFormat.GenericDefault);

            e.DrawFocusRectangle();
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = this.listBox1.Font.Height;
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Items.Clear();
                int index = this.listBox1.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    string nick = this.GetUser(this.listBox1.Items[index].ToString());
                    this.contextMenuStrip1.Items.Add(nick).Enabled = false;
                    this.contextMenuStrip1.Items.Add("Query").Click += (s, a) =>
                    {
                        this.Main.AppendChannel(
                            this.Main.GetIdentify,
                            nick
                            );
                    };
                    this.contextMenuStrip1.Items.Add("Slap").Click += (s, a) =>
                    {
                        this.Input.Write("/me slap " + nick + " just becuse i can");
                        this.Input.Send();
                    };
                    this.contextMenuStrip1.Show();
                }
            }
        }

        private void UserList_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private string GetUser(string name)
        {
            if (name[0] == '@' || name[0] == '+')
                name = name.Substring(1);

            return name;
        }
    }
}

using Irc.Irc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Irc.Forms
{
    public partial class Message : UserControl
    {
        private delegate void DoEmpty();

        public Message()
        {
            InitializeComponent();
        }

        public void Write(MessageData data)
        {
            data.AppendLine(this.richTextBox1 as InvokeSafeRichTextBox);
        }

        public void Empty()
        {
            this.richTextBox1.BeginInvoke(new DoEmpty(() => {
                richTextBox1.Text = "";
            }));
        }
    }
}

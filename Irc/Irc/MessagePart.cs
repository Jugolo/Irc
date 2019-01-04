using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Irc
{
    class MessagePart
    {
        public Color FontColor;
        public Color BackGround;
        public bool Bold;
        public bool Underline;
        public string Text = "";

        public MessagePart Clone()
        {
            MessagePart buffer = new MessagePart();
            buffer.FontColor = this.FontColor;
            buffer.BackGround = this.BackGround;
            buffer.Bold = this.Bold;
            buffer.Underline = this.Underline;
            return buffer;
        }
    }
}

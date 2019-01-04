using Irc.Forms;
using Irc.Script.Token;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Irc
{
    public class MessageData
    {
        private DateTime time;
        private string nick;
        private List<MessagePart> message = new List<MessagePart>();

        public MessageData(string nick, string message)
        {
            this.time = DateTime.Now;
            this.nick = nick;
            this.ParseMessage(message);
        }

        public void AppendLine(InvokeSafeRichTextBox form)
        {
            form.AppendText("[" + time.Hour + ":" + time.Minute + "]" + (nick == null ? "" : nick + ": "));
            for(int i = 0; i < message.Count; i++)
            {
                form.AppendColoredText(this.message[i].Text, this.message[i].FontColor, this.message[i].BackGround);
            }

            form.AppendText("\n");
        }

        private void ParseMessage(string message)
        {
            char[] chars = message.ToCharArray();
            MessagePart msg = new MessagePart();
            int length = chars.Length;

            for(int i = 0; i < length; i++)
            {
                char c = chars[i];
                if(c == 3)
                {
                    if (i+1 < length && EcmaChar.IsDigit((int)chars[i+1]))
                    {
                        i++;
                        string code = chars[i].ToString();
                        if (EcmaChar.IsDigit(chars[i + 1]))
                        {
                            i++;
                            code += chars[i];
                        }

                        this.message.Add(msg);
                        msg = msg.Clone();
                        msg.FontColor = IrcColorPlate.GetColor(code);

                        //now background color
                        if(chars[i+1] == ',')
                        {
                            i++;
                            if (EcmaChar.IsDigit(chars[i + 1]))
                            {
                                i++;
                                code = chars[i].ToString();
                                if (EcmaChar.IsDigit(chars[i + 1]))
                                {
                                    i++;
                                    code += chars[i];
                                }
                                msg.BackGround = IrcColorPlate.GetColor(code);
                            }
                            else
                            {
                                msg.BackGround = Color.White;
                            }
                        }
                    }
                    else
                    {
                        this.message.Add(msg);
                        msg = new MessagePart();
                    }
                }
                else
                {
                    msg.Text += chars[i];
                }
            }

            if (msg.Text.Length > 0)
                this.message.Add(msg);
        }
    }
}

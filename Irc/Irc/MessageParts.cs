﻿using Irc.Database;
using System;
using System.Drawing;

namespace Irc.Irc
{
    class MessageParts
    {
        private TextPart Message;
        public int Lines { get; private set; }

        public MessageParts(string from, string message, Config config)
        {
            DateTime time = DateTime.Now;
            string text = '\x003'.ToString()+config["output.message.time.color.font"] +","+config["output.message.time.color.back"]+"[" + this.ConvertTime(time.Hour) + ":" + this.ConvertTime(time.Minute) + "]"+'\x003'.ToString();
            if(from != null)
            {
                text += " " + from + ": ";
            }

            text += message;
            this.Message = this.InitMessage(text.ToCharArray());
        }

        public void Draw(Graphics g, TextSize size, int width)
        {
            this.Message.Draw(g, size, width);
        }

        public void OnResize(int nw, Graphics g)
        {
            this.Lines = this.Message.CalculateLine(nw, g)[1];
        }

        private TextPart InitMessage(char[] text)
        {
            return InitMessage(text, 0, TextPart.Clean());
        }

        private TextPart InitMessage(char[] text, int i, TextPart part)
        {
            if (text[i] == '\x003')
            {
                //color codes detected here
                if (i+1 < text.Length && text[i + 1] >= '0' && text[i + 1] <= '9')
                {
                    string colorcode = text[i + 1].ToString();
                    if (text[i + 2] >= '0' && text[i + 2] <= '9')
                    {
                        colorcode += text[i + 2].ToString();
                        i++;
                    }
                    i++;
                    i++;
                    part.Color = IrcColorPlate.GetColor(colorcode);

                    if (text[i] == ',' && text[i + 1] >= '0' && text[i + 1] <= '9')
                    {
                        string backcode = text[i + 1].ToString();
                        if (text[i + 2] >= '0' && text[i + 2] <= '9')
                        {
                            backcode += text[i + 2].ToString();
                            i++;
                        }
                        i++;
                        i++;
                        part.Background = IrcColorPlate.GetColor(backcode);
                    }
                }
                else
                {
                    i++;
                    TextPart.Reset(part);//reset the colors to standart. White background and Black text color
                }
            }

            //be sure we can append text to this part of message
            part.Text = "";

            for (; i < text.Length; i++)
            {
                if (text[i] == '\x003')
                {
                    return this.InitMessage(text, i, part.Clone());
                }

                part.Text += text[i];
            }

            return part;
        }

        private string ConvertTime(int time)
        {
            if (time < 10)
                return "0" + time;
            return time.ToString();
        }
    }
}

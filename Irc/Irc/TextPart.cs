using System;
using System.Drawing;

namespace Irc.Irc
{
    internal class TextPart
    {
        internal static TextPart Clean()
        {
            TextPart textpart = new TextPart();
            textpart.Background = Color.White;
            textpart.Color = Color.Black;
            return textpart;
        }

        internal static void Reset(TextPart tp)
        {
            tp.Background = Color.White;
            tp.Color = Color.Black;
        }

        public Color Background { get; set; }
        public Color Color { get; set; }

        public string Text { get; set; }
        public TextPart Last { get; set; }
        public Font Font = new Font("Arial", 8);

        public TextPart Clone()
        {
            TextPart tp = new TextPart();
            tp.Background = this.Background;
            tp.Color = this.Color;
            tp.Last = this;
            return tp;
        }

        public float Draw(Graphics g, TextSize size, int width)
        {
            float x = 0;

            if(this.Last != null)
            {
                x = this.Last.Draw(g, size, width);
            }
            SizeF s = new SizeF();
            for(int i = 0; i < this.Text.Length; i++)
            {
                s = g.MeasureString(this.Text[i].ToString(), this.Font);
                if(x + s.Width > width)
                {
                    x = 0;
                    size.NextLine();
                }
                g.FillRectangle(new SolidBrush(this.Background), x, size.GetY(), s.Width, s.Height);
                g.DrawString(this.Text[i].ToString(), this.Font, new SolidBrush(this.Color), x, size.GetY());
                x += s.Width;
            }

            return x + s.Width;
            /*
            SizeF s = g.MeasureString(this.Text, this.Font);
            if (s.Width + x <= width)
            {
                if (this.Background != Color.FromArgb(255, 255, 255))
                    g.FillRectangle(new SolidBrush(this.Background), x, size.GetY(), s.Width, s.Height);
                g.DrawString(this.Text, this.Font, new SolidBrush(this.Color), x, size.GetY());
            }
            else
            {
                int i = 0;
                string text = "";
                for (; i < this.Text.Length; i++)
                {
                    SizeF n = g.MeasureString(text + this.Text[i], this.Font);
                    if (n.Width + x > width)
                    {
                        if (this.Background != Color.FromArgb(255, 255, 255))
                            g.FillRectangle(new SolidBrush(this.Background), x, size.GetY(), s.Width, s.Height);
                        g.DrawString(text, this.Font, new SolidBrush(this.Color), x, size.GetY());
                        text = "";
                        size.NextLine();
                        x = 0;
                    }
                    s = n;
                }
                if(i < this.Text.Length)
                {
                    if (this.Background != Color.FromArgb(255, 255, 255))
                        g.FillRectangle(new SolidBrush(this.Background), x, size.GetY(), s.Width, s.Height);
                    g.DrawString(text, this.Font, new SolidBrush(this.Color), x, size.GetY());
                }
            }
            return s.Width + x;
            */
        }
    }
}
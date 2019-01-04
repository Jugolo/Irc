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

        internal int[] CalculateLine(Graphics g)
        {
            int[] lines = new int[] { 0, 1 };
            if(this.Last != null)
            {
                lines = this.Last.CalculateLine(g);
            }

            SizeF size = new SizeF();
            for(int i = 0; i < this.Text.Length; i++)
            {
                size = g.MeasureString(this.Text[i].ToString(), this.Font);
                lines[0] += (int)size.Width;
            }

            lines[0] += (int)size.Width;
            return lines;
        }

        public float Draw(Graphics g, TextSize size)
        {
            float x = 0;

            if(this.Last != null)
            {
                x = this.Last.Draw(g, size);
            }
            SizeF s = new SizeF();
            for(int i = 0; i < this.Text.Length; i++)
            {
                s = g.MeasureString(this.Text[i].ToString(), this.Font);
                g.FillRectangle(new SolidBrush(this.Background), x, size.GetY(), s.Width, s.Height);
                g.DrawString(this.Text[i].ToString(), this.Font, new SolidBrush(this.Color), x, size.GetY());
                x += s.Width-2;
            }

            return x + s.Width;
        }
    }
}
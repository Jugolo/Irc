using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc.Forms
{
    public class InvokeSafeRichTextBox : RichTextBox
    {
        private delegate void BW(string str);
        private delegate void CT(string str, Color font, Color back);

        public new void AppendText(string text)
        {
            this.BeginInvoke(new BW(BeginWrite), new object[] { text });
        }

        public void AppendColoredText(string text, Color font, Color back)
        {
            this.BeginInvoke(new CT(BeginColor), new object[] {
                text, 
                font,
                back
            });
        }

        private void BeginColor(string text, Color font, Color back)
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionColor = font;
            this.SelectionBackColor = back;
            base.AppendText(text);
            this.SelectionColor = this.ForeColor;
        }

        private void BeginWrite(string str)
        {
            base.AppendText(str);
        }
    }
}

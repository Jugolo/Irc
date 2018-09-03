using System;
using System.Drawing;
using System.Windows.Forms;

namespace Irc.Irc
{
    internal class IrcColorPlate
    {
        internal static Color GetColor(string v)
        {
            switch (v)
            {
                case "0":
                case "00":
                    return Color.FromArgb(255, 255, 255);
                case "1":
                case "01":
                    return Color.FromArgb(0, 0, 0);
                case "2":
                case "02":
                    return Color.FromArgb(0, 0, 127);
                case "3":
                case "03":
                    return Color.FromArgb(0, 147, 0);
                case "4":
                case "04":
                    return Color.FromArgb(255, 0, 0);
                case "5":
                case "05":
                    return Color.FromArgb(127, 0, 0);
                case "6":
                case "06":
                    return Color.FromArgb(156, 0, 156);
                case "7":
                case "07":
                    return Color.FromArgb(252, 127, 0);
                case "8":
                case "08":
                    return Color.FromArgb(255, 255, 0);
                case "9":
                case "09":
                    return Color.FromArgb(0, 252, 0);
                case "10":
                    return Color.FromArgb(0, 147, 147);
                case "11":
                    return Color.FromArgb(0, 255, 255);
                case "12":
                    return Color.FromArgb(0, 0, 252);
                case "13":
                    return Color.FromArgb(255, 0, 255);
                case "14":
                    return Color.FromArgb(127, 127, 127);
                case "15":
                    return Color.FromArgb(210, 210, 210);
                default:
                    MessageBox.Show("Unknown color number: " + v);
                    return Color.White;
            }
        }
    }
}
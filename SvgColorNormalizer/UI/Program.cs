using System;
using System.Windows.Forms;
using SvgColorNormalizer.UI;

namespace SvgColorNormalizer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Radio101
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool portableMode  = (argv.Length == 1 && argv[0] == "--portable");
            Form1 f = new Form1(portableMode);
            Application.Run(f);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.AI;

namespace WASender
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args != null && args.Any(a => string.Equals(a, "--marketing-bot", StringComparison.OrdinalIgnoreCase)))
            {
                Application.Run(new MarketingBotStudio());
                return;
            }

            Application.Run(new WaSenderForm(args));

            //Application.Run(new InfoWindow());
        }
    }
}

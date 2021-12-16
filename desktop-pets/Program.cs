using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows;

namespace desktop_pets
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new PetDisplay(SaveSystem.LoadTobyTheDog()));
            Application.Run(new PetDisplay(SaveSystem.LoadRiiTheCat()));
            //Application.Run(new ManagementDisplay());
        }

/*        public void GetProcesses() {
            Process[] plist = Process.GetProcesses();
            plist[0].current
        }*/
    }
}

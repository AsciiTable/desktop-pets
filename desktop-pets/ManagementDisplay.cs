using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace desktop_pets
{
    public partial class ManagementDisplay : Form
    {
        #region Icon Fields
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private MenuItem menuItemExit;
        private MenuItem menuItemManagement;
        #endregion


        public ManagementDisplay()
        {
            InitializeComponent();
        }

        private void ManagementDisplayForm_Load(object sender, EventArgs e) {
            #region System Tray Components
            components = new Container();
            contextMenu = new ContextMenu();
            menuItemExit = new MenuItem();
            menuItemManagement = new MenuItem();
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItemExit, menuItemManagement }); // Make new context menu with listed items
            // Menu Item Exit
            menuItemExit.Index = 1;
            menuItemExit.Text = "Exit";
            menuItemExit.Click += new EventHandler(this.menuItemExit_Click); // If clicked, exit
            // Menu Item Management
            menuItemManagement.Index = 0;
            menuItemManagement.Text = "Management";
            menuItemManagement.Click += new EventHandler(this.menuItemManagement_Click);
            // Icon
            notifyIcon = new NotifyIcon(this.components);   // Initialize NotifyIcon object
            notifyIcon.Icon = new Icon("Art/icon.ico");     // Sets the icon that appears in the systray
            notifyIcon.ContextMenu = contextMenu;           // Context menu will open when right clicked
            notifyIcon.Text = "Desktop Pets";
            notifyIcon.Visible = true;
            #endregion

            #region Window properties
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            // make the exit minimize to tray
            #endregion
        }

        private void menuItemExit_Click(object Sender, EventArgs e) {
            this.Close();
        }

        private void menuItemManagement_Click(object Sender, EventArgs e) {
            Console.WriteLine("Management menu opened.");
        }
    }
}

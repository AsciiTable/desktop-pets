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

        private List<Pet> allPets;

        private const int NUMOFROWS = 8;
        private const int NUMOFCOLUMNSPERROW = 5;
        


        public ManagementDisplay()
        {
            allPets = new List<Pet>();
            allPets.Add(SaveSystem.LoadRiiTheCat());
            
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

            LoadStandardForm();
            

        }

        private void menuItemExit_Click(object Sender, EventArgs e) {
            this.Close();
        }

        private void menuItemManagement_Click(object Sender, EventArgs e) {
            Console.WriteLine("Management menu opened.");
        }

        private IEnumerable<Control> GetAllControlsOfType(Control cont, Type t) {
            var controls = cont.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControlsOfType(ctrl, t)).Concat(controls).Where(c => c.GetType() == t);
        }

        private void LoadStandardForm() {

            TableLayoutPanel tlp = (TableLayoutPanel)GetAllControlsOfType(this, typeof(TableLayoutPanel)).First();

            for (int i = 0; i < NUMOFROWS; i++)
            {
                for (int j = 0; j < NUMOFCOLUMNSPERROW; j++)
                    Console.WriteLine("Control name: " + tlp.GetControlFromPosition(j, i).Name);                    // The row and column params are flipped, be careful!

                if (i == 0) { // Load Rii the Cat information
                    Console.WriteLine("Rii the cat's row");
                    PictureBox pb = (PictureBox)tlp.GetControlFromPosition(0, i);
                    pb.Image = new Bitmap("Art/cat/idle-trans.png");
                    tlp.GetControlFromPosition(1, i).Text = "Rii";
                }
                else if (i == 1) { // Load Toby the Dog information 
                    Console.WriteLine("Toby the dog's row");
                    tlp.GetControlFromPosition(1, i).Text = "Toby";
                }
                if (i == 0 || i == 1) { // If it is either Rii or Toby's row, enable all remaining controls
                    tlp.GetControlFromPosition(2, i).Enabled = false;                                           // Eventually change this out to lead to a different functionality
                    tlp.GetControlFromPosition(3, i).Enabled = true;
                    tlp.GetControlFromPosition(4, i).Enabled = true;
                }
                else {
                    Console.WriteLine("Everyone else's rows");
                    tlp.GetControlFromPosition(1, i).Text = "";
                    tlp.GetControlFromPosition(2, i).Enabled = false;                                           // Eventually change this out to lead to a different functionality
                    tlp.GetControlFromPosition(3, i).Enabled = false;
                    tlp.GetControlFromPosition(4, i).Enabled = false;
                }
            }
        }
    }
}

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
    public partial class Pet : Form
    {
        private int _DisplayHeight = SystemInformation.WorkingArea.Height;
        private int _DisplayWidth = SystemInformation.WorkingArea.Width;
        private Bitmap layer1 = new Bitmap("Art/Cat/idle.png");
        private Bitmap layer2 = new Bitmap("Art/Cat/walk.png");
        #region Icon Fields
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private MenuItem menuItemExit;
        private IContainer componenets;
        #endregion
        // Art Fields
        // Walking
        private Bitmap walk_sheet = new Bitmap("Art/Cat/walk_anim.png");
        private List<Bitmap> walk = new List<Bitmap>();
        private int walkInd = 0;
        private bool walkVarient = false;


        Timer timer = new Timer();
        private int count = 0;

        private int XSIZE = 64;
        private int YSIZE = 64;
        private int SINKLEVEL = 3;

        private int HeadPoint = 0;

        // Mode bools
        private bool isDragging = false;

        public Pet()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            #region System Tray Components
            components = new Container();
            contextMenu = new ContextMenu();
            menuItemExit = new MenuItem();
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItemExit }); // Make new context menu with listed items
            // Menu Item Exit
            menuItemExit.Index = 0;
            menuItemExit.Text = "Exit";
            menuItemExit.Click += new EventHandler(this.menuItemExit_Click); // If clicked, exit
            // Icon
            notifyIcon = new NotifyIcon(this.components);   // Initialize NotifyIcon object
            notifyIcon.Icon = new Icon("Art/icon.ico");     // Sets the icon that appears in the systray
            notifyIcon.ContextMenu = contextMenu;           // Context menu will open when right clicked
            notifyIcon.Text = "Desktop Pets";
            notifyIcon.Visible = true;
            #endregion

            // Load in Spritesheets
            // Walk sheet
            int col = walk_sheet.Width / XSIZE;
            for (int i = 0; i < col; i++) {
                Rectangle cloneRect = new Rectangle(XSIZE * i, 0, XSIZE, YSIZE);
                walk.Add(walk_sheet.Clone(cloneRect, walk_sheet.PixelFormat));
            }
            
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(XSIZE, YSIZE);
            Color transColor = layer1.GetPixel(0, 0);
            layer1.MakeTransparent(transColor);
            this.BackgroundImage = layer1;
            this.BackColor = transColor;
            this.TransparencyKey = transColor;
            HeadPoint =_DisplayHeight - YSIZE + SINKLEVEL;
            this.Location = new Point(0, HeadPoint);
            timer.Interval = 1;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e) {
            if (this.Location.Y >= HeadPoint)
            {
                if (!isDragging)
                {
                    /*if (is_fly)
                    {
                        is_fly = false;
                        fc1 = 0;
                        fc2 = 0;
                    }*/
                    moveright();
                }
                else
                {
                   //dragmode();
                }
            }
            else
            {
                if (!isDragging)
                {
                    //flymode();
                }
                else
                {
                    //dragmode();
                }
            }
        }
        
        private void moveright() {
            this.Left += 1;
            if (this.Left > _DisplayWidth) {
                this.Left = XSIZE *-1;
                count = 0;
                // Randomly swap to blinking & walking
            }
            if(!walkVarient)
                GoThroughFrames(ref walk, ref walkInd, 10);
            // else go through blink walk varient
            
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mousedown(object sender, MouseEventArgs e)
        {
            isDragging = true;
        }

        private void mouseup(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }
        // use ref keyword to pass by reference
        private void GoThroughFrames(ref List<Bitmap> bm, ref int index, int fps)
        {
            count++;
            if (count % fps == 0) {
                if (index >= bm.Count)
                    index = 0;
                this.BackgroundImage = bm[index];
                index++;
                count = 0;
            }   
        }
        private void menuItemExit_Click(object Sender, EventArgs e) {
            this.Close();
        }
    }
}

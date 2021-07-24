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
    public partial class PetDisplay : Form
    {
        private int _DisplayHeight = SystemInformation.WorkingArea.Height;
        private int _DisplayWidth = SystemInformation.WorkingArea.Width;
        private Bitmap defaultLayer = new Bitmap("Art/Cat/idle.png");
        #region Icon Fields
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private MenuItem menuItemExit;
        #endregion
        #region Art Fields (Should be Deprecated)
        #region Idle
        //private Animation idle_v0 = new Animation(new Bitmap("Art/cat/idle_blink"), 64, 64, 10);
        //private State(Pet.States.Idle,)
        #endregion
        #region Walking
        private Bitmap walk_v0 = new Bitmap("Art/Cat/walk_anim.png");                       // Default walking spritesheet
        private Bitmap walk_v1 = new Bitmap("Art/Cat/walk_anim_v1.png");                    // Blinking variation walking spritesheet
        private Dictionary<int, List<Bitmap>> walk = new Dictionary<int, List<Bitmap>>();   // Dictionary to store Bitmaps and their indexes. 0 is default, > 0 is variation
        private int walkInd = 0;                                                            // Keeps track of the frame that the pet is currently walking at
        private int walkVarient = 0;                                                        // Determines which walk variation to play
        private bool walkComplete = false;
        #endregion
        #endregion

        private Timer timer = new Timer();
        private DateTime fpsTimer = new DateTime();
        private int count = 0;
        private Random rand = new Random();
        private int XSIZE = 64;
        private int YSIZE = 64;
        private int SINKLEVEL = 3;

        private int HeadPoint = 0;
        private Pet displayedPet;


        // Mode bools
        private bool isMoving = false;
        private bool isDragging = false;

        public PetDisplay()
        {
            displayedPet = SaveSystem.LoadRiiTheCat();
            InitializeComponent();
        }

        public PetDisplay(Pet p) {
            displayedPet = p;
            InitializeComponent();
        }

        private void PetDisplayForm_Load(object sender, EventArgs e) {
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

            /*            #region Load in Spritesheets
                        LoadInSpritesheet(ref walk, ref walk_v0, 0);    // Walk v0
                        LoadInSpritesheet(ref walk, ref walk_v1, 1);    // Walk v1
                        #endregion*/

            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(XSIZE, YSIZE);

            #region Background of Window
            Color transColor = defaultLayer.GetPixel(0, 0);
            defaultLayer.MakeTransparent(transColor);
            this.BackgroundImage = defaultLayer;
            this.BackColor = transColor;
            this.TransparencyKey = transColor;
            #endregion

            HeadPoint = _DisplayHeight - YSIZE + SINKLEVEL;
            this.Location = new Point(0, HeadPoint);
            timer.Interval = 1;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e) {
            if (this.Location.Y >= HeadPoint) {                     // If the window is detected to have moved up from/is equal to the original Y position
                if (!isDragging) {                                  // If the user is not actively dragging the window
                    if (displayedPet.activeState.state == Pet.States.Drag)
                    {
                        SwitchStates(Pet.States.Idle);
                    }
                    SwitchStates(displayedPet.activeState.state);   // Continue normal/auto behavior
                }
                else
                {                                              // Else if the user IS actively dragging the window
                    SwitchStates(Pet.States.Drag);
                }

            }
            else {
                if (isDragging) {
                    SwitchStates(Pet.States.Drag);
                }
            }
            
        }

        #region Drag reference
        /*private void Timer_Tick(object sender, EventArgs e) {
            if (this.Location.Y >= HeadPoint)
            {
                if (!isDragging)
                {
                    *//*if (is_fly)
                    {
                        is_fly = false;
                        fc1 = 0;
                        fc2 = 0;
                    }*//*
                    //moverightDEP();
                    SwitchStates(displayedPet.activeState.state);
                    //SwitchStates(Pet.States.Walk);
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
        }*/
        #endregion

        #region Move reference (Deprecated)
        private void moverightDEP() {
            this.Left += 1;
            if (this.Left > _DisplayWidth) {                                // If the pet has gone off the side of the screen...
                this.Left = XSIZE *-1;                                      // Set it back to the left of the screen
            }
            // Randomly swap to blinking & walking
            if (walkComplete)
            {
                walkVarient = rand.Next(0, walk.Count);
                walkComplete = false;
            }
            GoThroughFrames(walk[walkVarient], ref walkInd, 10);
            // else go through blink walk varient
        }
        #endregion

        private void SwitchStates(Pet.States selectedState = Pet.States.Idle) {

            if (!displayedPet.activeState.stateComplete && displayedPet.activeState.state != selectedState)
            {
                if (displayedPet.currentAnimation != null) {
                    displayedPet.currentAnimation.ResetAnimation();
                    displayedPet.currentAnimation = null;
                }
                displayedPet.ImmediatelyChangeToThisState(selectedState);
                displayedPet.currentAnimation = displayedPet.dictionaryOfStates[selectedState].GetAnimationToPlay();
                fpsTimer = DateTime.Now;
                GoThroughAnimFrames(displayedPet.currentAnimation);
            }
            else if (!displayedPet.activeState.stateComplete)
            {
                if (displayedPet.currentAnimation == null)
                {
                    displayedPet.currentAnimation = displayedPet.dictionaryOfStates[selectedState].GetAnimationToPlay();
                    fpsTimer = DateTime.Now;
                    GoThroughAnimFrames(displayedPet.currentAnimation);
                }
                else if (displayedPet.currentAnimation.complete)
                {
                    displayedPet.currentAnimation.ResetAnimation();
                    displayedPet.currentAnimation = null;
                }
                else
                {
                    GoThroughAnimFrames(displayedPet.currentAnimation);
                }
            }
            else
            {
                displayedPet.activeState.ResetState();
                displayedPet.AutoPickNextState();
            }
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
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

        #region Non-FPS based frame iterater (Deprecated)
        // use ref keyword to pass by reference
        private void GoThroughFrames(List<Bitmap> bm, ref int index, int fps){
            count++;
            if (count % fps == 0) {                     // This needs to eventually be scaled off of time, not frame count to allow for uniformity
                this.BackgroundImage = bm[index];
                index++;
                count = 0;                              // Resets the frames count so that we don't eventually overflow the value
                if (index >= bm.Count){                 // If we finish cycling through the spritesheet...
                    index = 0;                          // Reset the frame index to 0
                    walkComplete = true;                // Tell everyone else that the walk cycle has been completed
                }
            }   
        }
        #endregion

        private void GoThroughAnimFrames(Animation anim) {                        // FPS-based animation
            if ((DateTime.Now - fpsTimer).TotalSeconds >= anim.fpsSecondInterval) // When the animation's time interval between each frame is reached...
            {
                if (displayedPet.activeState.state.Equals(Pet.States.Walk) && !isMoving)
                    isMoving = true;
                else if (!displayedPet.activeState.state.Equals(Pet.States.Walk) && isMoving)
                    isMoving = false;

                this.BackgroundImage = anim.GetNextFrame();                       // Change the frame
                fpsTimer = DateTime.Now;                                          // Reset the timer for the next animation
                if (anim.complete) {
                    displayedPet.activeState.IncrementLoop();
                }
                    
            }
            if (isMoving) {
                RandomWalk();
            }
            if (isDragging) {
                Drag();
            }
        }

        private void RandomWalk() {
            int rightleft = rand.Next(0, 2);
            this.Left += 1;
            if (this.Left > _DisplayWidth) {                                 // If the pet has gone off the side of the screen...
                this.Left = XSIZE * -1;                                      // Set it back to the left of the screen
            }
        }

        private void Drag() {
            DesktopLocation = Cursor.Position;
        }
/*        private void LoadInSpritesheet(ref Dictionary<int, List<Bitmap>> bm, ref Bitmap sheet, int key)
        {
            List<Bitmap> store = new List<Bitmap>();
            int col = sheet.Width / XSIZE;
            int row = sheet.Height / YSIZE;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    // Add checkers to look for content just in case there is a non-full row at the end
                    Rectangle cloneRect = new Rectangle(XSIZE * c, YSIZE * r, XSIZE, YSIZE);
                    store.Add(sheet.Clone(cloneRect, sheet.PixelFormat));
                }
            }
            bm.Add(key, store);
        }*/
        private void menuItemExit_Click(object Sender, EventArgs e) {
            this.Close();
        }
    }
}

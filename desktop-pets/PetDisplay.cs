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
using System.Media;

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
        private DateTime freefallTimer = new DateTime();
        private int count = 0;
        private Random rand = new Random();
        private int XSIZE = 64;
        private int YSIZE = 64;
        private int SINKLEVEL = 3;
        private const float GRAVIATIONALCONSTANT = 9.8f;

        private int HeadPoint = 0;
        private Pet displayedPet;


        // Mode bools
        private bool isMoving = false;
        private bool isDragging = false;
        private bool isFreeFalling = false;
        private bool isWantingAttention = false;
        private bool isSatisfied = false;

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

            fpsTimer = DateTime.Now;

            DoubleBuffered = true;
        }


        private void Timer_Tick(object sender, EventArgs e) {
            if (isDragging) {                                                       // If pet is being dragged
                PlayState(Pet.States.Drag);
                //displayedPet.ImmediatelyChangeToThisState(Pet.States.Drag);
                //this.BackgroundImage = displayedPet.activeState.GetAnimationToPlay().GetFrameAtIndex(0);
                Drag();
            }
            else if (isSatisfied) {
                PlayState(Pet.States.Satisfied);
                isSatisfied = false;
            }
            else if (this.Location.Y >= HeadPoint) {                                // If the window is detected to have moved up from/is equal to the original Y position
                if (!isDragging) {                                                  // If the user is not actively dragging the window
                    if (displayedPet.activeState.state == Pet.States.Drag){         // If the current state is LISTED as Drag
                        DesktopLocation = new Point(DesktopLocation.X, HeadPoint);  // Ensures that the pet cannot be dropped lower than the defined headpoint
                        PlayState(Pet.States.Idle);                                 // Play the state that comes after being dragged
                    }
                    else {
                        PlayState(displayedPet.activeState.state);                  // Continue normal/auto behavior
                    }
                    
                }
                if (isFreeFalling)
                    isFreeFalling = false;
            }
            else {                                                          // Pet is in free fall
                if (!isFreeFalling) {                                       // If free falling is first detected...
                    isFreeFalling = true;                                   // Mark it as true
                    freefallTimer = DateTime.Now;                           // Save the time in which the free fall started to calcuate the increase of velocity over time
                }
                PlayState(Pet.States.Fall);
                FreeFall();
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
                    PlayState(displayedPet.activeState.state);
                    //PlayState(Pet.States.Walk);
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

        private void PlayState(Pet.States selectedState = Pet.States.Null) {
            if (isDragging) {                                                                                           // IF the system automatically detects dragging...
                if (displayedPet.activeState == null || displayedPet.activeState.state != Pet.States.Drag) {            // If the current state is NOT Pet.States.Drag...
                    displayedPet.ImmediatelyChangeToThisState(Pet.States.Drag);                                         // Change the state to Drag
                    if (displayedPet.currentAnimation != null)                                                          // And if the current animation isn't null (safe-guarding against glitches)
                        displayedPet.currentAnimation.ResetAnimation();                                                 // Reset the current animation for future reuse
                    SetNewAnimActive(true);
                }
                else {
                    displayedPet.activeState.ResetState();
                    GoThroughAnimFrames(displayedPet.currentAnimation);                                                 // Play the drag animation every frame
                }    
            }
            else {                                                                                                      // Else if the system DOES NOT detect dragging...
                if (displayedPet.activeState == null && displayedPet.dictionaryOfStates.Count > 0) {                    // If there is no assigned state and states exist for the pet...
                    if (displayedPet.currentAnimation != null)                                                          // If (somehow) the current animation is not null
                        displayedPet.currentAnimation.ResetAnimation();                                                 // Reset the current animation
                    displayedPet.AutoPickNextState();                                                                   // Auto pick a state to be in
                    SetNewAnimActive(true); 
                }
                else if (displayedPet.activeState.state != selectedState) {                                             // Else if the current state does not match the requested state...
                    displayedPet.ImmediatelyChangeToThisState(selectedState);                                           // Change the state to requested state
                    if (displayedPet.currentAnimation != null)                                                          // And if the current animation isn't null (safe-guarding against glitches)
                        displayedPet.currentAnimation.ResetAnimation();                                                 // Reset the current animation for future reuse
                    SetNewAnimActive(true);
                }
                else if (displayedPet.activeState.stateComplete) {                                                      // If the state is determined to be complete after the last animation loop...
                    displayedPet.currentAnimation.ResetAnimation();                                                     // Reset the animation that was just running (just in case it didn't reset before)
                    displayedPet.activeState.ResetState();                                                              // Reset the entire state so that it can fully run again
                    displayedPet.AutoPickNextState();                                                                   // Auto pick the next state that should trigger after this
                    SetNewAnimActive(true);                                                                             // Set and activate the new animation for the new state
                }
                else {                                                                                                  // Else if the state is not complete...
                    if (displayedPet.currentAnimation == null) {                                                        // If the animation is (somehow) null
                        SetNewAnimActive(true);                                                                         // Pick and play a new animation
                    }
                    else if (displayedPet.currentAnimation.complete) {                                                  // Else if the current animation is marked as complete
                        displayedPet.currentAnimation.ResetAnimation();                                                 // Reset the animation
                        SetNewAnimActive(false);                                                                        // And auto pick a new animation from the same state to follow & play
                    }
                    else {                                                                                              // Else if NEITHER the animation nor state are complete...
                        GoThroughAnimFrames(displayedPet.currentAnimation);                                             // Iterate through the animation frames as per usual
                    }
                }
            }
        }

        private void SetNewAnimActive(bool activeNow) {                                       // Gets the animation of the new current state and has it play immediately
            displayedPet.currentAnimation = displayedPet.activeState.GetAnimationToPlay();    // Sets the current animation to a random varient within the active state
            GoThroughAnimFrames(displayedPet.currentAnimation, activeNow);                    // Starts the animation run through either immediately (true) or after the FPS timer is met (false)
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void mousedown(object sender, MouseEventArgs e)
        {
            if (isWantingAttention){
                isSatisfied = true;
            }
            else {
                isDragging = true;
            } 
            if (isFreeFalling)
                isFreeFalling = false;
        }

        private void mouseup(object sender, MouseEventArgs e)
        {
            if (isWantingAttention && isSatisfied) {
                isWantingAttention = false;
            }
            else if(isDragging)
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

        private void GoThroughAnimFrames(Animation anim, bool switchNow = false) {              // FPS-based animation
            if (displayedPet.activeState.state.Equals(Pet.States.Walk) && !isMoving)
                isMoving = true;
            else if (!displayedPet.activeState.state.Equals(Pet.States.Walk) && isMoving)
                isMoving = false;
            else if (displayedPet.activeState.state.Equals(Pet.States.Attention) && !isWantingAttention)
                isWantingAttention = true;
            else if (!displayedPet.activeState.state.Equals(Pet.States.Attention) && isWantingAttention)
                isWantingAttention = false;
                

            if ((DateTime.Now - fpsTimer).TotalSeconds >= anim.fpsSecondInterval || switchNow)  // When the animation's time interval between each frame is reached...
            {
                this.BackgroundImage = anim.GetNextFrame();                                     // Change the frame
                if (anim.complete) {                                                                // If the animation is marked as complete after it incremented this frame
                    displayedPet.activeState.IncrementLoop();                                       // Increment the number of animations played for the state
                    displayedPet.currentAnimation.ResetAnimation();                                 // Reset this animation back to its initial frame and complete state
                    displayedPet.currentAnimation = displayedPet.activeState.GetAnimationToPlay();
                }
                fpsTimer = DateTime.Now;                                                        // Reset the timer for the next animation    
            }
            if (isMoving) {
                RandomWalk();
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
            DesktopLocation = new Point(Cursor.Position.X - XSIZE/2, Cursor.Position.Y - YSIZE/3);
        }

        private void FreeFall() {
            int freefallvelocity = (int)(GRAVIATIONALCONSTANT * (DateTime.Now - freefallTimer).TotalSeconds); // velocity = graviationalConstant * timeSpentDropping
            if (this.Top + freefallvelocity >= HeadPoint) {
                isFreeFalling = false;
                DesktopLocation = new Point(DesktopLocation.X, HeadPoint);  // Ensures that the pet cannot be dropped lower than the defined headpoint
                PlayState(Pet.States.Idle);                                 // Play the state that comes after landing
            }   
            else 
                this.Top += freefallvelocity;          
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

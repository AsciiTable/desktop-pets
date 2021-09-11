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
        private Bitmap defaultLayer = new Bitmap("Art/Cat/idle_v0.png");
/*        #region Icon Fields
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private MenuItem menuItemExit;
        private MenuItem menuItemManagement;
        #endregion*/

        private Timer timer = new Timer();
        private DateTime fpsTimer = new DateTime();
        private DateTime freefallTimer = new DateTime();
        private int count = 0;
        private Random rand = new Random();
        private int XSIZE = 0;                                            
        private int YSIZE = 0;
        private int SINKLEVEL = 3;
        private const float GRAVIATIONALCONSTANT = 9.8f;

        private int HeadPoint = 0;
        private Pet displayedPet;
        private int leftright = 0;

        // Mode bools
        private bool isMoving = false;
        private bool isDragging = false;
        private bool isFreeFalling = false;
        private bool isWantingAttention = false;
        private bool isSatisfied = false;
        private Pet.States previousState = Pet.States.Null;

        public PetDisplay()
        {
            displayedPet = SaveSystem.LoadRiiTheCat();
            InitializeComponent();
        }

        public PetDisplay(Pet p) {
            displayedPet = p;
            XSIZE = p.XSIZE;
            YSIZE = p.YSIZE;
            defaultLayer.MakeTransparent(p.transparentColor);
            this.BackColor = p.transparentColor;
            this.TransparencyKey = p.transparentColor;
            this.Size = new Size(XSIZE, YSIZE);
            InitializeComponent();
        }

        private void PetDisplayForm_Load(object sender, EventArgs e) {
/*            #region System Tray Components
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
            #endregion*/

            /*            #region Load in Spritesheets
                        LoadInSpritesheet(ref walk, ref walk_v0, 0);    // Walk v0
                        LoadInSpritesheet(ref walk, ref walk_v1, 1);    // Walk v1
                        #endregion*/

            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(XSIZE, YSIZE);

            #region Background of Window
            //Color transColor = defaultLayer.GetPixel(0, 0);
            //defaultLayer.MakeTransparent(transColor);
            this.BackgroundImage = defaultLayer;
            //this.BackColor = transColor;
            //this.TransparencyKey = transColor;
            #endregion

            HeadPoint = _DisplayHeight - YSIZE + SINKLEVEL;
            this.Location = new Point(0, HeadPoint);

            DoubleBuffered = true;

            if (displayedPet.activeState != null) {
                switch (displayedPet.activeState.state) {
                    case Pet.States.Walk:
                        isMoving = true;
                        break;
                    case Pet.States.Drag:
                        isDragging = true;
                        break;
                    case Pet.States.Fall:
                        isFreeFalling = true;
                        break;
                    case Pet.States.Attention:
                        isWantingAttention = true;
                        break;
                    case Pet.States.Satisfied:
                        isSatisfied = true;
                        break;
                    default:
                        break;
                }
            }

            timer.Interval = 10;
            timer.Tick += new EventHandler(OnUpdate);
            timer.Start();

            fpsTimer = DateTime.Now;
        }


        private void OnUpdate(object sender, EventArgs e) { 
            if (isDragging) {                                                       // If pet is being dragged
                PlayState(Pet.States.Drag);                                         // Play the drag state
                Drag();                                                             // Function that allows the pet to be dragged around the screen
            }
            else if (isSatisfied) {                                                 // If the pet is satisfied (occurs after they are pet when asking for attention)
                PlayState(Pet.States.Satisfied);                                    // Play the satisfied state
                isSatisfied = false;                                                // Set satisfied to false once the pet is satisfied
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
                if (isFreeFalling)                                                  // If free falling is still true dispite having finished falling
                    isFreeFalling = false;                                          // Set the free fall to false
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

        private void PlayState(Pet.States selectedState = Pet.States.Null) {
            if (isDragging) {                                                                                           // IF the system automatically detects dragging...
                if (displayedPet.activeState == null || displayedPet.activeState.state != Pet.States.Drag) {            // If the current state is NOT Pet.States.Drag...
                    displayedPet.ImmediatelyChangeToThisState(Pet.States.Drag);                                         // Change the state to Drag
                    previousState = displayedPet.activeState.state;
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
                    previousState = displayedPet.activeState.state;
                    if (displayedPet.currentAnimation != null)                                                          // And if the current animation isn't null (safe-guarding against glitches)
                        displayedPet.currentAnimation.ResetAnimation();                                                 // Reset the current animation for future reuse
                    SetNewAnimActive(true);
                }
                else if (displayedPet.activeState.stateComplete) {                                                      // If the state is determined to be complete after the last animation loop...
                    previousState = displayedPet.activeState.state;
                    displayedPet.currentAnimation.ResetAnimation();                                                     // Reset the animation that was just running (just in case it didn't reset before)
                    displayedPet.activeState.ResetState();                                                              // Reset the entire state so that it can fully run again
                    displayedPet.AutoPickNextState();                                                                   // Auto pick the next state that should trigger after this
                    if (previousState.Equals(displayedPet.activeState.state))
                        SetNewAnimActive(false);                                                                        // Set and activate the new animation for the new state
                    else
                        SetNewAnimActive(true);
                }
                else {                                                                                                  // Else if the state is not complete...
                    if (displayedPet.currentAnimation == null) {                                                        // If the animation is (somehow) null
                        SetNewAnimActive(true);                                                                         // Pick and play a new animation
                    }
                    else if (displayedPet.currentAnimation.complete) {                                                  // Else if the current animation is marked as complete
                        displayedPet.currentAnimation.ResetAnimation();                                                 // Reset the animation
                        displayedPet.activeState.IncrementLoop();
                        SetNewAnimActive(false);                                                                         // And auto pick a new animation from the same state to follow & play
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

        private void GoThroughAnimFrames(Animation anim, bool switchNow = false) {              // FPS-based animation
            #region State Management (may move to a more intuative place if need be)
            if (displayedPet.activeState.state.Equals(Pet.States.Walk) && !isMoving) {
                isMoving = true;
                int r = rand.Next(0, 2);
                if (r == 1) {                                                                   // Move Right
                    leftright = 1;
                }
                else {                                                                          // Move Left
                    leftright = -1;
                }
            }
            else if (!displayedPet.activeState.state.Equals(Pet.States.Walk) && isMoving) {
                isMoving = false;
                displayedPet.activeState.FlipAllAnimationsInState(false);
                leftright = 0;
            }  
            if (displayedPet.activeState.state.Equals(Pet.States.Attention) && !isWantingAttention)
                isWantingAttention = true;
            else if (!displayedPet.activeState.state.Equals(Pet.States.Attention) && isWantingAttention)
                isWantingAttention = false;
            #endregion

            if ((DateTime.Now - fpsTimer).TotalSeconds >= anim.fpsSecondInterval || switchNow)              // When the animation's time interval between each frame is reached...
            {
                this.BackgroundImage = anim.GetNextFrame();                                                 // Change the frame
                fpsTimer = DateTime.Now;                                                                    // Reset the timer for the next animation    
            }

            if (isMoving) {
                RandomWalk(leftright);
            }
        }

        private void RandomWalk(int dir = 1) {
            this.Left += dir;                                                // Move right (+1) or left (-1) depending on the direction value

            if(dir >= 0)
                displayedPet.activeState.FlipAllAnimationsInState(false);
            else
                displayedPet.activeState.FlipAllAnimationsInState(true);

            if (this.Left < XSIZE * -1) {                                    // If the pet has gone off the side of the screen...
                this.Left = _DisplayWidth;                                   // Set it back to the right of the screen
            }
            else if (this.Left > _DisplayWidth) {                            // If the pet has gone off the side of the screen...
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
/*        private void menuItemExit_Click(object Sender, EventArgs e) {
            this.Close();
        }

        private void menuItemManagement_Click(object Sender, EventArgs e) {
            Console.WriteLine("Management menu opened.");
        }*/
    }
}

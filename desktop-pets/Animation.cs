using System;
using System.Collections.Generic;
using System.Drawing;

namespace desktop_pets
{
    /** TO DOs (+ = essential, ~ = for custom dlc)
     *  + Play(): play the animation directly through this object, if objective & possible.
     *      Issues: To show each frame, the BackgrounImage (Bitmap) of a Form object must be changed
     *      Temporarily solved with an untested external call to iterate through frames
     *  + Change the durationInFrames (based on number of frames) to FPS (based on time)
     *  +~ Transparancy automation: currently believed to be impossible and must use a certain 
     *      color as a key for the system to know which color specifically to make transparent
     *      when rendering it to the screen.
     *  +~ External setters of adjustable variables; primarly for buttons/fields on the customizable form on update
     *  ~ Row & Column variable constructor as another option opposed to the X & Y pixel count
     */
    [System.Serializable]
    public class Animation
    {
        #region Adjustable Variables
        private Bitmap fullSpritesheet;                 // Spritesheet of the animation
        public int fps { get; private set; }
        public float fpsSecondInterval { get; private set; }
        public int xsize { get; private set; }          // Number of pixels length-wise for each individual frame
        public int ysize { get; private set; }          // Number of pixels height-wise for each individual frame
        public int numOfFrames { get; private set; }    // Total number of frames in an animation
        #endregion

        #region Object Checkers
        private int frameIndex;                     // Keeps track of which frame the active animation is currently on
        public bool isFlippedX;
        public bool complete { get; private set; }  // Signals the completion of an animation cycle
        private List<Bitmap> frames;                // The divided Bitmap that is used for animation cycles
        #endregion

        #region Constructors
        public Animation() {
            fullSpritesheet = null;
            fps = 0;
            xsize = 0;
            ysize = 0;
            numOfFrames = 0;
            frameIndex = 0;
            complete = true;
            frames = new List<Bitmap>();
            fpsSecondInterval = 0f;
            isFlippedX = false;
        }

        public Animation(Bitmap spriteSheet, int numXPixels = 0, int numYPixels = 0, int FPS = 10)
        {
            fullSpritesheet = spriteSheet;
            fps = FPS;
            fpsSecondInterval = (float)1 / (float)fps;
            Console.WriteLine("Time: " + fpsSecondInterval);
            xsize = numXPixels;
            ysize = numYPixels;
            numOfFrames = CalcuateNumberOfFrames();
            frameIndex = 0;
            complete = true;
            frames = new List<Bitmap>();
            if (numOfFrames > 0) 
                frames = LoadInSpritesheet();
            isFlippedX = false;
        }
        #endregion

        #region Internal Functionalities
        private List<Bitmap> LoadInSpritesheet() {
            List<Bitmap> store = new List<Bitmap>();
            int col = fullSpritesheet.Width / xsize;
            int row = fullSpritesheet.Height / ysize;
            int frameIndex = 0;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    frameIndex++;
                    if (frameIndex <= numOfFrames) {    // Makes sure that there are no extra frames loaded in
                        Rectangle cloneRect = new Rectangle(xsize * c, ysize * r, xsize, ysize);
                        store.Add(fullSpritesheet.Clone(cloneRect, fullSpritesheet.PixelFormat));
                    }
                }
            }
            return store;
        }

        private int CalcuateNumberOfFrames() {   // Automates the process of getting the number of frames in a spritesheet
            int col = fullSpritesheet.Width / xsize;
            int row = fullSpritesheet.Height / ysize;
            return col*row;
        }
        #endregion

        #region External Functionalities
        public Bitmap GetNextFrame() {          // An external call to iterate through the frames of an animation
            frameIndex++;
            if (frameIndex > 0 && frameIndex <= frames.Count) {
                if (frameIndex == numOfFrames)  // If the animation as reached its last frame
                    complete = true;            // Flag it as true and reset it externally using ResetAnimation when ready
                return frames[frameIndex - 1];
            }
            else if (frameIndex >= frames.Count) {
                return frames[0];
            }
            return null;
        }

        public Bitmap GetFrameAtIndex(int ind) {
            return frames[ind];
        }

        public void ResetAnimation() {      // An external call to reset the animation to the beginning
            frameIndex = 0;
            complete = false;
            isFlippedX = false;
        }

        public void FlipAnimationX(bool flipUnflip = true) {
            if (!isFlippedX && flipUnflip) {                            // If the animation is not currently flipped and is requested to be flipped
                foreach (Bitmap bm in frames)
                    bm.RotateFlip(RotateFlipType.RotateNoneFlipX);
                isFlippedX = true;
            }
            else if (isFlippedX && !flipUnflip) {                       // If the animation is currently flipped and is requested not to be flipped
                foreach (Bitmap bm in frames)
                    bm.RotateFlip(RotateFlipType.RotateNoneFlipX);
                isFlippedX = false;
            }
        }
        #endregion

        #region External Setters (Customization usage only)
        // fps setter (DON'T FORGET TO ALSO UPDATE THE FPSSECONDINTERVAL WITH THIS SETTER)
        // xsize setter
        // ysize setter
        // numOfFrames setter
        #endregion
    }
}

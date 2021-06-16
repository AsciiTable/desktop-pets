﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace desktop_pets
{
    /** TO DOs
     *  + Play(): play the animation directly through this object, if objective & possible.
     *      Issues: To show each frame, the BackgrounImage (Bitmap) of a Form object must be changed
     *  + Transparancy automation: currently believed to be impossible and must use a certain 
     *      color as a key for the system to know which color specifically to make transparent
     *      when rendering it to the screen.
     *  + External setters of adjustable variables; primarly for buttons/fields on the customizable form
     */
    class Animation
    {
        #region Adjustable Variables
        private Bitmap fullSpritesheet;                 // Spritesheet of the animation
        public int fps { get; private set; }            // Number of frames played per second
        public int xsize { get; private set; }          // Number of pixels length-wise for each individual frame
        public int ysize { get; private set; }          // Number of pixels height-wise for each individual frame
        public int numOfFrames { get; private set; }    // Total number of frames in an animation
        #endregion

        #region Object Checkers
        private int frameIndex;                     // Keeps track of which frame the active animation is currently on
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
        }

        public Animation(Bitmap spriteSheet, int framesPerSecond = 10, int numXPixels = 0, int numYPixels = 0){
            fullSpritesheet = spriteSheet;
            fps = framesPerSecond;
            xsize = numXPixels;
            ysize = numYPixels;
            numOfFrames = CalcuateNumberOfFrames();
            frameIndex = 0;
            complete = true;
            frames = new List<Bitmap>();
            if (numOfFrames > 0) 
                frames = LoadInSpritesheet();
        }

        public Animation(Bitmap spriteSheet, int totalNumOfFrames, int framesPerSecond = 10, int numXPixels = 0, int numYPixels = 0)
        {
            fullSpritesheet = spriteSheet;
            fps = framesPerSecond;
            xsize = numXPixels;
            ysize = numYPixels;
            numOfFrames = totalNumOfFrames;
            frameIndex = 0;
            complete = true;
            frames = new List<Bitmap>();
            if (numOfFrames > 0)
                frames = LoadInSpritesheet();
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
            return null;
        }

        public void ResetAnimation() {      // An external call to reset the animation to the beginning
            frameIndex = 0;
            complete = false;
        }
        #endregion

        #region External Setters (Customization usage only)
        // fps setter
        // xsize setter
        // ysize setter
        // numOfFrames setter
        #endregion
    }
}
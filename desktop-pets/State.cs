﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;


namespace desktop_pets
{
    [System.Serializable]
    public class State
    {
        public Pet.States state { get; private set; }              // Defines which state the pet is currently in
        private Dictionary<int, Animation> dictionaryOfAnimations;  // A dictionary of animations associated with this state                             
        private int animVarient;                                    // Keeps track of which animation it's playing out of the list of animations avalible
        private bool isFlippedX;
        private int minNumLoops;
        public int numOfLoopsPlayed;
        public bool stateComplete { get; private set; }
        private Random rand;
        public Pet.States dependantState { get; private set; }       // A state that must be played in sequence after this state
        public bool canRandomlyTrigger { get; private set; }
        private SoundPlayer sfx;

        public State() {                                            // Empty, default constructor
            state = Pet.States.Null;
            dictionaryOfAnimations = new Dictionary<int, Animation>();
            animVarient = 0;
            rand = new Random();
            dependantState = Pet.States.Null;
            minNumLoops = 0;
            stateComplete = false;
            canRandomlyTrigger = false;
            numOfLoopsPlayed = 0;
            isFlippedX = false;
        }

        public State(Pet.States assignedState, List<Animation> animations, bool ableToRandomlyTrigger, Pet.States stateToPlayAterThis = Pet.States.Null, int minimumNumberOfLoops = 0, SoundPlayer associatedSound = null) {
            state = assignedState;
            dictionaryOfAnimations = new Dictionary<int, Animation>();
            int keyForState = 0;
            foreach (Animation a in animations) {
                dictionaryOfAnimations.Add(keyForState, a);
                keyForState++;
            }
            animVarient = 0;
            rand = new Random();
            dependantState = stateToPlayAterThis;
            stateComplete = false;
            minNumLoops = minimumNumberOfLoops;
            canRandomlyTrigger = ableToRandomlyTrigger;
            sfx = associatedSound;
            numOfLoopsPlayed = 0;
            isFlippedX = false;
        }

        public Animation GetAnimationToPlay() {
            if (dictionaryOfAnimations.Count == 1){          // Return the animation immediately
                if (dictionaryOfAnimations.ContainsKey(0))
                    return dictionaryOfAnimations[0];
            }
            else if (dictionaryOfAnimations.Count > 1) {    // Randomize the index and return the animation at that index
                animVarient = rand.Next(0, dictionaryOfAnimations.Count);
                //Console.WriteLine("Chosen Animation: " + animVarient);      // Does reach 0, but it skips it entirely; maybe because the 0 varient is somehow never reset?
                if (dictionaryOfAnimations.ContainsKey(animVarient))
                    return dictionaryOfAnimations[animVarient];
            }
            if (numOfLoopsPlayed >= minNumLoops && !stateComplete)
                stateComplete = true;
            return null;                                    // Returns nothing if there are no animations to select from
        }

        public void ResetState() {
            numOfLoopsPlayed = 0;
            stateComplete = false;
        }

        public void IncrementLoop() {
            numOfLoopsPlayed++;
            if (numOfLoopsPlayed >= minNumLoops) {
                if(rand.Next(0,2) >= 1)
                    stateComplete = true;
            } 
        }

        public void RequestFinishingState() {
            stateComplete = true;
        }

        public void PlaySFX() {
            if (sfx != null) {
                sfx.Play();
            }
        }

        public void FlipAllAnimationsInState(bool flipUnflip = true) {
            if (!isFlippedX && flipUnflip) {                            // If the animation is not currently flipped and is requested to be flipped
                foreach (Animation anim in dictionaryOfAnimations.Values)
                    anim.FlipAnimationX(true);
                isFlippedX = true;
            }
            else if (isFlippedX && !flipUnflip)  {                       // If the animation is currently flipped and is requested not to be flipped
                foreach (Animation anim in dictionaryOfAnimations.Values)
                    anim.FlipAnimationX(true);
                isFlippedX = false;
            }
        }
    }
}

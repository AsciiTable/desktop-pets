using System;
using System.Collections.Generic;
using System.Drawing;


namespace desktop_pets
{
    class State
    {
        private Pet.States state;                                   // Defines which state the pet is currently in
        private Dictionary<int, Animation> dictionaryOfAnimations;  // A dictionary of animations associated with this state                             
        private int animVarient;                                    // Keeps track of which animation it's playing out of the list of animations avalible
        private Random rand;

        public State() {                                            // Empty, default constructor
            state = Pet.States.Null;
            dictionaryOfAnimations = new Dictionary<int, Animation>();
            animVarient = 0;
        }

        public State(Pet.States assignedState, List<Animation> animations) {
            state = assignedState;
            dictionaryOfAnimations = new Dictionary<int, Animation>();
            int keyForState = 0;
            foreach (Animation a in animations) {
                dictionaryOfAnimations.Add(keyForState, a);
                keyForState++;
            }
            animVarient = 0;
        }

        public Animation GetAnimationToPlay() {
            if (dictionaryOfAnimations.Count == 1){          // Return the animation immediately
                if (dictionaryOfAnimations.ContainsKey(0))
                    return dictionaryOfAnimations[0];
            }
            else if (dictionaryOfAnimations.Count > 1) {    // Randomize the index and return the animation at that index
                animVarient = rand.Next(0, dictionaryOfAnimations.Count);
                if (dictionaryOfAnimations.ContainsKey(animVarient))
                    return dictionaryOfAnimations[animVarient];
            }
            return null;                                    // Returns nothing if there are no animations to select from
        }
    }
}

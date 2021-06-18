using System;
using System.Collections.Generic;
using System.Drawing;

namespace desktop_pets
{
    class SaveSystem
    {
        public static void SaveNewPet() { 
        }

        public static void UpdatePet() { 
        }

        public static Pet LoadPet() {
            return null;
        }

        // The non-DLC loading function that only loads Rii the Default Cat
        public static Pet LoadRiiTheCat() {
            Dictionary<Pet.States, State> stateLoadingDict = new Dictionary<Pet.States, State>();
            #region Idle State
            List<Animation> idleAnimations = new List<Animation>();
            Animation idle_v0 = new Animation(new Bitmap("Art/cat/idle.png"), 64, 64, 1);
            Animation idle_v1 = new Animation(new Bitmap("Art/cat/idle_blink.png"), 64, 64, 6);
            idleAnimations.Add(idle_v0);
            idleAnimations.Add(idle_v1);
            State idle = new State(Pet.States.Idle, idleAnimations);
            stateLoadingDict.Add(Pet.States.Idle, idle);
            #endregion
            #region Walk State
            List<Animation> walkAnimations = new List<Animation>();
            Animation walk_v0 = new Animation(new Bitmap("Art/Cat/walk_anim.png"), 64, 64, 10);
            Animation walk_v1 = new Animation(new Bitmap("Art/Cat/walk_anim_v1.png"), 64, 64, 10);
            walkAnimations.Add(walk_v0);
            walkAnimations.Add(walk_v1);
            State walk = new State(Pet.States.Walk, walkAnimations);
            stateLoadingDict.Add(Pet.States.Walk, walk);
            #endregion

            return new Pet("Rii", stateLoadingDict);
        }

    }
}

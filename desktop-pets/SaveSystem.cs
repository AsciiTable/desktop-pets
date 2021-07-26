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
            State idle = new State(Pet.States.Idle, idleAnimations, true, Pet.States.Null, 30);
            stateLoadingDict.Add(Pet.States.Idle, idle);
            #endregion
            #region Walk State
            List<Animation> walkAnimations = new List<Animation>();
            Animation walk_v0 = new Animation(new Bitmap("Art/Cat/walk_anim.png"), 64, 64, 6);
            Animation walk_v1 = new Animation(new Bitmap("Art/Cat/walk_anim_v1.png"), 64, 64, 6);
            walkAnimations.Add(walk_v0);
            walkAnimations.Add(walk_v1);
            State walk = new State(Pet.States.Walk, walkAnimations, true, Pet.States.Null, 3);
            stateLoadingDict.Add(Pet.States.Walk, walk);
            #endregion
            #region Drag State
            List<Animation> dragAnimations = new List<Animation>();
            Animation drag_v0 = new Animation(new Bitmap("Art/Cat/drag_v0.png"), 64, 64, 1);
            dragAnimations.Add(drag_v0);
            State drag = new State(Pet.States.Drag, dragAnimations, false, Pet.States.Fall, 1);
            stateLoadingDict.Add(Pet.States.Drag, drag);
            #endregion
            #region Fall State
            List<Animation> fallAnimations = new List<Animation>();
            Animation fall_v0 = new Animation(new Bitmap("Art/Cat/fall_v0.png"), 64, 64, 1);
            fallAnimations.Add(fall_v0);
            State fall = new State(Pet.States.Fall, fallAnimations, false, Pet.States.Idle, 1);
            stateLoadingDict.Add(Pet.States.Fall, fall);
            #endregion
            return new Pet("Rii", stateLoadingDict);
        }

    }
}

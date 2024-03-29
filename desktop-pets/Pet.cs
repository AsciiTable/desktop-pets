﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace desktop_pets
{
    [System.Serializable]
    public class Pet
    {
        public enum States                                     // All avaliable states that a pet could be in
        {
            Idle, Walk, Drag, Fall, Land, Attention, Satisfied, Null
        }
        #region Attributes
        public string name { get; private set; }               // Name of the pet
        public Dictionary<States, State> dictionaryOfStates { get; private set; }  // A dictionary with states to have more flexible management of avalible states
        private List<States> listOfSelectableStates;
        #endregion

        public State activeState { get; private set; }
        public Animation currentAnimation { get; set; }
        //private List<States> dependantStates;
        private Random rand;
        public int XSIZE = 0;
        public int YSIZE = 0;
        public Color transparentColor;

        public Pet() {
            name = "";
            dictionaryOfStates = new Dictionary<States, State>();
            listOfSelectableStates = new List<States>(dictionaryOfStates.Keys);
            activeState = null;
            //dependantStates = new List<States>();
            rand = new Random();
            transparentColor = new Color();
        }

        public Pet(string petName, Dictionary<States, State> dictOfStates, Color transcolor, int xsize = 64, int ysize = 64) {
            name = petName;
            dictionaryOfStates = dictOfStates;
            listOfSelectableStates = new List<States>(dictionaryOfStates.Keys);
            //dependantStates = new List<States>();
            foreach (State s in dictOfStates.Values) {
                if (!s.canRandomlyTrigger)
                    listOfSelectableStates.Remove(s.state);
            }
            if (dictOfStates.ContainsKey(States.Idle))
                activeState = dictOfStates[States.Idle];
            else
                activeState = null;
            rand = new Random();
            XSIZE = xsize;
            YSIZE = ysize;
            transparentColor = transcolor;
        }


        public States AutoPickNextState() {
            if (dictionaryOfStates.Count > 0)
            {
                if (activeState.dependantState == States.Null)
                {
                    // Pick a new independent state
                    int chosenStateNum = rand.Next(0, listOfSelectableStates.Count);
                    activeState.ResetState();
                    activeState = dictionaryOfStates[listOfSelectableStates[chosenStateNum]];   // Trying to switch to drag caused an issue here (out of index error)
                    Console.WriteLine("State chosen: " + activeState.state.ToString());
                    activeState.PlaySFX();
                    return activeState.state;
                }
                else
                {
                    // Go to dependent state
                    foreach (State s in dictionaryOfStates.Values)
                    {
                        if (s.state == activeState.dependantState)
                        {
                            activeState = s;
                            return activeState.state;
                        }
                    }
                }
            }
            return States.Null;
        }

        public void ImmediatelyChangeToThisState(Pet.States s) {
            foreach (State sl in dictionaryOfStates.Values) {
                if (s == sl.state) {
                    if(activeState != null)
                        activeState.ResetState();
                    Console.Write("Immediately changed from " + activeState.state.ToString() + " to ");
                    activeState = sl;
                    Console.WriteLine(activeState.state.ToString() + ".");
                    activeState.PlaySFX();
                    break;
                }
            }
        }
    }
}

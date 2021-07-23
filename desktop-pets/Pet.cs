using System;
using System.Collections.Generic;
using System.Drawing;

namespace desktop_pets
{
    [System.Serializable]
    public class Pet
    {
        public enum States                                     // All avaliable states that a pet could be in
        {
            Idle, Walk, Drag, Land, Needy, Satisfied, Null
        }
        #region Attributes
        public string name { get; private set; }               // Name of the pet
        public Dictionary<States, State> dictionaryOfStates { get; private set; }  // A dictionary with states to have more flexible management of avalible states
        private List<States> listOfSelectableStates;
        #endregion

        public State activeState { get; private set; }
        public Animation currentAnimation { get; set; }
        private List<States> dependantStates;
        private bool independantStateExists;
        private Random rand;

        public Pet() {
            name = "";
            dictionaryOfStates = new Dictionary<States, State>();
            listOfSelectableStates = new List<States>(dictionaryOfStates.Keys);
            activeState = null;
            dependantStates = new List<States>();
            independantStateExists = false;
            rand = new Random();
        } 

        public Pet(string petName, Dictionary<States, State> dictOfStates) {
            name = petName;
            dictionaryOfStates = dictOfStates;
            listOfSelectableStates = new List<States>(dictionaryOfStates.Keys);
            dependantStates = new List<States>();
            foreach (State s in dictOfStates.Values) {
                if (s.dependantState != States.Null && !dependantStates.Contains(s.dependantState)) {
                    dependantStates.Add(s.dependantState);
                    foreach (States sl in listOfSelectableStates) {
                        if (sl.Equals(s.dependantState)) {
                            listOfSelectableStates.Remove(sl);
                            break;
                        }
                    }
                }
            }
            if (dictOfStates.ContainsKey(States.Idle))
                activeState = dictOfStates[States.Idle];
            else
                activeState = null;
            rand = new Random();
        }


        public States AutoPickNextState() {
            if (dictionaryOfStates.Count > 0 && (dictionaryOfStates.Count > dependantStates.Count))
            {
                if (activeState.dependantState == States.Null)
                {
                    // Pick a new independent state
                    int chosenStateNum = rand.Next(0, dictionaryOfStates.Count);
                    Console.WriteLine("Random number chosen: " + chosenStateNum);
                    activeState = dictionaryOfStates[listOfSelectableStates[chosenStateNum]];
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
    }
}

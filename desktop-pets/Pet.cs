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
        #endregion

        public State activeState { get; private set; }
        public Animation currentAnimation { get; set; }
        private List<States> dependantStates;

        public Pet() {
            name = "";
            dictionaryOfStates = new Dictionary<States, State>();
            activeState = null;
            dependantStates = new List<States>();
        }

        public Pet(string petName, Dictionary<States, State> dictOfStates) {
            name = petName;
            dictionaryOfStates = dictOfStates;
            dependantStates = new List<States>();
            foreach (State s in dictOfStates.Values) {
                if (s.dependantState != States.Null && !dependantStates.Contains(s.dependantState))
                    dependantStates.Add(s.dependantState);
            }
            if (dictOfStates.ContainsKey(States.Idle))
                activeState = dictOfStates[States.Idle];
            else
                activeState = null;
        }

        // BEHAVIOUR TREE
        public void AutoPickNextState() { 
            
        }


    }
}

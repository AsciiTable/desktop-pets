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

        public Pet() {
            name = "";
            dictionaryOfStates = new Dictionary<States, State>();
            activeState = null;
        }

        public Pet(string petName, Dictionary<States, State> dictOfStates) {
            name = petName;
            dictionaryOfStates = dictOfStates;
            if (dictOfStates.ContainsKey(States.Idle))
                activeState = dictOfStates[States.Idle];
            else
                activeState = null;
        }

        // BEHAVIOUR TREE
    }
}

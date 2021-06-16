using System;
using System.Collections.Generic;
using System.Drawing;

namespace desktop_pets
{
    class Pet
    {
        public enum States                                     // All avaliable states that a pet could be in
        {
            Idle, Walk, Drag, Land, Needy, Satisfied, Null
        }
        private Dictionary<States, State> dictionaryOfStates;  // A dictionary with states to have more flexible management of avalible states
        
        // BEHAVIOUR TREE
    }
}

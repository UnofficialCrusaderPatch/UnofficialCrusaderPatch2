using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.AICharacters
{
    /// <summary> How the AI determines its next attack target. </summary>
    public enum TargetingType
    {
        /// <summary> Goes for the enemy with the highest gold. </summary>
        Gold,
        /// <summary> Goes for an average of weakest & closest enemy. </summary>
        Balanced,
        /// <summary> Goes for the closest enemy. </summary>
        Closest,
        /// <summary> Nothing specified. </summary>
        Any,
        /// <summary> Goes for the player or if no players are left for the closest AI. </summary>
        Player
    }
}

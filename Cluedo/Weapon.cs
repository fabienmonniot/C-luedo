using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    [Serializable]
    class Weapon : Item, ICollectible
    {
        #region Attributes & Properties

        /// <summary>
        /// Item that enables the collection of the weapon
        /// </summary>
        public Item Requirement { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the weapon is collectible
        /// </summary>
        /// <returns>The boolean associated</returns>
        public bool IsCollectible()
        {
            if(Requirement != null)
                return Requirement.Observed;
            return false;
        }

        /// <summary>
        /// Collects the weapon
        /// </summary>
        public void Collect()
        {
            base.Observe();

            Room.Game.Collect(this);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="name">Name of the weapon</param>
        /// <param name="description">Description of the weapon</param>
        /// <param name="observed">Has the weapon already been observed ?</param>
        /// <param name="extraTime">Extra-time of the observation of the weapon</param>
        /// <param name="specialObserved">Has the weapon already been special-observed ?</param>
        /// <param name="specialInitiator">Suspect that initiates the special observation</param>
        /// <param name="specialDescription">Special description of the weapon</param>
        /// <param name="requirement">Item that makes the weapon collectible</param>
        public Weapon(ConsoleString name,
            List<ConsoleString> description,
            bool observed,
            int extraTime,
            bool specialObserved,
            Suspect specialInitiator,
            List<ConsoleString> specialDescription,
            Item requirement)
            : base(name, description, observed, 0, specialObserved, specialInitiator, specialDescription)
        {
            Requirement = requirement;
            Observed = false;
        }

        /// <summary>
        /// Constructor without ExtraObserveTime and special observation
        /// </summary>
        /// <param name="name">Name of the weapon</param>
        /// <param name="description">Description of the weapon</param>
        /// <param name="requirement">Item that makes the weapon collectible</param>
        public Weapon(ConsoleString name, List<ConsoleString> description, Item requirement)
            : this(name, description, false, 0, false, null, null, requirement)
        {

        }

        #endregion
    }
}

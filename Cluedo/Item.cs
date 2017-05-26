using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    [Serializable]
    class Item : IObservable
    {
        #region Static attributes

        /// <summary>
        /// Duration of an observation
        /// </summary>
        public static readonly int ObserveDuration = 10;

        #endregion

        #region Attributes & Properties

        /// <summary>
        /// Room of the item
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        public ConsoleString Name { get; set; }

        /// <summary>
        /// Description of the item
        /// </summary>
        public List<ConsoleString> Description { get; set; }

        /// <summary>
        /// Has the item already been observed ?
        /// </summary>
        public bool Observed { get; set; }

        /// <summary>
        /// Extra-time of the oservation
        /// </summary>
        public int ExtraObserveTime { get; set; }

        /// <summary>
        /// Has the item already been special-observed ?
        /// </summary>
        public bool SpecialObserved;

        /// <summary>
        /// Suspect that enables the special description
        /// </summary>
        public Suspect SpecialInitiator;

        /// <summary>
        /// Special description of the item
        /// </summary>
        public List<ConsoleString> SpecialDescription { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the description, and adds it to the memo.
        /// Updates the time of the game.
        /// </summary>
        public void Observe()
        {
            Program.ColorWriteLine(Description);

            var descriptionList = Program.AddNewLine(Description, new ConsoleString(Name + " :"));
            Room.Game.AddToMemo(descriptionList);

            Room.Game.AddMinutes(ObserveDuration + ExtraObserveTime);
            Observed = true;
        }

        /// <summary>
        /// Checks if the item is special-observable
        /// </summary>
        /// <returns>The boolean associated</returns>
        public bool IsSpecialObservable()
        {
            return (SpecialDescription != null && SpecialInitiator.SpecialTalked);
        }

        /// <summary>
        /// Special observes
        /// </summary>
        public void SpecialObserve()
        {
            if (IsSpecialObservable())
            {
                Program.ColorWriteLine(SpecialDescription);

                Room.Game.AddMinutes(ObserveDuration);

                var specialDescriptionList = Program.AddNewLine(Description, new ConsoleString(Name + " :"));
                Room.Game.AddToMemo(specialDescriptionList);

                SpecialObserved = true;
            }
        }

        /// <summary>
        /// Overrides the method ToString()
        /// </summary>
        /// <returns>The name of the item</returns>
        public override string ToString()
        {
            return Name.text;
        }

        #endregion

        #region Constructors
      
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="name">Name of the item</param>
        /// <param name="description">Description of the item</param>
        /// <param name="observed">Has the item already been observed ?</param>
        /// <param name="extraTime">Extra-time of the observation of the item</param>
        /// <param name="specialObserved">Has the item already been special-observed ?</param>
        /// <param name="specialInitiator">Suspect that initiates the special observation</param>
        /// <param name="specialDescription">Special description of the item</param>
        public Item(ConsoleString name, 
            List<ConsoleString> description, 
            bool observed, 
            int extraTime, 
            bool specialObserved, 
            Suspect specialInitiator, 
            List<ConsoleString> specialDescription)
        {
            Name = name;
            Description = description;
            Observed = observed;
            ExtraObserveTime = extraTime;
            SpecialObserved = specialObserved;
            SpecialInitiator = specialInitiator;
            SpecialDescription = specialDescription;
        }

        /// <summary>
        /// Constructor without ExtraObserveTime and special observation
        /// </summary>
        /// <param name="name">Name of the item</param>
        /// <param name="description">Description of the item</param>
        public Item(ConsoleString name,
            List<ConsoleString> description) 
            : this(name, description, false, 0, false, null, null)
        {

        }

        #endregion
    }
}

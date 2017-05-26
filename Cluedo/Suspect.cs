using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    [Serializable]
    class Suspect : NPC, IObservable
    {
        #region Static attributes

        /// <summary>
        /// Duration of a talk
        /// </summary>
        public readonly new static int TalkDuration = 20;

        /// <summary>
        /// Duration of a special talk
        /// </summary>
        public readonly static int SpecialTalkDuration = 20;

        /// <summary>
        /// Duration of an observation
        /// </summary>
        public readonly static int ObserveDuration = 10;

        #endregion

        #region Attributes and properties

        /// <summary>
        /// Description of the suspect
        /// </summary>
        public List<ConsoleString> Description { get; set; }

        /// <summary>
        /// Has the suspect already been observed ?
        /// </summary>
        public bool Observed { get; set; }

        /// <summary>
        /// Has the suspect already been special talked ?
        /// </summary>
        public bool SpecialTalked { get; protected set; }

        /// <summary>
        /// Extra time of the talk
        /// </summary>
        public int ExtraTalkTime { get; protected set; }

        /// <summary>
        /// Extra time of the observation
        /// </summary>
        public int ExtraObserveTime { get; set; }

        /// <summary>
        /// Answer of the special talk
        /// </summary>
        public List<ConsoleString> SpecialAnswer { get; set; }

        /// <summary>
        /// Special object that enables the special talk
        /// </summary>
        public Weapon SpecialObject;

        #endregion

        #region Methods

        /// <summary>
        /// Displays the description, and adds it to the memo.
        /// Updates the time of the game.
        /// </summary>
        public void Observe()
        {
            var observeList = Program.AddNewLine(Description, new ConsoleString(Name + " :"));
            Room.Game.AddToMemo(observeList);

            Room.Game.AddMinutes(ObserveDuration + ExtraObserveTime);

            Program.ColorWriteLine(Description);

            Observed = true;
        }

        /// <summary>
        /// Displays the answer, and adds it to the memo.
        /// Updates the time of the game.
        /// </summary>
        public override void Talk()
        {
            base.Talk();
            Room.Game.AddMinutes(ExtraTalkTime);
        }

        /// <summary>
        /// Displays the special answer, and adds it to the memo.
        /// Updates the time of the game.
        /// </summary>
        public void SpecialTalk()
        {
            Program.ColorWriteLine(SpecialAnswer);

            Room.Game.AddMinutes(SpecialTalkDuration);
            
            var specialTalkList = Program.AddNewLine(SpecialAnswer, new ConsoleString(Name + " :"));
            Room.Game.AddToMemo(specialTalkList);

            SpecialTalked = true;
        }

        /// <summary>
        /// Checks if the special talk is available.
        /// </summary>
        /// <returns>the boolean associated</returns>
        public bool IsSpecialAvailable()
        {
            return Room.Game.IsCollected(SpecialObject);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="name">Name of the Suspect</param>
        /// <param name="answer">Answer of the Suspect</param>
        /// <param name="talked">Has the Suspect already talked ?</param>
        /// <param name="description">Description of the suspect</param>
        /// <param name="observed">Has the Suspect already been observed ?</param>
        /// <param name="extraTalkTime">Extra time of the talk with the suspect</param>
        /// <param name="extraObserveTime">Extra time of the observation of the suspect</param>
        /// <param name="specialAnswer">Special answer of the suspect</param>
        /// <param name="specialTalked">Has the Suspect already special-talked ?</param>
        /// <param name="specialObject">Special object that enables the special answer</param>
        public Suspect(ConsoleString name, 
            List<ConsoleString> answer,
            bool talked,
            List<ConsoleString> description,
            bool observed,
            int extraTalkTime,
            int extraObserveTime,
            List<ConsoleString> specialAnswer,
            bool specialTalked,
            Weapon specialObject)
            : base(name, answer, talked)
        {
            Description = description;
            Observed = observed;
            ExtraTalkTime = extraTalkTime;
            ExtraObserveTime = extraObserveTime;
            SpecialAnswer = specialAnswer;
            SpecialTalked = specialTalked;
            SpecialObject = specialObject;
        }

        /// <summary>
        /// Constructor without extraTalkTime and extraObserveTime
        /// </summary>
        /// <param name="name">Name of the Suspect</param>
        /// <param name="answer">Answer of the Suspect</param>
        /// <param name="talked">Has the Suspect already talked ?</param>
        /// <param name="description">Description of the suspect</param>
        /// <param name="observed">Has the Suspect already been observed ?</param>
        /// <param name="specialAnswer">Special answer of the suspect</param>
        /// <param name="specialTalked">Has the Suspect already special-talked ?/param>
        /// <param name="specialObject">Special object that enables the special answer</param>
        public Suspect(ConsoleString name,
            List<ConsoleString> answer,
            bool talked,
            List<ConsoleString> description,
            bool observed,
            List<ConsoleString> specialAnswer,
            bool specialTalked,
            Weapon specialObject)
            : this(name, answer, talked, description, observed, 0, 0, specialAnswer, specialTalked, specialObject)
        {
        }

        #endregion

    }
}

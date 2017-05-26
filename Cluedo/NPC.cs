using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    [Serializable]
    class NPC
    {
        #region Static attribute

        /// <summary>
        /// Duration of a talk
        /// </summary>
        public readonly static int TalkDuration = 60;

        #endregion

        #region Attributes and properties
        
        /// <summary>
        /// Room of the NPC
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// Name of the NPC
        /// </summary>
        public ConsoleString Name { get; protected set; }

        /// <summary>
        /// Answer of the NPC
        /// </summary>
        protected List<ConsoleString> _answer;

        /// <summary>
        /// Has the player already talked to the NPC ?
        /// </summary>
        public bool Talked { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the answer, and adds it to the memo.
        /// Updates the time of the game.
        /// </summary>
        public virtual void Talk()
        {
            Program.ColorWriteLine(_answer);

            var talkList = Program.AddNewLine(_answer, new ConsoleString(Name + " :"));
            Room.Game.AddToMemo(talkList);

            Room.Game.AddMinutes(TalkDuration);
            Talked = true;
        }

        /// <summary>
        /// Overrides the method ToString()
        /// </summary>
        /// <returns>The name of the NPC</returns>
        public override string ToString()
        {
            return Name.text;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="name">Name of the NPC</param>
        /// <param name="answer">Answer of the NPC</param>
        /// <param name="talked">Has the Suspect has already talked ?</param>
        public NPC(ConsoleString name, List<ConsoleString> answer, bool talked)
        {
            Name = name;
            _answer = answer;
            Talked = talked;
        }

        /// <summary>
        /// Partial constructor 
        /// </summary>
        /// <param name="name">Name of the NPC</param>
        /// <param name="answer">Answer of the NPC</param>
        public NPC(ConsoleString name, List<ConsoleString> answer) : this(name, answer, false)
        {

        }

        #endregion
    }
}

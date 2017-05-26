using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    [Serializable]
    class Policeman : NPC
    {
        #region Attributes and properties

        /// <summary>
        /// Reactions of the policeman
        /// </summary>
        protected List<ConsoleString> _reactions;

        /// <summary>
        /// Answer of the policeman if the accusation was right
        /// </summary>
        protected List<ConsoleString> _goodAnswer;

        /// <summary>
        /// Answer of the policeman if the accusation was false
        /// </summary>
        protected List<ConsoleString> _badAnswer;

        /// <summary>
        /// Talk of the policeman at the beginning of the game
        /// </summary>
        protected List<ConsoleString> _introduction;

        /// <summary>
        /// Talk of the policeman when the daughter arrives
        /// </summary>
        protected List<ConsoleString> _daughterArriveTalk;

        #endregion

        #region Methods

        /// <summary>
        /// Displays the introduction, and adds it to the memo
        /// </summary>
        public void TalkIntro()
        {
            Console.Clear();
            Program.DisplayCluedoTitle();

            Program.ColorWriteLine(_introduction);
            Room.Game.AddToMemo(_introduction);
            Console.ReadKey();
        }

        /// <summary>
        /// Displays the talk describing the arrival of the daughter
        /// </summary>
        public void TalkDaughterArrive()
        {
            Program.ColorWriteLine("");
            Program.ColorWriteLine(_daughterArriveTalk);
            Room.Game.AddToMemo(_daughterArriveTalk);
        }

        /// <summary>
        /// Displays the answer of the Policeman if the accusation is right 
        /// </summary>
        public void TalkGoodAnswer()
        {
            Console.Clear();
            Program.ColorWriteLine(_goodAnswer);
            Room.Game.AddToMemo(_goodAnswer);
        }

        /// <summary>
        /// Displays the answer of the Policeman if the accusation is false 
        /// </summary>
        public void TalkBadAnswer()
        {
            Console.Clear();
            Program.ColorWriteLine(_badAnswer);
            Room.Game.AddToMemo(_badAnswer);
        }

        /// <summary>
        /// Displays a random reaction of the policeman
        /// </summary>
        public void React()
        {
            var random = new Random();
            int randId = random.Next(0, _reactions.Count);

            var reaction = new List<ConsoleString>();
            reaction.Add(new ConsoleString("\n" + Name + " :"));
            reaction.Add(_reactions[randId]);

            Program.ColorWriteLine(reaction);
        }

        #endregion 

        #region Constructor

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="name">Name of the Policeman</param>
        /// <param name="reactions">Ripostes of the Policeman</param>
        /// <param name="daughterArriveTalk">Talk of the policeman when the daughter arrives</param>
        /// <param name="goodAnswer">Answer of the Policeman if the accusation is right</param>
        /// <param name="badAnswer">Answer of the Policeman if the accusation is false</param>
        /// <param name="introduction">Introduction of the game, told by the Policeman</param>
        public Policeman(ConsoleString name,
            List<ConsoleString> reactions, 
            List<ConsoleString> daughterArriveTalk,
            List<ConsoleString> goodAnswer, 
            List<ConsoleString> badAnswer, 
            List<ConsoleString> introduction,
            bool talked)
            : base(name, null, talked)
        {
            _reactions = reactions;
            _daughterArriveTalk = daughterArriveTalk;
            _goodAnswer = goodAnswer;
            _badAnswer = badAnswer;

            introduction.Insert(0, new ConsoleString(name.ToString() + " :"));
            _introduction = introduction;
        }

        #endregion
    }
}

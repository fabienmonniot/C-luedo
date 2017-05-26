using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    /// <summary>
    /// Action to do
    /// </summary>
    public delegate void DoAction();

    /// <summary>
    /// All the actions the player can do
    /// </summary>
    public enum Actions
    {
        Accuse,
        Collect,
        Help,
        Observe,
        Move,
        None,
        ReadMemo,
        SpecialObserve,
        SpecialTalk,
        Talk
    }

    class CluedoAction
    {
        #region Attributes and properties

        /// <summary>
        /// Action chosen by the player
        /// </summary>
        public Actions ActionChosen { get; protected set; }

        /// <summary>
        /// Action to do
        /// </summary>
        public DoAction DoAction;

        /// <summary>
        /// Text of the action
        /// </summary>
        protected ConsoleString _text;

        #endregion
        
        #region Methods

        /// <summary>
        /// Overrides the ToString() method
        /// </summary>
        /// <returns>The text of the action</returns>
        public override string ToString()
        {
            return _text.text;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="actionChosen">Action chosen by the player</param>
        /// <param name="text">Text of the action</param>
        /// <param name="action">Method associated</param>
        public CluedoAction(Actions actionChosen, ConsoleString text, DoAction action)
        {
            ActionChosen = actionChosen;
            _text = text;
            DoAction += action;
        }

        #endregion
    }
}

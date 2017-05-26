using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Cluedo
{
    [Serializable]
    class Game
    {
        #region Static attributes

        /// <summary>
        /// Hour when the game starts
        /// </summary>
        public static readonly int StartHour = 8;

        /// <summary>
        /// Minutes when the game starts
        /// </summary>
        public static readonly int StartMinute = 0;

        /// <summary>
        /// Hour when the daughter arrives
        /// </summary>
        public static readonly int DaughterHour = 12;
        
        /// <summary>
        /// Hour when the game ends
        /// </summary>
        public static readonly int EndHour = 18;

        /// <summary>
        /// Minutes when the game ends
        /// </summary>
        public static readonly int EndMinute = 0;

        /// <summary>
        /// Duration of the move to another room
        /// </summary>
        public static readonly int MoveDuration = 10;

        /// <summary>
        /// Duration of the collection of a weapon
        /// </summary>
        public static readonly int CollectDuration = 10;

        #endregion

        #region Attributes & Properties

        /// <summary>
        /// Is game over ?
        /// </summary>
        public bool End { get; protected set; }

        /// <summary>
        /// Has the player made the right accusation ?
        /// </summary>
        public bool Won { get; protected set; }

        /// <summary>
        /// Name of the player
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Timestamp of the creation of the game
        /// </summary>
        public string Timestamp { get; protected set; }

        /// <summary>
        /// Current room in the game
        /// </summary>
        public Room CurrentRoom { get; set; }

        /// <summary>
        /// All the rooms of the game
        /// </summary>
        protected List<Room> _rooms;

        /// <summary>
        /// Policeman of the game
        /// </summary>
        public Policeman Policeman { get; set; }

        /// <summary>
        /// Daughter of the game
        /// </summary>
        public NPC Daughter { get; set; }

        /// <summary>
        /// Current date of the game
        /// </summary>
        protected CluedoDate _currentDate;

        /// <summary>
        /// Killer of the game
        /// </summary>
        protected CluedoKiller _killer;

        /// <summary>
        /// Help for the player
        /// </summary>
        protected string _help;

        /// <summary>
        /// All the sentences of the game for i18n
        /// </summary>
        public CluedoSentences Sentences;

        /// <summary>
        /// Inventory of the player in the game
        /// </summary>
        protected List<ICollectible> _inventory;

        /// <summary>
        /// Memo of the player in the game
        /// </summary>
        protected List<ConsoleString> _memo;

        /// <summary>
        /// Has the daughter arrived ?
        /// </summary>
        bool _daughterArrived;

        #endregion

        #region Methods

        /// <summary>
        /// Add items to the inventory
        /// </summary>
        /// <param name="items">Items to add</param>
        /// <returns>The game</returns>
        public Game Collect(params ICollectible[] items)
        {
            foreach (ICollectible item in items)
            {
                _inventory.Add(item);
            }

            return this;
        }

        /// <summary>
        /// Add rooms to the game
        /// </summary>
        /// <param name="rooms">Rooms to add</param>
        /// <returns>The game</returns>
        public Game AddRooms(params Room[] rooms)
        {
            foreach (Room room in rooms)
            {
                _rooms.Add(room);
                room.Game = this;
            }

            return this;
        }

        /// <summary>
        /// Add minutes to the game
        /// </summary>
        /// <param name="minutes">Minutes to add</param>
        /// <returns>The game</returns>
        public Game AddMinutes(int minutes)
        {
            if (minutes > 0)
            {
                int hours = (_currentDate.minute + minutes) / 60;
                _currentDate.hour += hours;
                _currentDate.minute = (_currentDate.minute + minutes) % 60;
            }

            return this;
        }

        /// <summary>
        /// Adds to the memo
        /// </summary>
        /// <param name="stringObject">Object to add</param>
        /// <returns>The game</returns>
        public Game AddToMemo(Object stringObject)
        {
            var list = Program.ConvertToLCS(stringObject);
            list.Add(new ConsoleString("\n"));

            foreach (var consoleString in list)
            {
                _memo.Add(consoleString);
            }

            return this;
        }

        /// <summary>
        /// Checks if the accusation is right
        /// </summary>
        /// <param name="suspect">Suspect accused</param>
        /// <param name="weapon">Weapon accused</param>
        /// <returns>The boolean associated</returns>
        public bool IsKiller(Suspect suspect, Weapon weapon)
        {
            return (suspect == _killer.suspect && weapon == _killer.weapon);
        }

        /// <summary>
        /// Checks if an item has already been collected
        /// </summary>
        /// <param name="collectible">The item</param>
        /// <returns>The boolean associated</returns>
        public bool IsCollected(ICollectible collectible)
        {
            return _inventory.Contains(collectible);
        }

        /// <summary>
        /// Sets the killer and the weapon
        /// </summary>
        /// <param name="suspect">The suspect</param>
        /// <param name="weapon">The weapon</param>
        /// <returns>The game</returns>
        public Game setKiller(Suspect suspect, Weapon weapon)
        {
            _killer.suspect = suspect;
            _killer.weapon = weapon;
            return this;
        }

        /// <summary>
        /// Accuses a suspect and a weapon, ends the game
        /// </summary>
        public void Accuse()
        {
            End = true;

            //Get suspects and weapons 
            var suspects = new List<Object>();
            var weapons = new List<Object>();

            foreach (Room room in _rooms)
            {
                //Get suspects
                foreach (NPC npc in room.GetNPCs())
                {
                    if (npc is Suspect)
                        suspects.Add(npc);
                }

                //Get weapons
                foreach (Item item in room.GetItems())
                {
                    if (item is Weapon)
                        weapons.Add(item);
                }
            }

            // Get accusation
            int suspectAccused = Program.AskPlayer(Sentences.AccuseKiller, suspects.ToArray());
            int weaponAccused = Program.AskPlayer(Sentences.AccuseWeapon, weapons.ToArray());

            Won = IsKiller((Suspect)suspects[suspectAccused], (Weapon)weapons[weaponAccused]);
        }

        /// <summary>
        /// Shows help
        /// </summary>
        /// <returns>The game</returns>
        public Game Help()
        {
            Console.Clear();
            Program.DisplayCluedoTitle();
            Program.DisplayTitle(Sentences.HelpTitle, 10);

            Program.ColorWriteLine(_help);
            Console.ReadKey();
            return this;
        }

        /// <summary>
        /// Displays the memo
        /// </summary>
        /// <returns>The game</returns>
        public Game Memo()
        {
            Console.Clear();
            Program.DisplayCluedoTitle();
            Program.DisplayTitle(Sentences.MemoTitle, 10);

            Program.ColorWriteLine(_memo);
            Console.ReadKey();
            return this;
        }

        /// <summary>
        /// Moves to another room
        /// </summary>
        /// <param name="room">The room to go</param>
        /// <returns>The game</returns>
        public Game Move(Room room)
        {
            if (_rooms.Contains(room) && CurrentRoom.HasNeighbor(room))
            {
                AddMinutes(MoveDuration);
                CurrentRoom = room;
            }

            return this;
        }

        /// <summary>
        /// Asks the player and moves the player in another room
        /// </summary>
        /// <returns>The game</returns>
        public Game ChangeRoom()
        {
            var possibleRooms = new string[CurrentRoom.GetNeighbors().Length + 1];
            var rooms = CurrentRoom.GetNeighbors();
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i] == CurrentRoom.TrapDoor)
                    possibleRooms[i] = Sentences.TrapDoor;
                else
                    possibleRooms[i] = rooms[i].Name.text;
            }
            possibleRooms[possibleRooms.Length - 1] = Sentences.NoneAction;

            int roomChosen = Program.AskPlayer(Sentences.ChangeRoom, possibleRooms.Select(x => (Object)x).ToArray());

            if (roomChosen != possibleRooms.Length - 1)
                Move(CurrentRoom.GetNeighbors()[roomChosen]);

            return this;
        }

        /// <summary>
        /// Saves the game
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Program.SavesPath + Timestamp, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();

            return true;
        }

        /// <summary>
        /// Gets the action chosen by the player
        /// </summary>
        /// <param name="possibilityChosen">Possibility chosen by the player</param>
        /// <param name="possibilities">All the possibilities</param>
        /// <returns>The game</returns>
        public Game GetChosenAction(out int possibilityChosen, out List<Object> possibilities)
        {
            //Get possibilities
            var possibleItems = CurrentRoom.GetItems();
            var possibleNPCs = CurrentRoom.GetNPCs();
            possibilities = new List<Object>();

            //Add items
            foreach (var item in possibleItems)
            {
                if (item is Weapon)
                {
                    var weapon = (Weapon)item;
                    if (weapon.IsCollectible() && !IsCollected(weapon))
                    {
                        possibilities.Add(weapon);
                    }
                }
                else
                    possibilities.Add(item);
            }

            //Add NPC
            foreach (var npc in possibleNPCs)
            {
                if (npc != Daughter || _currentDate.hour == DaughterHour)
                    possibilities.Add(npc);
            }

            //Go to another room
            possibilities.Add("\n" + Sentences.MoveAction);

            //Display memo
            possibilities.Add("\n" + Sentences.ReadMemoAction);

            //Display Help
            possibilities.Add(Sentences.HelpAction);

            // Get the possibility chosen by the player
            possibilityChosen = Program.AskPlayer(Sentences.KnowMore, possibilities.ToArray());

            return this;
        }

        /// <summary>
        /// Do the action chosen by the player
        /// </summary>
        /// <param name="choice">Choice of the player</param>
        /// <returns>The game</returns>
        public Game DoChosenAction(Object choice)
        {
            var possibleActions = new List<CluedoAction>();

            // Add Observe
            if (choice is IObservable && !(choice is Weapon))
            {
                var observable = (IObservable)choice;
                if (!observable.Observed)
                    possibleActions.Add(new CluedoAction(Actions.Observe, new ConsoleString(Sentences.ObserveAction), observable.Observe));

                //Add SpecialObserve
                if (choice is Item)
                {
                    var item = (Item)choice;
                    if (item.IsSpecialObservable() && !item.SpecialObserved)
                        possibleActions.Add(new CluedoAction(Actions.SpecialObserve, new ConsoleString(Sentences.SpecialObserveAction), item.SpecialObserve));
                }
            }
            // Add Collect
            if (choice is ICollectible)
            {
                var collectible = (ICollectible)choice;
                possibleActions.Add(new CluedoAction(Actions.Collect, new ConsoleString(Sentences.CollectAction), collectible.Collect));
            }
            if (choice is NPC)
            {
                // Add Accuse
                if (choice is Policeman)
                {
                    var policeman = (Policeman)choice;
                    possibleActions.Add(new CluedoAction(Actions.Accuse, new ConsoleString(Sentences.AccuseAction), Accuse));
                }

                // Add Talk
                else
                {
                    var npc = (NPC)choice;

                    if (!npc.Talked)
                        possibleActions.Add(new CluedoAction(Actions.Talk, new ConsoleString(Sentences.TalkAction), npc.Talk));

                    // Add SpecialTalk
                    if (choice is Suspect)
                    {
                        var suspect = (Suspect)choice;
                        if (suspect.IsSpecialAvailable() && !suspect.SpecialTalked)
                            possibleActions.Add(new CluedoAction(Actions.SpecialTalk, new ConsoleString(Sentences.SpecialTalkAction), suspect.SpecialTalk));
                    }
                }
            }

            possibleActions.Add(new CluedoAction(Actions.None, new ConsoleString(Sentences.NoneAction), Play));

            int actionChosen = Program.AskPlayer(Sentences.WhichAction, possibleActions.Select(x => (Object)x).ToArray());

            Program.ColorWriteLine("");
            possibleActions[actionChosen].DoAction();

            Console.ReadKey();

            return this;
        }

        /// <summary>
        /// Plays a turn
        /// </summary>
        public void Play()
        {
            Console.Clear();
            Program.DisplayCluedoTitle();

            //Displays date
            Program.DisplayTitle(_currentDate.ToString(), 0);

            //Room description
            Program.ColorWriteLine(CurrentRoom.Description);

            //Reactions of the policeman
            if (CurrentRoom.GetNPCs().Contains(Policeman))
                Policeman.React();

            //Daughter arrives
            if (_currentDate.hour == DaughterHour && !_daughterArrived)
            {
                Policeman.TalkDaughterArrive();
                _daughterArrived = true;
            }

            //Get the action chosen by the player
            int possibilityChosen;
            List<Object> possibilities;
            GetChosenAction(out possibilityChosen, out possibilities);

            //Do the action chosen
            if (possibilityChosen == possibilities.Count - 1)
                Help();
            else if (possibilityChosen == possibilities.Count - 2)
                Memo();
            else if (possibilityChosen == possibilities.Count - 3)
                ChangeRoom();
            else
                DoChosenAction(possibilities[possibilityChosen]);

            //End of the time
            if (_currentDate.hour >= EndHour && _currentDate.minute >= EndMinute)
            {
                Console.Clear();

                //Displays date
                Program.DisplayTitle(_currentDate.ToString(), 0);

                Program.ColorWriteLine(Sentences.PolicemanEnd);
                Accuse();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="playerName">Name of the player</param>
        /// <param name="currentRoom">Current room in the game</param>
        /// <param name="hour">Hour when the game starts</param>
        /// <param name="minute">Minutes when the game starts</param>
        /// <param name="memo">Memo of the player in the game</param>
        /// <param name="help">Help for the player</param>
        public Game(string playerName, Room currentRoom, int hour, int minute, string memo, string help)
        {
            Timestamp = DateTime.Now.ToString("yyyyMMddHHmmssff");
            PlayerName = playerName;
            CurrentRoom = CurrentRoom;
            _currentDate = new CluedoDate(hour, minute);

            _inventory = new List<ICollectible>();
            _rooms = new List<Room>();
            _memo = new List<ConsoleString>();

            Sentences = new CluedoSentences();
            _help = help;

            _daughterArrived = false;
        }

        /// <summary>
        /// Partial constructor
        /// </summary>
        /// <param name="help">Help for the player</param>
        public Game(string help) : this("", null, Game.StartHour, Game.StartMinute, "", help) { }

        #endregion
    }
}

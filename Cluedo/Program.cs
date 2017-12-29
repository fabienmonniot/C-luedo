using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cluedo
{
    /// <summary>
    /// String with colors
    /// </summary>
    [Serializable]
    struct ConsoleString
    {
        public string text;
        public ConsoleColor color;

        public override string ToString()
        {
            return text;
        }

        public ConsoleString(string init_text, ConsoleColor init_color)
        {
            text = init_text;
            color = init_color;
        }

        public ConsoleString(string init_text) : this(init_text, Program.narratorColor) { }
    }

    /// <summary>
    /// Hour and minute of the playing day
    /// </summary>
    [Serializable]
    struct CluedoDate
    {
        public int hour;
        public int minute;

        public override string ToString()
        {
            return hour.ToString("00") + ":" + minute.ToString("00");
        }

        public CluedoDate(int h, int min)
        {
            hour = (h > Game.StartHour) ? h : Game.StartHour;
            minute = min;
        }
    }

    /// <summary>
    /// Suspect and Weapon of the crime
    /// </summary>
    [Serializable]
    struct CluedoKiller
    {
        public Suspect suspect;
        public Weapon weapon;

        public CluedoKiller(Suspect init_suspect, Weapon init_weapon)
        {
            suspect = init_suspect;
            weapon = init_weapon;
        }
    }

    /// <summary>
    /// Sentences for i18n
    /// </summary>
    [Serializable]
    struct CluedoSentences
    {
        public string GetLastGame;
        public string ScriptTitle;
        public string ChooseScript;
        public string AskNumber;
        public string YesOrNo;
        public string Replay;
        public string AccuseKiller;
        public string AccuseWeapon;
        public string MemoTitle;
        public string HelpTitle;
        public string ChangeRoom;
        public string KnowMore;
        public string TrapDoor;
        public string PolicemanEnd;
        public string WhichAction;

        //Actions
        public string ObserveAction;
        public string CollectAction;
        public string SpecialObserveAction;
        public string TalkAction;
        public string SpecialTalkAction;
        public string AccuseAction;
        public string ReadMemoAction;
        public string HelpAction;
        public string MoveAction;
        public string NoneAction;
    }

    class Program
    {
        #region Static attributes

        /// <summary>
        /// Rootpath of the application
        /// </summary>
        public static readonly string RootPath = Directory.GetParent("..\\..\\").FullName;

        /// <summary>
        /// Path of the scripts
        /// </summary>
        public static readonly string ScriptsPath = RootPath + "\\data\\script\\";

        /// <summary>
        /// Path of the game script file
        /// </summary>
        public static readonly string ScriptGamePath = "\\game.xml";

        /// <summary>
        /// Path of the rooms script folder
        /// </summary>
        public static readonly string ScriptRoomsPath = "\\Rooms\\";

        /// <summary>
        /// Path of the saves
        /// </summary>
        public static readonly string SavesPath = RootPath + "\\data\\save\\";

        /// <summary>
        /// Path for the configuration file (sentences)
        /// </summary>
        public static readonly string ConfigPath = ScriptsPath + "\\Config.xml";

        /// <summary>
        /// Color of the text of the NPCs talks
        /// </summary>
        public static readonly ConsoleColor talkColor = ConsoleColor.White;

        /// <summary>
        /// Color of the text of the narrator
        /// </summary>
        public static readonly ConsoleColor narratorColor = ConsoleColor.Gray;

        /// <summary>
        /// Default sentences (with no game started)
        /// </summary>
        protected static CluedoSentences _sentences = new CluedoSentences();

        /// <summary>
        /// The game in play
        /// </summary>
        protected static Game _game = null;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the last game played
        /// </summary>
        public static void GetLastGame()
        {
            Directory.CreateDirectory(SavesPath);
            var fullPathSaves = Directory.GetFiles(SavesPath);

            //If at least one save
            if (fullPathSaves.Length > 0)
            {
                //Get the last one
                string[] fullPathSave = fullPathSaves[fullPathSaves.Length - 1].Split('\\');
                string save = fullPathSave[fullPathSave.Length - 1];

                Game game = deserializeSave(SavesPath + save);

                //If the game isn't finished
                if (!game.End)
                {
                    string year = save.Substring(0, 4);
                    string month = save.Substring(4, 2);
                    string day = save.Substring(6, 2);
                    string hours = save.Substring(8, 2);
                    string mins = save.Substring(10, 2);
                    string secs = save.Substring(12, 2);

                    string question = _sentences.GetLastGame;
                    question = question.Replace("[DAY]", day);
                    question = question.Replace("[MONTH]", month);
                    question = question.Replace("[YEAR]", year);
                    question = question.Replace("[HOUR]", hours);
                    question = question.Replace("[MINUTE]", mins);
                    question = question.Replace("[SECOND]", secs);

                    if (YesOrNo(question))
                    {
                        _game = game;
                    }
                }
            }
        }

        /// <summary>
        /// Get a game from a save file
        /// </summary>
        /// <param name="filePath">Absolute path of the save file</param>
        /// <returns>The game</returns>
        public static Game deserializeSave(string filePath)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var game = (Game)formatter.Deserialize(stream);
            stream.Close();

            return game;
        }

        /// <summary>
        /// Asks the player to choose one of the scripts available
        /// </summary>
        /// <returns>The game choosen</returns>
        public static void ChooseScript()
        {
            //Get all scripts
            var fullPathScripts = Directory.GetDirectories(ScriptsPath);
            var scripts = new string[fullPathScripts.Length];

            for (int i = 0; i < fullPathScripts.Length; i++)
            {
                string[] paths = fullPathScripts[i].Split('\\');
                scripts[i] = paths[paths.Length - 1];
            }

            //Choose one
            ColorWriteLine("");

            DisplayTitle(_sentences.ScriptTitle, 10);
            int iScript = AskPlayer(_sentences.ChooseScript, scripts);

            //Build the game from it
            _game = buildGameFromScriptFiles(ScriptsPath + scripts[iScript]);
        }

        /// <summary>
        /// Build a game from a script file
        /// </summary>
        /// <param name="filePath">Absolute path of the script folder</param>
        /// <returns>The game built</returns>
        public static Game buildGameFromScriptFiles(string filePath)
        {
            //Get game and rooms files
            var gameXml = XDocument.Load(filePath + ScriptGamePath);
            string[] roomsDir = Directory.GetFiles(filePath + ScriptRoomsPath);
            var roomsXml = new XDocument[roomsDir.Length];

            for (int i = 0; i < roomsDir.Length; i++)
            {
                roomsXml[i] = XDocument.Load(roomsDir[i]);
            }

            //Creates the game
            Game game = new Game(XmlGetFirstValue(gameXml.Descendants("Help").Descendants("Content")));
            game.Sentences = XmlGetSentences(gameXml);

            string killerSuspectName = XmlGetFirstValue(gameXml.Descendants("Killer").Descendants("Suspect").Descendants("Name"));
            string killerWeaponName = XmlGetFirstValue(gameXml.Descendants("Killer").Descendants("Weapon").Descendants("Name"));
            Suspect killerSuspect = null;
            Weapon killerWeapon = null;

            //Saves items & NPCs for relations after
            var listItems = new List<Item>();
            var listNPCs = new List<NPC>();
            var listRooms = new List<Room>();
            string startRoom = XmlGetFirstValue(gameXml.Descendants("StartRoom").First().Descendants("Name"));

            //Creates and adds each room to the game
            InitRooms(game, listRooms, listItems, listNPCs, roomsXml);

            //Add relations between classes
            InitRelationships(game, listRooms, listItems, listNPCs, roomsXml, startRoom, killerSuspectName, killerWeaponName, ref killerSuspect, ref killerWeapon);

            return game;
        }

        /// <summary>
        /// Gets the sentences from the game
        /// </summary>
        /// <param name="gameXml">The XDocument of the game</param>
        /// <returns>The sentences</returns>
        public static CluedoSentences XmlGetSentences(XDocument gameXml)
        {
            var sentences = new CluedoSentences();

            //Sentences
            var sentencesXml = gameXml.Descendants("Sentences");
            sentences.HelpTitle = XmlGetFirstValue(gameXml.Descendants("Help").Descendants("Title"));
            sentences.AskNumber = XmlGetFirstValue(sentencesXml.Descendants("AskNumber"));
            sentences.YesOrNo = XmlGetFirstValue(sentencesXml.Descendants("YesOrNo"));
            sentences.Replay = XmlGetFirstValue(sentencesXml.Descendants("Replay"));
            sentences.AccuseKiller = XmlGetFirstValue(sentencesXml.Descendants("AccuseKiller"));
            sentences.AccuseWeapon = XmlGetFirstValue(sentencesXml.Descendants("AccuseWeapon"));
            sentences.MemoTitle = XmlGetFirstValue(sentencesXml.Descendants("MemoTitle"));
            sentences.ChangeRoom = XmlGetFirstValue(sentencesXml.Descendants("ChangeRoom"));
            sentences.KnowMore = XmlGetFirstValue(sentencesXml.Descendants("KnowMore"));
            sentences.TrapDoor = XmlGetFirstValue(sentencesXml.Descendants("TrapDoor"));
            sentences.PolicemanEnd = XmlGetFirstValue(sentencesXml.Descendants("PolicemanEnd"));
            sentences.WhichAction = XmlGetFirstValue(sentencesXml.Descendants("WhichAction"));

            //Actions
            var actionsXml = sentencesXml.Descendants("Actions");
            sentences.AccuseAction = XmlGetFirstValue(actionsXml.Descendants("Accuse"));
            sentences.CollectAction = XmlGetFirstValue(actionsXml.Descendants("Collect"));
            sentences.HelpAction = XmlGetFirstValue(actionsXml.Descendants("Help"));
            sentences.ObserveAction = XmlGetFirstValue(actionsXml.Descendants("Observe"));
            sentences.MoveAction = XmlGetFirstValue(actionsXml.Descendants("Move"));
            sentences.NoneAction = XmlGetFirstValue(actionsXml.Descendants("None"));
            sentences.ReadMemoAction = XmlGetFirstValue(actionsXml.Descendants("ReadMemo"));
            sentences.SpecialObserveAction = XmlGetFirstValue(actionsXml.Descendants("SpecialObserve"));
            sentences.SpecialTalkAction = XmlGetFirstValue(actionsXml.Descendants("SpecialTalk"));
            sentences.TalkAction = XmlGetFirstValue(actionsXml.Descendants("Talk"));

            return sentences;
        }

        /// <summary>
        /// Initiates the rooms of the game
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="listRooms">A list of all the rooms</param>
        /// <param name="listItems">A list of all the items</param>
        /// <param name="listNPCs">A list of all the NPCs</param>
        /// <param name="roomsXml">The XDocuments of the rooms</param>
        public static void InitRooms(Game game, List<Room> listRooms, List<Item> listItems, List<NPC> listNPCs, XDocument[] roomsXml)
        {
            foreach (var roomXml in roomsXml)
            {
                //Room name
                var roomName = new ConsoleString(roomXml.Root.Element("Name").Value.Trim());

                //Room description
                var roomDescription = ConvertToLCS(roomXml.Root.Element("Description").Value.Trim());

                //Creates room
                var room = new Room(roomName, roomDescription);
                game.AddRooms(room);
                listRooms.Add(room);

                //Add items to room
                foreach (var itemXml in roomXml.Descendants("Items").Elements())
                {
                    //Item name
                    var itemName = new ConsoleString(XmlGetFirstValue(itemXml.Descendants("Name")));

                    //Item description
                    var itemDescription = ConvertToLCS(XmlGetFirstValue(itemXml.Descendants("Description")));

                    //Extra-time
                    int itemExtraTime = 0;
                    string sExtraTime = XmlGetFirstValue(itemXml.Descendants("ExtraTime"));
                    if (!String.IsNullOrEmpty(sExtraTime))
                    {
                        itemExtraTime = int.Parse(sExtraTime);
                    }

                    //Item or Weapon ?
                    Item item;

                    if (itemXml.Name == "Weapon")
                        item = new Weapon(itemName, itemDescription, null);
                    else
                        item = new Item(itemName, itemDescription, false, itemExtraTime, false, null, null);

                    room.AddItems(item);
                    listItems.Add(item);
                }

                //Add NPCs to room
                foreach (var npcXml in roomXml.Descendants("NPCs").Elements())
                {
                    NPC npc;
                    //NPC name
                    var npcName = new ConsoleString(XmlGetFirstValue(npcXml.Descendants("Name")));

                    //Policeman
                    if (npcXml.Name == "Policeman")
                    {
                        var intro = TalksToLCS(roomXml.Descendants("Introduction"));
                        var reactions = TalksToLCS(roomXml.Descendants("Reactions"));
                        var rightAnswer = TalksToLCS(roomXml.Descendants("RightAnswer"));
                        var wrongAnswer = TalksToLCS(roomXml.Descendants("WrongAnswer"));
                        var daughterArriveTalk = TalksToLCS(roomXml.Descendants("DaughterArriveTalk"));


                        npc = new Policeman(npcName, reactions, daughterArriveTalk, rightAnswer, wrongAnswer, intro, false);
                        game.Policeman = (Policeman)npc;
                    }
                    else
                    {
                        //NPC
                        string npcTalk = XmlGetFirstValue(npcXml.Descendants("Talk"));
                        var npcAnswer = new List<ConsoleString>();
                        npcAnswer.Add(new ConsoleString(npcTalk.Insert(0, "\"").Insert(npcTalk.Length + 1, "\""), talkColor));

                        //Suspect
                        if (npcXml.Name == "Suspect")
                        {
                            var npcDescription = ConvertToLCS(XmlGetFirstValue(npcXml.Descendants("Description")));

                            //Extra observe time
                            int npcExtraObserveTime;
                            if (!int.TryParse(XmlGetFirstValue(npcXml.Descendants("ExtraObserveTime")), out npcExtraObserveTime))
                                npcExtraObserveTime = 0;

                            //Extra talk time
                            int npcExtraTalkTime;
                            if (!int.TryParse(XmlGetFirstValue(npcXml.Descendants("ExtraTalkTime")), out npcExtraTalkTime))
                                npcExtraTalkTime = 0;

                            string npcSpecialTalk = XmlGetFirstValue(npcXml.Descendants("SpecialTalk").Descendants("Talk"));
                            var npcSpecialAnswer = new List<ConsoleString>();
                            npcSpecialAnswer.Add(new ConsoleString(npcSpecialTalk.Insert(0, "\"").Insert(npcSpecialTalk.Length + 1, "\""), talkColor));

                            npc = new Suspect(npcName, npcAnswer, false, npcDescription, false, npcExtraTalkTime, npcExtraObserveTime, npcSpecialAnswer, false, null);
                        }
                        //Daughter
                        else if (npcXml.Name == "Daughter")
                        {
                            npc = new NPC(npcName, npcAnswer);
                            game.Daughter = npc;
                        }
                        else
                        {
                            npc = new NPC(npcName, npcAnswer);
                        }
                    }

                    room.AddNPCs(npc);
                    listNPCs.Add(npc);
                }
            }
        }

        /// <summary>
        /// Initiates the relationships between objects
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="listRooms">A list of all the rooms</param>
        /// <param name="listItems">A list of all the items</param>
        /// <param name="listNPCs">A list of all the NPCs</param>
        /// <param name="roomsXml">The XDocuments of the rooms</param>
        /// <param name="startRoom">The name of the start room</param>
        /// <param name="killerSuspectName">The name of the suspect</param>
        /// <param name="killerWeaponName">The name of the weapon</param>
        /// <param name="killerSuspect">The killer</param>
        /// <param name="killerWeapon">The weapon of the crime</param>
        public static void InitRelationships(
            Game game,
            List<Room> listRooms,
            List<Item> listItems,
            List<NPC> listNPCs,
            XDocument[] roomsXml,
            string startRoom,
            string killerSuspectName,
            string killerWeaponName,
            ref Suspect killerSuspect,
            ref Weapon killerWeapon)
        {
            int itemId = -1, roomId = -1, npcId = -1;
            foreach (var roomXml in roomsXml)
            {
                roomId++;

                //Add start room
                if (listRooms[roomId].Name.text == startRoom)
                    game.CurrentRoom = listRooms[roomId];

                //Add neighbors
                foreach (var neighborXml in roomXml.Descendants("Neighbors").Elements())
                {
                    string trapDoorName = XmlGetFirstValue(neighborXml.Descendants("TrapDoor"));

                    foreach (var room in listRooms)
                    {
                        string neightborName = XmlGetFirstValue(neighborXml.Descendants("Name"));

                        if (room.Name.text == neightborName)
                        {
                            listRooms[roomId].AddNeighbors(room);

                            if (!String.IsNullOrEmpty(trapDoorName))
                            {
                                listRooms[roomId].TrapDoor = room;
                            }
                        }


                    }
                }

                //Add relations for items requirements and weapon of the crime
                foreach (var itemXml in roomXml.Descendants("Items").Elements())
                {
                    itemId++;

                    //Add weapon requirement
                    if (itemXml.Name == "Weapon")
                    {
                        //Weapon of the crime ?
                        if (listItems[itemId].Name.text == killerWeaponName)
                            if (listItems[itemId] is Weapon)
                                killerWeapon = (Weapon)listItems[itemId];

                        //Requirements
                        string requirementName = XmlGetFirstValue(itemXml.Descendants("Requirement"));

                        //Search the item related
                        foreach (var item in listItems)
                        {
                            if (listItems[itemId] is Weapon)
                            {
                                if (item.Name.text == requirementName)
                                {
                                    //Add reference
                                    ((Weapon)listItems[itemId]).Requirement = item;
                                    break;
                                }
                            }
                        }
                    }

                    //Add special description and suspect related
                    string specialDescriptionTag = XmlGetFirstValue(itemXml.Descendants("SpecialDescription"));
                    if (!String.IsNullOrEmpty(specialDescriptionTag))
                    {
                        listItems[itemId].SpecialDescription = ConvertToLCS(XmlGetFirstValue(itemXml.Descendants("SpecialDescription").Descendants("Description")));
                        string requirementName = XmlGetFirstValue(itemXml.Descendants("SpecialDescription").Descendants("Requirement"));

                        //Search the item related
                        foreach (var npc in listNPCs)
                        {
                            if (npc.Name.text == requirementName)
                            {
                                //Add reference
                                if (npc is Suspect)
                                {
                                    listItems[itemId].SpecialInitiator = (Suspect)npc;
                                    break;
                                }
                            }
                        }
                    }
                }

                //Add relations for NPCs requirements
                foreach (var npcXml in roomXml.Descendants("NPCs").Elements())
                {
                    npcId++;

                    if (npcXml.Name == "Suspect")
                    {
                        //Killer ?
                        if (listNPCs[npcId].Name.text == killerSuspectName)
                            if (listNPCs[npcId] is Suspect)
                                killerSuspect = (Suspect)listNPCs[npcId];

                        //Special talk
                        string specialTalkTag = XmlGetFirstValue(npcXml.Descendants("SpecialTalk"));
                        if (!String.IsNullOrEmpty(specialTalkTag))
                        {
                            string requirementName = XmlGetFirstValue(npcXml.Descendants("SpecialTalk").Descendants("Requirement"));

                            if (listNPCs[npcId] is Suspect)
                                ((Suspect)listNPCs[npcId]).SpecialAnswer = ConvertToLCS(XmlGetFirstValue(npcXml.Descendants("SpecialTalk").Descendants("Talk")));

                            //Search the item related
                            foreach (var item in listItems)
                            {
                                if (item.Name.text == requirementName)
                                {
                                    //Add reference
                                    if (listNPCs[npcId] is Suspect && item is Weapon)
                                    {
                                        ((Suspect)listNPCs[npcId]).SpecialObject = (Weapon)item;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Set killer suspect
            game.setKiller(killerSuspect, killerWeapon);
        }

        /// <summary>
        /// Gets the value of the first element in the collection
        /// </summary>
        /// <param name="xmlDocs">The XElement collection</param>
        /// <returns>The value</returns>
        public static string XmlGetFirstValue(IEnumerable<XElement> xmlDocs)
        {
            string xmlValue = "";
            if (xmlDocs.Any())
            {
                xmlValue = xmlDocs.First().Value.Trim();
            }

            return xmlValue;
        }

        /// <summary>
        /// Converts Talks childs into a List of Console String
        /// </summary>
        /// <param name="xmlElement">The parent element</param>
        /// <returns>The colored list</returns>
        public static List<ConsoleString> TalksToLCS(IEnumerable<XElement> xmlElement)
        {
            var list = new List<ConsoleString>();

            foreach (var textXml in xmlElement.Elements())
            {
                string talk = textXml.Value.Trim();
                if (textXml.Name == "Talk")
                {
                    talk = talk.Insert(0, "\"").Insert(talk.Length + 1, "\"");
                    list.Add(new ConsoleString(talk, talkColor));
                }
                else
                {
                    list.Add(new ConsoleString(talk));
                }
            }

            return list;
        }

        /// <summary>
        /// Writes in the console with colors
        /// </summary>
        /// <param name="stringObject">The string, ConsoleString or List<ConsoleString> to display</param>
        public static void ColorWriteLine(Object stringObject)
        {
            List<ConsoleString> list = ConvertToLCS(stringObject);

            foreach (var consoleString in list)
            {
                Console.ForegroundColor = consoleString.color;
                string text = consoleString.text.Replace("\t", "");

                //Replaces shortcodes
                if (_game != null)
                {
                    if (text.Contains("[ASK_NAME]") && String.IsNullOrEmpty(_game.PlayerName))
                    {
                        string[] texts = text.Split(new string[] { "[ASK_NAME]" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < texts.Length; i++)
                        {
                            Console.WriteLine(text.Replace("[ASK_NAME]", ""));
                            if (i == 0)
                            {
                                _game.PlayerName = Console.ReadLine();
                            }
                        }
                    }
                    else
                    {
                        text = text.Replace("[ASK_NAME]", _game.PlayerName);
                        text = text.Replace("[PRINT_NAME]", _game.PlayerName);
                        Console.WriteLine(text);
                    }
                }
                else
                {
                    Console.WriteLine(text);
                }
            }
        }

        /// <summary>
        /// Convert any kind of object to a list of console strings
        /// </summary>
        /// <param name="stringObject">Object to convert</param>
        /// <returns>A list of console strings to display</returns>
        public static List<ConsoleString> ConvertToLCS(Object stringObject)
        {
            var list = new List<ConsoleString>();

            if (stringObject is List<ConsoleString>)
                list = (List<ConsoleString>)stringObject;
            else if (stringObject is ConsoleString)
                list.Add((ConsoleString)stringObject);
            else
                list.Add(new ConsoleString(stringObject.ToString()));

            return list;
        }

        /// <summary>
        /// Creates a new list with a new first line
        /// </summary>
        /// <param name="lcs">Original list</param>
        /// <param name="line">Line to add</param>
        /// <returns>The new list</returns>
        public static List<ConsoleString> AddNewLine(List<ConsoleString> lcs, ConsoleString line)
        {
            var newList = new List<ConsoleString>();
            newList.Add(line);
            foreach (var cs in lcs)
                newList.Add(cs);

            return newList;
        }

        /// <summary>
        /// Asks the player for a list of choices
        /// </summary>
        /// <param name="sQuestion">The question to ask</param>
        /// <param name="aItems">Choices to display</param>
        /// <returns>The one-based number choosen</returns>
        public static int AskPlayer(string sQuestion, Object[] aItems)
        {
            int iChoosen, iNbItems = aItems.Length;
            bool bError = false;

            do
            {
                ColorWriteLine("");
                for (int i = 0; i < iNbItems; i++)
                {
                    if (aItems[i].ToString().Contains("\n"))
                        ColorWriteLine("");
                    ColorWriteLine(String.Format("{0} - {1}", i + 1, aItems[i]).ToString().Replace("\n", ""));
                }

                if (bError)
                {
                    string question = (_game == null) ? _sentences.AskNumber : _game.Sentences.AskNumber;

                    question = question.Replace("[X]", "1").Replace("[Y]", iNbItems.ToString());
                    ColorWriteLine(question);
                }

                iChoosen = -1;
                bError = false;
                try
                {
                    ColorWriteLine("\n" + sQuestion + " ");
                    iChoosen = int.Parse(Console.ReadLine()) - 1;
                }
                catch (FormatException e)
                {
                    bError = true;
                }

                if (iChoosen < 0 || iChoosen >= iNbItems)
                {
                    bError = true;
                }
            } while (bError);

            return iChoosen;
        }

        /// <summary>
        /// Displays a title with borders
        /// </summary>
        /// <param name="sTitle">The title</param>
        /// <param name="iPlus">The number of white spaces (before+after)</param>
        public static void DisplayTitle(string sTitle, int iPlus)
        {
            int iLength = sTitle.Length + iPlus;
            DisplayBorderLine(1, iLength);

            Console.Write("|");
            for (int j = 0; j < iLength + 2; j++)
            {
                Console.Write(" ");
            }
            Console.Write("|\n");

            //Displays the title
            Console.Write("| ");
            for (int j = 0; j < iPlus / 2; j++)
            {
                Console.Write(" ");
            }
            Console.Write(sTitle);
            for (int j = 0; j < iPlus / 2; j++)
            {
                Console.Write(" ");
            }
            Console.Write(" |\n");

            Console.Write("|");
            for (int j = 0; j < iLength + 2; j++)
            {
                Console.Write(" ");
            }
            Console.Write("|\n");

            DisplayBorderLine(1, iLength);

            Console.Write("\n");
        }

        /// <summary>
        /// Display a border line of the grid
        /// </summary>
        /// <param name="iNbCol">Number of columns</param>
        /// <param name="iColWidth">Width of a column</param>
        public static void DisplayBorderLine(int iNbCol, int iColWidth)
        {
            for (int j = 0; j < iNbCol; j++)
            {
                Console.Write(" ");
                for (int k = 0; k < iColWidth + 2; k++)
                {
                    Console.Write("-");
                }
            }
            Console.Write(" \n");
        }

        /// <summary>
        /// Asks the user a true/false question
        /// </summary>
        /// <param name="sText">Question displayed to the user</param>
        /// <returns>The answer choosen</returns>
        public static bool YesOrNo(string sText)
        {
            string yesOrNo = (_game == null) ? _sentences.YesOrNo : _game.Sentences.YesOrNo;

            ColorWriteLine(String.Format("\n{0} {1} ", sText, yesOrNo));
            ConsoleKeyInfo oResponse = Console.ReadKey();
            Console.Write("\n");

            return (oResponse.Key == ConsoleKey.Enter || oResponse.Key == ConsoleKey.O || oResponse.Key == ConsoleKey.Y);
        }

        /// <summary>
        /// Displays Cluedo Title
        /// </summary>
        public static void DisplayCluedoTitle()
        {
            ColorWriteLine(new ConsoleString(" ###          #                     #", ConsoleColor.White));
            ColorWriteLine(new ConsoleString("#   #   # #   #                     #", ConsoleColor.White));
            ColorWriteLine(new ConsoleString("#      #####  #  #   #   ###    #####   ###", ConsoleColor.White));
            ColorWriteLine(new ConsoleString("#       # #   #  #   #  #   #  #    #  #   #", ConsoleColor.White));
            ColorWriteLine(new ConsoleString("#      #####  #  #   #  #####  #    #  #   #", ConsoleColor.White));
            ColorWriteLine(new ConsoleString("#   #   # #   #  #   #  #      #    #  #   #", ConsoleColor.White));
            ColorWriteLine(new ConsoleString(" ###          #   ####   ###    #####   ###", ConsoleColor.White));
            ColorWriteLine("");
        }

        /// <summary>
        /// Get the sentences to launch the game
        /// </summary>
        public static void GetConfig()
        {
            var configXml = XDocument.Load(ConfigPath);

            var sentencesXml = configXml.Descendants("Sentences");
            _sentences.GetLastGame = XmlGetFirstValue(configXml.Descendants("GetLastGame"));
            _sentences.ScriptTitle = XmlGetFirstValue(configXml.Descendants("ScriptTitle"));
            _sentences.ChooseScript = XmlGetFirstValue(configXml.Descendants("ChooseScript"));
            _sentences.AskNumber = XmlGetFirstValue(configXml.Descendants("AskNumber"));
            _sentences.YesOrNo = XmlGetFirstValue(configXml.Descendants("YesOrNo"));
        }

        #endregion

        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args">Console parameters</param>
        static void Main(string[] args)
        {
            bool replay = true;

            //Init console
            Console.WindowWidth = 155;
            Console.WindowHeight = 50;
            Console.Title = "C#luedo";
            DisplayCluedoTitle();

            //Config file (sentences)
            GetConfig();

            //Gets the last game
            GetLastGame();

            do
            {
                //Or creates a new one
                if (_game == null)
                {
                    ChooseScript();

                    //Introduction
                    _game.Policeman.TalkIntro();

                    _game.Save();
                }

                //Plays the game
                do
                {
                    _game.Play();
                    _game.Save();
                } while (!_game.End);

                //End words
                if (_game.Won)
                {
                    _game.Policeman.TalkGoodAnswer();
                    Console.ReadKey();
                    replay = false;
                }
                else
                {
                    _game.Policeman.TalkBadAnswer();
                    replay = YesOrNo(_game.Sentences.Replay);
                    _game = null;
                    Console.Clear();
                }
            } while (replay);
        }
    }
}

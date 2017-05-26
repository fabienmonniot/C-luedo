using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    [Serializable]
    class Room
    {
        #region Attributes & Properties

        /// <summary>
        /// The game of the room
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// The name of the room
        /// </summary>
        public ConsoleString Name { get; protected set; }

        /// <summary>
        /// The description of the room
        /// </summary>
        public List<ConsoleString> Description { get; set; }

        /// <summary>
        /// The trapdoor of the room
        /// </summary>
        public Room TrapDoor { get; set; }

        /// <summary>
        /// The neighbor rooms of the room
        /// </summary>
        protected List<Room> _neighbors;

        /// <summary>
        /// The items that are in the room
        /// </summary>
        protected List<Item> _items;

        /// <summary>
        /// The NPCs that are in the room
        /// </summary>
        protected List<NPC> _npcs;

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets the items of the room
        /// </summary>
        /// <returns>The items</returns>
        public Item[] GetItems()
        {
            return _items.ToArray();
        }

        /// <summary>
        /// Gets the NPCs of the room
        /// </summary>
        /// <returns>The NPCs</returns>
        public NPC[] GetNPCs()
        {
            return _npcs.ToArray();
        }

        /// <summary>
        /// Gets the neighbor rooms of the room
        /// </summary>
        /// <returns>The neighbor rooms</returns>
        public Room[] GetNeighbors()
        {
            return _neighbors.ToArray();
        }

        /// <summary>
        /// Adds items to the items of the room
        /// </summary>
        /// <param name="items">The items to add</param>
        /// <returns>The room</returns>
        public Room AddItems(params Item[] items)
        {
            foreach (Item item in items)
            {
                _items.Add(item);
                item.Room = this;
            }

            return this;
        }

        /// <summary>
        /// Adds NPCs to the room
        /// </summary>
        /// <param name="items">The NPCs to add</param>
        /// <returns>The room</returns>
        public Room AddNPCs(params NPC[] npcs)
        {
            foreach (NPC npc in npcs)
            {
                _npcs.Add(npc);
                npc.Room = this;
            }

            return this;
        }

        /// <summary>
        /// Adds neighbor rooms to the room
        /// </summary>
        /// <param name="items">The neighbor rooms to add</param>
        /// <returns>The room</returns>
        public Room AddNeighbors(params Room[] rooms)
        {
            foreach (var room in rooms)
            {
                _neighbors.Add(room);
            }

            return this;
        }

        /// <summary>
        /// Checks if the room is a neighbor
        /// </summary>
        /// <param name="room">The room</param>
        /// <returns>The boolean associated</returns>
        public bool HasNeighbor(Room room)
        {
            return (_neighbors.Contains(room));
        }

        /// <summary>
        /// Overrides the method ToString()
        /// </summary>
        /// <returns>The name of the room</returns>
        public override string ToString()
        {
            return Name.text;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="name">Name of the room</param>
        /// <param name="description">Description of the room</param>
        /// <param name="trapDoor">Trapdoor of the room</param>
        public Room(ConsoleString name, List<ConsoleString> description, Room trapDoor)
        {
            Name = name;
            Description = description;
            TrapDoor = trapDoor;

            _items = new List<Item>();
            _neighbors = new List<Room>();
            _npcs = new List<NPC>();
        }

        /// <summary>
        /// Constructor for a room that hasn't a trapdoor
        /// </summary>
        /// <param name="name">Name of the room</param>
        /// <param name="description">Description of the room</param>
        public Room(ConsoleString name, List<ConsoleString> description)
            : this(name, description, null)
        {

        }

        #endregion

    }
}

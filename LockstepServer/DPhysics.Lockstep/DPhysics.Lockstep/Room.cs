using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
namespace DPhysics.Lockstep
{
    /// <summary>
    /// Represents a room where every player receives the same input.
    /// In other words, every player in a room plays the same game.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Defines the maximum amount of players that can be in a room.
        /// Once this amount is met, the room starts and no more players can join.
        /// </summary>
        public static int MaxPlayers = 1;
        
        /// <summary>
        /// Constructs a Room and sets its identification number as ID.
        /// </summary>
        /// <param name="ID"></param>
        public Room(ushort ID)
        {
            RoomID = ID;
        }
        /// <summary>
        /// The players in this room.
        /// </summary>
        public List<Player> MyPlayers = new List<Player>();
        /// <summary>
        /// The package to send the next frame.
        /// </summary>
        public List<byte> CurrentFrame = new List<byte>(20);
        /// <summary>
        /// Describes whether or not this Room has started yet.
        /// </summary>
        public bool Started = false;
        /// <summary>
        /// Describes the amount of joined players.
        /// </summary>
        public int JoinedCount = 0;
        /// <summary>
        /// Similar to the step count client-side. This value gets serialized and describes which frame a game packet is for.
        /// </summary>
        private ushort StepCount = 0;
        /// <summary>
        /// This Room's unique ID. Used for accessing the Room in Core.AllRooms.
        /// </summary>
        public ushort RoomID;
        /// <summary>
        /// Simulates the room, distributing information to players as necessary and performing any Room logic.
        /// </summary>
        public void Simulate ()
        {
            if (Started)
            {
                DistributeFrame();
                StepCount++;
            }
        }

        /// <summary>
        /// A player's game data gets routed to his room here.
        /// </summary>
        /// <param name="data"></param>
        public void ProcessInformation (byte[] data)
        {
            CurrentFrame.AddRange(data);
        }
        /// <summary>
        /// Distributes a frame to every player in the Room.
        /// </summary>
        public void DistributeFrame ()
        {
            foreach (Player player in MyPlayers)
            {
                player.MyConnection.SendReply(1, StepCount, CurrentFrame.ToArray());
            }
            CurrentFrame.Clear();
        }

        /// <summary>
        /// Returns true if the room is full and can start, false if it still needs more players.
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public bool AddPlayer (Player player)
        {
            player.MyRoomID = RoomID;
            MyPlayers.Add(player);

                JoinedCount++;
                if (JoinedCount == MaxPlayers)
                {
                    Started = true;
                }
                else
                {
                    return false;
                }
                return true;
        }
        /// <summary>
        /// Returns true if room has no players and can end, false if it still has players
        /// </summary>
        /// <returns></returns>
        public bool RemovePlayer (Player player)
        {
            player.MyRoomID = 0;
            MyPlayers.Remove(player);
        
            JoinedCount--;
            if (JoinedCount == 0) return true;
            return false;
        }
    }
}

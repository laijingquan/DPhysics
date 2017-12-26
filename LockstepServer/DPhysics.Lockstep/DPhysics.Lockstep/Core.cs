using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
using System.Timers;
namespace DPhysics.Lockstep
{
    public class Core : Plugin
    {
        #region Meta Information
        public override string name
        {
            get { return "DPhysics Lockstep Server"; }
        }
        public override string version
        {
            get { return "1.0"; }
        }
        public override string author
        {
            get { return "John Pan"; }
        }
        public override string supportEmail
        {
            get { return "JPtheK9@gmail.com"; }
        }
        public override Command[] commands
        {
            get { return new Command[0]; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// This Timer is responsible for advancing the server along with clients.
        /// </summary>
        public static Timer GlobalTimer;
        /// <summary>
        /// The rate at which the server runs its logic, in milliseconds.
        /// The server's rate should be synced with clients' simulation rate.
        /// </summary>
        public const double SimulationRate = 100;
        public Core()
        {
            GlobalTimer = new Timer(SimulationRate);
            GlobalTimer.Elapsed += FixedUpdate;
            GlobalTimer.Start();
            ConnectionService.onDataDecoded += ConnectionService_onDataDecoded;
            ConnectionService.onPlayerConnect += ConnectionService_onPlayerConnect;
            ConnectionService.onPlayerDisconnect += ConnectionService_onPlayerDisconnect;
        }
        #endregion

        #region Player Logic
        /// <summary>
        /// All connected players
        /// </summary>
        public Dictionary<ConnectionService, Player> Connections = new Dictionary<ConnectionService, Player>();
        /// <summary>
        /// A list of all players connected to the server.
        /// </summary>
        public List<Player> Players = new List<Player>();

        void ConnectionService_onPlayerConnect(ConnectionService con)
        {
            //Create a player instance
            Player player = new Player(con);
            Players.Add(player);
            Connections.Add(con, player);
            //Try to find a room for the player. If no room is found, create one.
            Room room;
            if (SearchingRooms.Count > 0)
            {
                room = SearchingRooms.First();
            }
            else
            {
                room = new Room(IDGen.GenerateRoomID());
                AllRooms.Add(room.RoomID, room);
                SearchingRooms.Add(room); 
            }
            if (room.AddPlayer(player))
            {
                SearchingRooms.Remove(room);
            }
        }

        void ConnectionService_onPlayerDisconnect(ConnectionService con)
        {
            //Remove player if it disconnects
            Player player = Connections[con];
            if (player.MyRoomID != 0)
            {
               //Remove player from room; if room has no more players, remove the room
               Room room = AllRooms[player.MyRoomID];
               if (room.RemovePlayer(player))
               {
                   AllRooms.Remove(room.RoomID);
                   SearchingRooms.Remove(room);
               }
            }
            Connections.Remove(con);
            Players.Remove(player);

        }

        void ConnectionService_onDataDecoded(ConnectionService con, ref NetworkMessage data)
        {
            //Processing data sent from a player.
            Interface.Log("Data Received");
            switch(data.tag)
            {
                case 0:
                    
                break;
                case 1:
                    //Getting the player's instance
                    Player source = Connections[con];
                    //Finding the player's room and routing the data to that room
                    AllRooms[source.MyRoomID].ProcessInformation((byte[])data.data);
                    Interface.Log("Data Roomed");
                break;
            }
        }
        #endregion

        #region Room Logic
        public static Dictionary<ushort,Room> AllRooms = new Dictionary<ushort,Room>();
        public static HashSet<Room> SearchingRooms = new HashSet<Room>();
        void FixedUpdate(object sender, ElapsedEventArgs e)
        {
           foreach (Room room in AllRooms.Values)
           {
               room.Simulate();
           }
        }
        #endregion
    }
}

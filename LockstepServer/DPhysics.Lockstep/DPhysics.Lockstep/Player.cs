using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
namespace DPhysics.Lockstep
{
    /// <summary>
    /// Represents a networked player.
    /// </summary>
    public class Player
    {
        public Player (ConnectionService con)
        {
            MyConnection = con;
        }
        public ConnectionService MyConnection;
        public ushort MyRoomID = 0;
    }
}

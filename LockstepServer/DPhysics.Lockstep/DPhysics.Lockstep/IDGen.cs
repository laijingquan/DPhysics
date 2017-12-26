using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPhysics.Lockstep
{
    /// <summary>
    /// Used for generating room IDs, and potentially IDs for other objects.
    /// </summary>
    public static class IDGen
    {
        private static ushort NextRoomID = 0;
        private static Stack<ushort> CachedRoomID = new Stack<ushort>();
        /// <summary>
        /// Get a unique RoomID.
        /// </summary>
        /// <returns></returns>
        public static ushort GenerateRoomID()
        {
            if (CachedRoomID.Count > 0)
            {
                return CachedRoomID.Pop();
            }
            NextRoomID++;
            return NextRoomID;
        }
        /// <summary>
        /// When a room no longer uses its ID, this method recycles it.
        /// </summary>
        /// <param name="id"></param>
        public static void CacheRoomdID (ushort id)
        {
            CachedRoomID.Push(id);
        }
    }
}

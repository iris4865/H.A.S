using System;
using System.Collections.Generic;

namespace Server
{
    public class RoomList
    {
        static readonly Lazy<RoomList> instance = new Lazy<RoomList>(() => new RoomList());
        public static RoomList Instance => instance.Value;

        Dictionary<int, RoomInfo> list = new Dictionary<int, RoomInfo>();

        public RoomInfo this[int index] => GetRoom(index);

        RoomList() { }

        readonly object syncObj = new object();
        RoomInfo GetRoom(int index)
        {
            if (!list.ContainsKey(index))
            {
                lock (syncObj)
                {
                    list[index] = new RoomInfo();
                }
            }
            return list[index];
        }
    }
}

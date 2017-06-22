using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Server
{
    public class NPCManager
    {
        List<int> destination;
        int milliSecond = 5000;
        Timer timerEvent;
        int npcTotalNumber;
        public Action<Packet> Send;

        public NPCManager(int npcNumber)
        {
            npcTotalNumber = npcNumber;

            destination = new List<int>();
            for (int i = 0; i < npcNumber; i++)
                destination.Add(0);

            timerEvent = new Timer(milliSecond);
            timerEvent.Elapsed += SendRandomDestination;
        }

        public void Start() => timerEvent.Enabled = true;
        public void Stop() => timerEvent.Enabled = false;

        void SendRandomDestination(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < npcTotalNumber; i++)
            {
                if(new Random().Next(0, 2) == 1)
                    destination[i] = new Random().Next(0, destination.Count);
            }

            Packet msg = PacketBufferManager.Instance.Pop(PROTOCOL.NPCPosition);

            foreach(var num in destination)
                msg.Push(num);

            Send(msg);
        }
    }
}

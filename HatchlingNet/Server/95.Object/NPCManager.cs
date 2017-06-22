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
        int milliSecond = 100;
        Timer timerEvent;
        int npcTotalNumber;
        public Action<Packet> Send;

        public NPCManager(int npcNumber)
        {
            npcTotalNumber = npcNumber;

            destination = new List<int>();
            for (int i = 0; i < npcTotalNumber; i++)
                destination.Add(0);
            
            timerEvent = new Timer();
            timerEvent.Interval = milliSecond;
            timerEvent.Elapsed += SendRandomDestination;
        }

        public void Start() => timerEvent.Enabled = true;
        public void Stop() => timerEvent.Enabled = false;

        void SendRandomDestination(object sender, ElapsedEventArgs e)
        {
            Random ran = new Random();
            timerEvent.Interval = ran.Next(1, 6) * 1000;
            for (int i = 0; i < npcTotalNumber; i++)
            {
                if(ran.Next(0, 5) == 0)
                    destination[i] = ran.Next(0, destination.Count);
                else
                    destination[i] = -1;
            }

            Packet msg = PacketBufferManager.Instance.Pop(PROTOCOL.NPCPosition);

            foreach(var num in destination)
                msg.Push(num);

            Send(msg);
        }
    }
}

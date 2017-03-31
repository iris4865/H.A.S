using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomServer
{
    class GameServer
    {
        object operationLock;
        Queue<Packet> userOperation;

        //로직은 하나의 쓰레드로만 처리
        Thread logicThread;
        AutoResetEvent flowController;


        public GameServer()
        {
            this.operationLock = new object();
            this.flowController = new AutoResetEvent(false);
            this.userOperation = new Queue<Packet>();

            this.logicThread = new Thread(Update);
            this.logicThread.Start();
        }

        void Update()
        {
            while (true)
            {
                Packet packet = null;

                lock (this.operationLock)
                {
                    if (this.userOperation.Count > 0)
                    {
                        packet = this.userOperation.Dequeue();
                    }
                }

                if (packet != null)
                {
                    ProcessReceive(packet);
                }

                if (this.userOperation.Count <= 0)//패킷큐에 아무것도 없을경우
                {                                 //어딘가에서 flowController를
                                                //Set해줄때까지 대기
                    this.flowController.WaitOne();
                }

            }
        }


        public void EnqueuePacket(Packet packet, GameUser user)
        {
            lock (this.operationLock)
            {
                this.userOperation.Enqueue(packet);
                this.flowController.Set();
            }
        }

        void ProcessReceive(Packet msg)
        {
            msg.owner.ProcessUserOperation(msg);
        }

    }
}

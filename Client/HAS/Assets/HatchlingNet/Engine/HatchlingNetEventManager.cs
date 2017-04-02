using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HatchlingNet
{
    /// <summary>
    /// 네트워크 엔진에서 발생된 이벤트들을 큐잉시킨다.
    /// 워커 스레드와 메인 스레드 양쪽에서 호출될 수 있으므로 스레드 동기화 처리를 적용하였다.
    /// </summary>
    public class HatchlingNetEventManager
    {
        // 동기화 객체.
        object csEvent;

        // 네트워크 엔진에서 발생된 이벤트들을 보관해놓는 큐.
        Queue<NETWORK_EVENT> networkEventQueue;

        // 서버에서 받은 패킷들을 보관해놓는 큐.
        Queue<Packet> networkMessageQueue;

        public HatchlingNetEventManager()
        {
            this.networkEventQueue = new Queue<NETWORK_EVENT>();
            this.networkMessageQueue = new Queue<Packet>();
            this.csEvent = new object();
        }

        public void EnqueueNetworkEvent(NETWORK_EVENT eventType)
        {
            lock (this.csEvent)
            {
                this.networkEventQueue.Enqueue(eventType);
            }
        }

        public bool HasEvent()
        {
            lock (this.csEvent)
            {
                return this.networkEventQueue.Count > 0;
            }
        }

        public NETWORK_EVENT DequeueNetworkEvent()
        {
            lock (this.csEvent)
            {
                return this.networkEventQueue.Dequeue();
            }
        }


        public bool HasMessage()
        {
            lock (this.csEvent)
            {
                return this.networkMessageQueue.Count > 0;
            }
        }

        public void EnqueueNetworkMessage(Packet buffer)
        {

            lock (this.csEvent)
            {
                this.networkMessageQueue.Enqueue(buffer);
            }
        }

        public Packet DequeueNetworkMessage()
        {
            lock (this.csEvent)
            {
                Packet msg = this.networkMessageQueue.Dequeue();


                return msg;
            }
        }
    }



}
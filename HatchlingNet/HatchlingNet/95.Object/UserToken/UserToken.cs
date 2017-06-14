using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace HatchlingNet
{
    public class UserToken
    {
        public Socket socket;
        public SocketAsyncEventArgs receiveEventArgs;
        public SocketAsyncEventArgs sendEventArgs;
        public int TokenID { get; set; }

        MessageTranslator messageTranslator = new MessageTranslator();

        public IPeer Peer { get; set; }

        readonly Queue<Packet> sendingQueue = new Queue<Packet>();
        object csSendingQueue = new object();

        public UserToken()
        {
            messageTranslator.CompletedMessage = CompleteMessage;
        }

        public void OpenMessage()
        {
            messageTranslator.Translate(receiveEventArgs.Buffer, receiveEventArgs.Offset, receiveEventArgs.BytesTransferred);
        }

        //프로그램을 실행시킨 피어에게 수신패킷이 완성됬음을 알린다
        //피어는 메인에서 결정됨
        public void CompleteMessage(byte[] buffer)
        {
            if (Peer != null)
                Peer.OnMessage(buffer);
        }

        public void Send(Packet msg)
        {
            Packet clone = new Packet();
            msg.CopyTo(clone);

            lock (csSendingQueue)
            {
                if (sendingQueue.Count <= 0)
                {
                    sendingQueue.Enqueue(clone);
                    StartSend();
                    return;
                }
                sendingQueue.Enqueue(clone);
            }
        }

        void StartSend()
        {
            lock (csSendingQueue)
            {
                Packet msg = sendingQueue.Peek();
                msg.RecordSize();

                //패킷 크기만큼 버퍼 재설정
                sendEventArgs.SetBuffer(sendEventArgs.Offset, msg.position);

                Array.Copy(msg.buffer, 0, sendEventArgs.Buffer, sendEventArgs.Offset, msg.position);

                //전송!
                bool isAsync = socket.SendAsync(sendEventArgs);
                if (!isAsync)
                    ProcessSend(sendEventArgs);
            }
        }
        public Action<Packet, int> SendTo { get; set; }
        public Action<Packet, int> Broadcast { get; set; }
        public void BroadCastWithMe(Packet message)
        {
            Broadcast(message, -1);
        }


        static object csCount = new object();

        //테스트만하고 NetowowrkService클래스로 넘기자
        public void ProcessSend(SocketAsyncEventArgs sendArgs)
        {
            if (sendArgs.BytesTransferred <= 0)
                return;

            lock (csSendingQueue)
            {
                //0이하가 되는경우는 없지만 혹시모를 예외처리
                if (sendingQueue.Count <= 0)
                    throw new Exception("Sedning queue count is less than zero!");

                int size = sendingQueue.Peek().position;
                if (sendArgs.BytesTransferred != size)
                {
                    Trace.WriteLine($"Need to send more! transferred {sendArgs.BytesTransferred},  packet size {size}");
                    return;
                }

                //전송 완료된 패킷을 큐에서 제거
                Packet garbageMsg = sendingQueue.Dequeue();

                PacketBufferManager.Instance.Push(garbageMsg);


                // 아직 전송되지 않은 패킷이 있다면 다시한번 요청
                if (sendingQueue.Count > 0)
                    StartSend();
            }
        }

        //클라
        public void Disconnect()
        {
            try
            {
                this.socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }

            this.socket.Close();
        }

        //클라한테는 disconnect쓰고 서버에서는 OnRemove쓰고 막 섞여있네..?
        //하나로 합쳐야 될것같다.
        //서버
        public void Clear()
        {
            sendingQueue.Clear();
            socket.Disconnect(false);
            socket = null;

            if (Peer != null)
            {
                Peer.Destroy();
            }
        }
    }
}

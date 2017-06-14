using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Collections;

namespace HatchlingNet
{
    public class UserToken
    {
        public Socket socket;
        public SocketAsyncEventArgs receiveEventArgs;
        public SocketAsyncEventArgs sendEventArgs;
        public int TokenID { get; set; }

        MessageTranslator messageTranslator;

        //int bufferSize;

        public IPeer Peer { get; set; }

        Queue<Packet> sendingQueue;
        private object csSendingQueue;

        public delegate void BroadcastHandler(Packet msg, int withOut = -1);
        public BroadcastHandler CallbackBroadcast { get; set; }

        public delegate void SendToHandler(int tokenID, Packet msg);
        public SendToHandler CallbackSendTo { get; set; }

        public UserToken()
        {
            csSendingQueue = new object();//락 조건용
            messageTranslator = new MessageTranslator();
            Peer = null;
            sendingQueue = new Queue<Packet>();
        }

        public void OpenMessage(byte[] buffer, int offset, int transferred)
        {
            messageTranslator.Translate(buffer, offset, transferred, this);
        }

        //프로그램을 실행시킨 피어에게 수신패킷이 완성됬음을 알린다
        //피어는 메인에서 결정됨
        public void CompleteMessage(byte[] buffer)
        {
            //            Console.WriteLine("메세지완성!");

            if (this.Peer != null)
            {
                this.Peer.OnMessage(buffer);
            }
        }

        public void Send(Packet msg)
        {
            Packet clone = new Packet();
            msg.CopyTo(clone);

            lock (this.csSendingQueue)
            {
                if (this.sendingQueue.Count <= 0)
                {
                    this.sendingQueue.Enqueue(clone);
                    StartSend();
                    return;
                }
                this.sendingQueue.Enqueue(clone);
            }
        }

        void StartSend()
        {
            lock (this.csSendingQueue)
            {
                Packet msg = this.sendingQueue.Peek();
                msg.RecordSize();

                this.sendEventArgs.SetBuffer(this.sendEventArgs.Offset, msg.position); //NetworkService클래스에서
                                                                                       //풀에 들어가는 모든 이벤트객체들에게 버퍼할당했는데
                                                                                       //여기서 또하네?
                                                                                       //=> 여기서하는건 그버퍼중 어디서 어디까지 쓸건지 지정하는거임
                                                                                       //사용하는공간이 적으면 그만큼만 보내서 더 빠르게...
                                                                                       //그래서 거기선 인자3개 넘기고 여기선 인자 2개만 넘김...

                Array.Copy(msg.buffer, 0, sendEventArgs.Buffer, sendEventArgs.Offset, msg.position);

                bool pending = this.socket.SendAsync(this.sendEventArgs);//전송!
                if (!pending)
                {
                    ProcessSend(this.sendEventArgs);
                }
                //else
                //{
                //    Packet garbageMsg = this.sendingQueue.Dequeue();

                //    PacketBufferManager.Push(garbageMsg);
                //}

            }
        }

        //static int sendCount = 0;
        static object csCount = new object();

        //테스트만하고 NetowowrkService클래스로 넘기자
        public void ProcessSend(SocketAsyncEventArgs sendArgs)
        {
            if (sendArgs.BytesTransferred <= 0)//
            {
                return;
            }

            lock (this.csSendingQueue)
            {
                if (this.sendingQueue.Count <= 0)
                {//0이하가 되는경우는 없지만 혹시모를 예외처리
                    throw new Exception("Sedning queue count is less than zero!");
                }

                int size = this.sendingQueue.Peek().position;
                if (sendArgs.BytesTransferred != size)
                {
                    string error = string.Format("Need to send more! transferred {0},  packet size {1}", sendArgs.BytesTransferred, size);
                    //                    Console.WriteLine(error);
                    return;
                }

                //전송 완료된 패킷을 큐에서 제거
                Packet garbageMsg = this.sendingQueue.Dequeue();

                PacketBufferManager.Instance.Push(garbageMsg);


                // 아직 전송되지 않은 패킷이 있다면 다시한번 요청
                if (this.sendingQueue.Count > 0)
                {
                    StartSend();
                }
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
        public void OnRemove()
        {
            sendingQueue.Clear();
            socket.Disconnect(false);


            if (Peer != null)
            {
                Peer.Destroy();
            }
        }

        //public void start_keepalive()
        //{//????
        //    System.Threading.Timer keepalive = new System.Threading.Timer((object e) =>
        //    {
        //        Packet msg = Packet.create(0);
        //        msg.Push(0);
        //        Send(msg);
        //    }, null, 0, 3000);
        //}


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HatchlingNet
{
    public class UserToken
    {
        public Socket socket;
        public SocketAsyncEventArgs receiveEventArgs;
        public SocketAsyncEventArgs sendEventArgs;

        MessageTranslator messageTranslator;

        int bufferSize;

        IPeer peer;

        Queue<Packet> sendingQueue;
        private object csSendingQueue;

        public UserToken()
        {
            this.csSendingQueue = new object();//락 조건용
            this.messageTranslator = new MessageTranslator();
            this.peer = null;
            this.sendingQueue = new Queue<Packet>();
        }

        public void SetPeer(IPeer peer)
        {
            this.peer = peer;
        }

        public void OpenMessage(byte[] buffer, int offset, int transferred)
        {
            this.messageTranslator.Translate(buffer, offset, transferred, CompleteMessage);
        }

        void CompleteMessage(byte[] buffer)//프로그램을 실행시킨 피어에게 수신패킷이 완성됬음을 알린다
        {                               //피어는 메인에서 결정됨
            Console.WriteLine("메세지완성!");

            if (this.peer != null)
            {
                this.peer.OnMessage(buffer);
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
                                                                                     //버퍼매니저 쓰면 풀링해서 쓰기떄문에 빠르지만
                                                                                     //보내는 데이터가 많아지고
                                                                                     //여기서 할당하면 반대....

                Array.Copy(msg.buffer, 0, this.sendEventArgs.Buffer, this.sendEventArgs.Offset, msg.position);

                bool pending = this.socket.SendAsync(this.sendEventArgs);
                if (!pending)
                {
                    ProcessSend(this.sendEventArgs);
                }

            }
        }

        static int sendCount = 0;
        static object csCount = new object();

        //테스트만하고 NetowowrkService클래스로 넘기자
        public void ProcessSend(SocketAsyncEventArgs sendArgs)
        {
            if (sendArgs.BytesTransferred <= 0)
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
                    Console.WriteLine(error);
                    return;
                }

                //lock (csCount)//락안에서 락을 한번 더 할필요가 있나?
                //{
                //    ++sendCount;

                //    {
                //        Console.WriteLine(string.Format("process send : {0}, transferred {1}, sent count {2}",
                //            sendArgs.SocketError, sendArgs.BytesTransferred, sendCount));
                //    }
                //}

                //전송 완료된 패킷을 큐에서 제거
                this.sendingQueue.Dequeue();

                // 아직 전송되지 않은 패킷이 있다면 다시한번 요청
                if (this.sendingQueue.Count > 0)
                {
                    StartSend();
                }
            }
        }

        public void disconnect()
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
        public void OnRemove()
        {
            this.sendingQueue.Clear();

            if (this.peer != null)
            {
                this.peer.Disconnect();
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

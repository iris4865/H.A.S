using System;

namespace HatchlingNet
{
    /// <summary>
    /// 데이터를 파싱하는 클래스
    /// [header: body의 데이터크기를 갖는다.]
    /// [body: 데이터]
    /// </summary>
    class MessageTranslator
    {
        public Action<byte[]> CompletedMessage { private get; set; }
        readonly byte[] messageBuffer = new byte[1024];

        int messageSize;
        int currentPosition;//현재 진행중인 버퍼의 인덱스를 가리키는 변수. 패킷완성후 0으로 초기화 해야한다
        int goalPosition;   //읽어와야할 목표 위치
        int remainBytes;
        int srcPosition;

        //srcPosition은 첫번째 매개변수의 buffer에서 저장할 인덱스를 가리킨다.당연히 처음엔0, 패킷이 쪼개져 오는경우 ReadUntill함수에 의해 바뀐다
        //transffered 는 이번통신으로 수신된 바이트수. 한번에 통신으로 패킷클래스 전체가 올지 쪼개져서 올지는 알수없다
        /// <summary>
        /// 분리된 패킷을 합친다.
        /// </summary>
        public void Translate(byte[] buffer, int offset, int transffered)
        {
            //나중에 메세지가 문자열로 안오고 클래스로 오면 바껴야 되는데
            //한번에 어떻게 못하나?
            remainBytes = transffered;
            srcPosition = offset;

            while (remainBytes > 0)
            {
                if (currentPosition < Packet.HeaderSize)
                {
                    goalPosition = Packet.HeaderSize;
                    if (!IsHeaderPacket(buffer))
                        return;
                }

                //IsHeaderPacket이 true야만 올 수 있다.
                if (ReadUntil(buffer))
                {
                    CompletedMessage(messageBuffer);
                    ClearBuffer();
                }
            }
        }

        bool IsHeaderPacket(byte[] buffer)
        {
            if (ReadUntil(buffer))
            {
                messageSize = GetBodySize();
                goalPosition = messageSize + Packet.HeaderSize;
                return true;
            }
            return false;
        }

        bool ReadUntil(byte[] buffer)
        {
            int copySize = goalPosition - currentPosition;

            if (remainBytes < copySize)
                copySize = remainBytes;

            Array.Copy(buffer, srcPosition, messageBuffer, currentPosition, copySize);

            srcPosition += copySize;
            currentPosition += copySize;
            remainBytes -= copySize;

            if (currentPosition < goalPosition)
                return false;

            return true;
        }

        //헤더를 읽어서 메시지 크기를 반환한다
        int GetBodySize()
        {
            return BitConverter.ToInt16(messageBuffer, 0);
        }

        void ClearBuffer()
        {
            Array.Clear(messageBuffer, 0, messageBuffer.Length);

            currentPosition = 0;
            messageSize = 0;
        }

    }
}

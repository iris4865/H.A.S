using System;

namespace HatchlingNet
{
    /*
    * [header][body]구조를 갖는 데이터를 파싱하는 클래스
    * header : Define클래스에 HEADERSIZE로 정의된 타입만큼의 크기를 갖는다.
    *          헤더외에는 body의 크기에 대한 데이터가 들어간다
    * body : 보내려는 데이터
    * 
    */

    class MessageTranslator
    {

        byte[] messageBuffer = new byte[1024];

        int messageSize;
        int currentPosition;//현재 진행중인 버퍼의 인덱스를 가리키는 변수. 패킷완성후 0으로 초기화 해야한다
        int goalPosition;   //읽어와야할 목표 위치
        int remainBytes;


        public MessageTranslator()
        {
            this.messageSize = 0;
            this.currentPosition = 0;
            this.goalPosition = 0;
            this.remainBytes = 0;
        }


        //srcPosition은 첫번째 매개변수의 buffer에서 저장할 인덱스를 가리킨다.당연히 처음엔0, 패킷이 쪼개져 오는경우 ReadUntill함수에 의해 바뀐다
        //transffered 는 이번통신으로 수신된 바이트수. 한번에 통신으로 패킷클래스 전체가 올지 쪼개져서 올지는 알수없다
        //
        //나중에 메세지가 문자열로 안오고 클래스로 오면 바껴야 되는데
        //한번에 어떻게 못하나?
        public void Translate(byte[] buffer, int offset, int transffered, UserToken token)
        {
            remainBytes = transffered;
            int srcPosition = offset;

            while (this.remainBytes > 0)
            {
                bool complete = false;

                if (this.currentPosition < Define.HEADERSIZE)
                {
                    this.goalPosition = Define.HEADERSIZE;

                    complete = ReadUntil(buffer, ref srcPosition, offset, transffered);

                    if (!complete)
                    {
                        return;
                    }

                    this.messageSize = GetBodySize();
                    this.goalPosition = this.messageSize + Define.HEADERSIZE;
                }

                complete = ReadUntil(buffer, ref srcPosition, offset, transffered);

                if (complete)
                {
                    token.CompleteMessage(messageBuffer);
                    ClearBuffer();
                }
                //메세지 패킷 완성 안되면 전달받은 패킷중 remainByte확인하고 더 없으면 루프 끝
                //
            }
        }



        bool ReadUntil(byte[] buffer, ref int srcPosition, int offset, int transffered)
        {
            if (this.currentPosition >= offset + transffered) // 저장해야하는 위치 >= translate에서 받은 offset + 이번통신에 받은 크기
            {                                                   //이런일이 발생 가능? 없어도 될거같은데?
                return false;
            }

            int copySize = this.goalPosition - this.currentPosition;

            if (this.remainBytes < copySize)
            {
                copySize = this.remainBytes;
            }

            Array.Copy(buffer, srcPosition, this.messageBuffer, this.currentPosition, copySize);
            //copy(buffer[srcPosition], messageBuffer[currentPosition], copySize) 이런느낌 


            srcPosition += copySize;
            this.currentPosition += copySize;
            this.remainBytes -= copySize;

            if (this.currentPosition < this.goalPosition)
            {
                return false;
            }

            //이런식으로 짤경우 하나의 완성된 메세지패킷은 1024라는 버퍼의 크기를 넘지않도록 해야겠네
            //만약 보내는 사람이 하나의 메세지패킷을 3000이라는 크기로 보내면 받는입장에선
            //메세지패킷을 완성시킬수가 없으므로 해석도 불가...
            return true;
        }

        int GetBodySize()
        {//헤더의 크기는 2바이트라고 가정
         //패킷의 2바이트만큼 읽어서 바디의 크기를 알아내 반환한다

            return BitConverter.ToInt16(this.messageBuffer, 0);
        }

        void ClearBuffer()
        {
            Array.Clear(this.messageBuffer, 0, this.messageBuffer.Length);

            this.currentPosition = 0;
            this.messageSize = 0;
        }

    }
}

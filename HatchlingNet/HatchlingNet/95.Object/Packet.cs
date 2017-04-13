﻿using System;
using System.Text;


namespace HatchlingNet
{
    public class Packet
    {
        public IPeer owner { get; private set; }
        public byte[] buffer { get; private set; }
        public int position { get; private set; }

        public Int16 protocolType { get; private set; }
        public Int16 sendType { get; private set; }

        public Packet(byte[] buffer, IPeer owner)
        {
            this.owner = owner;
            this.buffer = buffer;

            position = Define.HEADERSIZE;
        }

        //인자없는건 버퍼에 메모리 할당하고 인자 있는생성자는 메모리 할당 안한다는걸 주의
        //패킷버퍼매니저에서 메모리 풀링할때 인자없는생성자 사용해서 미리 다 해놓고
        //인자있는건 보통 일회용으로 잠깐 패킷 해석할때 쓴다
        public Packet()
        {
            buffer = new byte[1024];
        }

        public Int16 PopProtocolType()
        {
            return PopInt16();
        }

        public Int16 PopSendType()
        {
            return PopInt16();
        }

        public Int16 PeekSendType()
        {
            Int16 data = BitConverter.ToInt16(this.buffer, Define.HEADERSIZE + Define.PROTOCOLSIZE);//this.buffer[this.position]에서 Int16만큼 변환

            return data;
        }

        public void CopyTo(Packet target)
        {
            target.SetProtocol(this.protocolType);
            target.SetSendType(this.sendType);
            target.OverWrite(this.buffer, this.position);
        }

        public void OverWrite(byte[] source, int position)
        {
            Array.Copy(source, this.buffer, position);
            this.position = position;
        }

        public byte PopByte()
        {
            byte data = (byte)BitConverter.ToInt16(this.buffer, this.position);
            this.position += sizeof(byte);

            return data;
        }


        public Int16 PopInt16()
        {
            Int16 data = BitConverter.ToInt16(this.buffer, this.position);//this.buffer[this.position]에서 Int16만큼 변환
            this.position += sizeof(Int16);

            return data;
        }

        public Int32 PopInt32()
        {
            Int32 data = BitConverter.ToInt32(this.buffer, this.position);
            this.position += sizeof(Int32);

            return data;
        }

        public float PopFloat()
        {
            float data = BitConverter.ToSingle(this.buffer, this.position);
            this.position += sizeof(float);

            return data;
        }

        public MyVector3 PopVector()
        {
            MyVector3 data;
            data.x = BitConverter.ToSingle(this.buffer, this.position); this.position += sizeof(float);
            data.y = BitConverter.ToSingle(this.buffer, this.position); this.position += sizeof(float);
            data.z = BitConverter.ToSingle(this.buffer, this.position); this.position += sizeof(float);

            return data;
        }

        public double PopDouble()
        {
            double data = BitConverter.ToDouble(this.buffer, this.position);
            this.position += sizeof(double);

            return data;
        }

        public string PopString()
        {
            Int16 len = BitConverter.ToInt16(this.buffer, this.position);
            this.position += sizeof(Int16);

            string data = System.Text.Encoding.UTF8.GetString(this.buffer, this.position, len);
            this.position += len;

            return data;
        }

        public void SetProtocol(Int16 protocolType)
        {
            this.protocolType = protocolType;
            this.position = Define.HEADERSIZE;            //헤더는 나중에 넣을것이므로 데이터부터 넣을수 있도록 위치를 점프 시켜놓는다
                                                            //RecordSize호출 위치 참고
            Push(protocolType);
        }

        public void SetSendType(Int16 sendType)
        {
            this.sendType = sendType;
            this.position = Define.HEADERSIZE + Define.PROTOCOLSIZE;            
            Push(sendType);
        }


        public void RecordSize()
        {//패킷의 헤더부분에 body의 크기를 입력한다
            Int16 bodySize = (Int16)(this.position - Define.HEADERSIZE);
            byte[] header = BitConverter.GetBytes(bodySize);
            header.CopyTo(this.buffer, 0);
        }


        public void Push(byte data)
        {
            byte[] tempBuffer = BitConverter.GetBytes(data);
            tempBuffer.CopyTo(this.buffer, this.position);//CopyTo라는 함수이름 너무 잘지은듯 
            this.position += tempBuffer.Length;
        }

        public void Push(Int16 data)
        {
            byte[] tempBuffer = BitConverter.GetBytes(data);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;
        }

        public void Push(Int32 data)
        {
            byte[] tempBuffer = BitConverter.GetBytes(data);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;
        }


        public void Push(float data)
        {
            byte[] tempBuffer = BitConverter.GetBytes(data);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;
        }

        public void Push(float x, float y, float z)
        {
            byte[] tempBuffer = BitConverter.GetBytes(x);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;

            tempBuffer = BitConverter.GetBytes(y);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;

            tempBuffer = BitConverter.GetBytes(z);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;
        }

        public void Push(MyVector3 data)
        {
            byte[] tempBuffer = BitConverter.GetBytes(data.x);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;

            tempBuffer = BitConverter.GetBytes(data.y);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;

            tempBuffer = BitConverter.GetBytes(data.z);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;
        }

        public void Push(double data)
        {
            byte[] tempBuffer = BitConverter.GetBytes(data);
            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;
        }


        public void Push(string data)
        {//왜 문자열일때만 문자열의 크기를 따로 저장하는 코드가 있는거지?
            byte[] tempBuffer = Encoding.UTF8.GetBytes(data);
            Int16 len = (Int16)tempBuffer.Length;

            byte[] lenBuffer = BitConverter.GetBytes(len);
            lenBuffer.CopyTo(this.buffer, this.position);
            this.position += sizeof(Int16);

            tempBuffer.CopyTo(this.buffer, this.position);
            this.position += tempBuffer.Length;
        }

    }
}

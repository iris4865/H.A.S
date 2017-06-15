using System;
using System.Text;


namespace HatchlingNet
{
    public class Packet
    {
        public IPeer Owner { get; private set; }
        public byte[] Buffer { get; private set; }
        public int Position { get; private set; }

        public Int16 ProtocolType { get; private set; }
        public Int16 SendType { get; private set; }

        public Packet(byte[] buffer, IPeer owner)
        {
            this.Owner = owner;
            this.Buffer = buffer;

            Position = Define.HeaderSize;
        }

        //인자없는건 버퍼에 메모리 할당하고 인자 있는생성자는 메모리 할당 안한다는걸 주의
        //패킷버퍼매니저에서 메모리 풀링할때 인자없는생성자 사용해서 미리 다 해놓고
        //인자있는건 보통 일회용으로 잠깐 패킷 해석할때 쓴다
        public Packet()
        {
            Buffer = new byte[1024];
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
            Int16 data = BitConverter.ToInt16(this.Buffer, Define.HeaderSize + Define.ProtocolSize);//this.buffer[this.position]에서 Int16만큼 변환

            return data;
        }

        public void CopyTo(Packet target)
        {
            target.SetProtocol(this.ProtocolType);
            target.SetSendType(this.SendType);
            target.OverWrite(this.Buffer, this.Position);
        }

        public void OverWrite(byte[] source, int position)
        {
            Array.Copy(source, this.Buffer, position);
            this.Position = position;
        }

        public byte PopByte()
        {
            byte data = (byte)BitConverter.ToInt16(this.Buffer, this.Position);
            this.Position += sizeof(byte);

            return data;
        }


        public Int16 PopInt16()
        {
            Int16 data = BitConverter.ToInt16(this.Buffer, this.Position);//this.buffer[this.position]에서 Int16만큼 변환
            this.Position += sizeof(Int16);

            return data;
        }

        public Int32 PopInt32()
        {
            Int32 data = BitConverter.ToInt32(this.Buffer, this.Position);
            this.Position += sizeof(Int32);

            return data;
        }

        public float PopFloat()
        {
            float data = BitConverter.ToSingle(this.Buffer, this.Position);
            this.Position += sizeof(float);

            return data;
        }

        public MyVector3 PopVector()
        {
            MyVector3 data;
            data.x = BitConverter.ToSingle(this.Buffer, this.Position); this.Position += sizeof(float);
            data.y = BitConverter.ToSingle(this.Buffer, this.Position); this.Position += sizeof(float);
            data.z = BitConverter.ToSingle(this.Buffer, this.Position); this.Position += sizeof(float);

            return data;
        }

        public double PopDouble()
        {
            double data = BitConverter.ToDouble(this.Buffer, this.Position);
            this.Position += sizeof(double);

            return data;
        }

        public string PopString()
        {
            Int16 len = BitConverter.ToInt16(this.Buffer, this.Position);
            this.Position += sizeof(Int16);

            string data = Encoding.UTF8.GetString(this.Buffer, this.Position, len);
            this.Position += len;

            return data;
        }

        public void SetProtocol(Int16 protocolType)
        {
            this.ProtocolType = protocolType;
            this.Position = Define.HeaderSize;            //헤더는 나중에 넣을것이므로 데이터부터 넣을수 있도록 위치를 점프 시켜놓는다
                                                            //RecordSize호출 위치 참고
            Push(protocolType);
        }

        public void SetSendType(Int16 sendType)
        {
            SendType = sendType;
            Position = Define.HeaderSize + Define.ProtocolSize;            
            Push(sendType);
        }

        //패킷의 헤더부분에 body의 크기를 입력한다
        public void RecordSize()
        {
            Int16 bodySize = (Int16)(Position - Define.HeaderSize);
            byte[] header = BitConverter.GetBytes(bodySize);
            header.CopyTo(Buffer, 0);
        }

        public void Push(byte[] data)
        {
            data.CopyTo(Buffer, Position);
            Position += data.Length;
        }

        public void Push(byte data)
        {
            Push(BitConverter.GetBytes(data));
        }

        public void Push(Int16 data)
        {
            Push(BitConverter.GetBytes(data));
        }

        public void Push(Int32 data)
        {
            Push(BitConverter.GetBytes(data));
        }

        public void Push(float data)
        {
            Push(BitConverter.GetBytes(data));
        }

        public void Push(params float[] datas)
        {
            foreach(float data in datas)
                Push(BitConverter.GetBytes(data));
        }

        public void Push(MyVector3 data)
        {
            Push(data.x);
            Push(data.y);
            Push(data.z);
        }

        public void Push(double data)
        {
            Push(BitConverter.GetBytes(data));
        }

        public void Push(string data)
        {
            byte[] tempBuffer = Encoding.UTF8.GetBytes(data);
            Int16 len = (Int16)tempBuffer.Length;

            Push(BitConverter.GetBytes(len));
            Push(tempBuffer);
        }

    }
}
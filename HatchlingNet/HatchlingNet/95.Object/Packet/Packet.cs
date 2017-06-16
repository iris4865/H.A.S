using Header;
using System;
using System.Text;


namespace HatchlingNet
{
    public class Packet
    {
        //다른 용도의 이유가 없기 때문에 const로 수정
        public const int HeaderSize = 2;
        public const int ProtocolSize = 2;

        public byte[] Buffer { get; private set; }
        public int Position { get; private set; }

        public Packet(byte[] buffer)
        {
            Buffer = buffer;

            Reallocate();
        }

        //인자없는건 버퍼에 메모리 할당하고 인자 있는생성자는 메모리 할당 안한다는걸 주의
        //패킷버퍼매니저에서 메모리 풀링할때 인자없는생성자 사용해서 미리 다 해놓고
        //인자있는건 보통 일회용으로 잠깐 패킷 해석할때 쓴다
        public Packet()
        {
            Buffer = new byte[1024];
        }

        public void Reallocate()
        {
            Position = HeaderSize + ProtocolSize;
        }

        public void SetProtocol(PROTOCOL protocolType)
        {
            Position = HeaderSize;
            Push((Int16)protocolType);
        }

        //패킷의 헤더부분에 body의 크기를 입력한다
        public void RecordSize()
        {
            Int16 bodySize = (Int16)(Position - HeaderSize);
            byte[] header = BitConverter.GetBytes(bodySize);
            header.CopyTo(Buffer, 0);
        }

        public void CopyTo(Packet target)
        {
            Buffer.CopyTo(target.Buffer, Position);
            target.Position = Position;
        }

        public PROTOCOL PopProtocolType()
        {
            return (PROTOCOL)PopInt16();
        }

        public byte PopByte()
        {
            byte data = Buffer[Position];
            Position += sizeof(byte);

            return data;
        }


        public Int16 PopInt16()
        {
            Int16 data = BitConverter.ToInt16(Buffer, Position);
            Position += sizeof(Int16);

            return data;
        }

        public Int32 PopInt32()
        {
            Int32 data = BitConverter.ToInt32(Buffer, Position);
            Position += sizeof(Int32);

            return data;
        }

        public float PopFloat()
        {
            float data = BitConverter.ToSingle(Buffer, Position);
            Position += sizeof(float);

            return data;
        }

        public MyVector3 PopVector()
        {
            MyVector3 data;
            data.x = BitConverter.ToSingle(Buffer, Position); Position += sizeof(float);
            data.y = BitConverter.ToSingle(Buffer, Position); Position += sizeof(float);
            data.z = BitConverter.ToSingle(Buffer, Position); Position += sizeof(float);

            return data;
        }

        public double PopDouble()
        {
            double data = BitConverter.ToDouble(Buffer, Position);
            Position += sizeof(double);

            return data;
        }

        public string PopString()
        {
            Int16 len = BitConverter.ToInt16(Buffer, Position);
            Position += sizeof(Int16);

            string data = Encoding.UTF8.GetString(Buffer, Position, len);
            Position += len;

            return data;
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
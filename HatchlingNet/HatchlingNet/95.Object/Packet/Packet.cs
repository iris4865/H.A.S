using Header;
using System;
using System.Text;


namespace HatchlingNet
{
    public class Packet
    {
        public const int HeaderSize = 2;
        public const int ProtocolSize = 2;

        public byte[] Buffer { get; private set; }
        public int Position { get; private set; }
        public PROTOCOL Protocol
        {
            get
            {
                return (PROTOCOL)BitConverter.ToInt16(Buffer, HeaderSize);
            }
            set
            {
                byte[] data = BitConverter.GetBytes((Int16)value);
                Array.Copy(data, 0, Buffer, HeaderSize, ProtocolSize);
            }
        }

        public Packet()
        {
            Buffer = new byte[1024];
            Clear();
        }

        public Packet(byte[] buffer)
        {
            Buffer = buffer;
            Clear();
        }

        public void Clear()
        {
            Position = HeaderSize + ProtocolSize;
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
            Buffer.CopyTo(target.Buffer, 0);
            target.Position = Position;
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
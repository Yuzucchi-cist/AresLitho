using System.IO;
using System.Text;

namespace AresLitho.Commons
{
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream output) : base(output) { }
        public BigEndianBinaryWriter(Stream output, Encoding encoding) : base(output, encoding) { }
        public BigEndianBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen) { }
        public override void Write(short value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
        public override void Write(ushort value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
        public override void Write(int value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
        public override void Write(uint value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
        public override void Write(long value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
        public override void Write(ulong value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
        public override void Write(float value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
    }
}

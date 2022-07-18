using System;
using System.Text;

namespace UCP.Patching
{

    public static partial class SectionEditor
    {
        private class SectionHeader
        {
            public const int HeaderSize = 40;
            public const int NameLength = 8;

            public string GetNameStr()
            {
                return Encoding.ASCII.GetString(Name);
            }

            public void SetNameStr(string nameStr)
            {
                byte[] buf = Encoding.ASCII.GetBytes(nameStr);
                Buffer.BlockCopy(buf, 0, Name, 0, buf.Length);
                for (int i = buf.Length; i < NameLength; i++)
                    Name[i] = 0;
            }

            public readonly byte[] Name = new byte[NameLength];
            public uint VirtSize;
            public uint VirtAddr;
            public uint RawSize;
            public uint RawAddr;
            public uint RelocAddr;
            public uint LineNums;
            public ushort RelocNum;
            public ushort LineNumsNum;
            public uint Characteristics;

            public SectionHeader(string nameStr)
            {
                SetNameStr(nameStr);
            }

            public SectionHeader(byte[] data, int offset)
            {
                Buffer.BlockCopy(data, offset, Name, 0, NameLength);

                VirtSize = BitConverter.ToUInt32(data, offset + 8);
                VirtAddr = BitConverter.ToUInt32(data, offset + 12);
                RawSize = BitConverter.ToUInt32(data, offset + 16);
                RawAddr = BitConverter.ToUInt32(data, offset + 20);
                RelocAddr = BitConverter.ToUInt32(data, offset + 24);
                LineNums = BitConverter.ToUInt32(data, offset + 28);
                RelocNum = BitConverter.ToUInt16(data, offset + 32);
                LineNumsNum = BitConverter.ToUInt16(data, offset + 34);
                Characteristics = BitConverter.ToUInt32(data, offset + 36);
            }

            public void Write(byte[] data, int offset)
            {
                Buffer.BlockCopy(Name, 0, data, offset, NameLength);

                BitConverter.GetBytes(VirtSize).CopyTo(data, offset + 8);
                BitConverter.GetBytes(VirtAddr).CopyTo(data, offset + 12);
                BitConverter.GetBytes(RawSize).CopyTo(data, offset + 16);
                BitConverter.GetBytes(RawAddr).CopyTo(data, offset + 20);
                BitConverter.GetBytes(RelocAddr).CopyTo(data, offset + 24);
                BitConverter.GetBytes(LineNums).CopyTo(data, offset + 28);
                BitConverter.GetBytes(RelocNum).CopyTo(data, offset + 32);
                BitConverter.GetBytes(LineNumsNum).CopyTo(data, offset + 34);
                BitConverter.GetBytes(Characteristics).CopyTo(data, offset + 36);
            }
        }
    }
}

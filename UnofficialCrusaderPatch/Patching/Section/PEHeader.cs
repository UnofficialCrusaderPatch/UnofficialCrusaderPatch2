using System;
using System.Collections.Generic;

namespace UCP.Patching
{
    public static partial class SectionEditor
    {
        private class PEHeader
        {
            private int    offset;
            public  ushort NumberOfSections { get; }

            public  uint SectionAlignment { get; }

            public  uint FileAlignment { get; }

            private List<SectionHeader>        sections;
            public  IEnumerable<SectionHeader> Sections => sections;

            public PEHeader(byte[] input, int offset)
            {
                this.offset = offset;

                NumberOfSections = BitConverter.ToUInt16(input, offset + 0x06);
                SectionAlignment = BitConverter.ToUInt32(input, offset + 0x38);
                FileAlignment = BitConverter.ToUInt32(input, offset + 0x3C);

                sections = new List<SectionHeader>(NumberOfSections);
                for (int i = 0; i < NumberOfSections; i++)
                {
                    sections.Add(new SectionHeader(input, offset + 0xF8 + SectionHeader.HeaderSize * i));
                }
            }

            public static PEHeader Find(byte[] input)
            {
                int index = input.FindIndex(0x4550); // "PE"
                return index < 0 ? null : new PEHeader(input, index);
            }

            public byte[] AddSection(byte[] input, SectionHeader sec)
            {
                // make space
                byte[] data = new byte[sec.RawAddr + sec.RawSize];
                Buffer.BlockCopy(input, 0, data, 0, (int)sec.RawAddr);

                // number of sections
                ushort sectionCount = (ushort)(NumberOfSections + 1);
                BitConverter.GetBytes(sectionCount).CopyTo(data, offset + 0x06);

                // size of image
                uint imageSize = sec.VirtAddr + GetMultiples(sec.VirtSize, SectionAlignment);
                BitConverter.GetBytes(imageSize).CopyTo(data, offset + 0x50);

                sec.Write(data, offset + 0xF8 + SectionHeader.HeaderSize * NumberOfSections);

                return data;
            }
        }
    }
}

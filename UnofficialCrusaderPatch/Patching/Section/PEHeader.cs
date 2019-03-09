using System;
using System.Collections.Generic;

namespace UCP.Patching
{
    public static partial class SectionEditor
    {
        class PEHeader
        {
            int offset;
            ushort numberOfSections;
            public ushort NumberOfSections => numberOfSections;

            uint sectionAlignment;
            public uint SectionAlignment => sectionAlignment;

            uint fileAlignment;
            public uint FileAlignment => fileAlignment;

            List<SectionHeader> sections;
            public IEnumerable<SectionHeader> Sections => sections;

            public PEHeader(byte[] input, int offset)
            {
                this.offset = offset;

                numberOfSections = BitConverter.ToUInt16(input, offset + 0x06);
                sectionAlignment = BitConverter.ToUInt32(input, offset + 0x38);
                fileAlignment = BitConverter.ToUInt32(input, offset + 0x3C);

                sections = new List<SectionHeader>(numberOfSections);
                for (int i = 0; i < numberOfSections; i++)
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
                ushort sectionCount = (ushort)(this.NumberOfSections + 1);
                BitConverter.GetBytes(sectionCount).CopyTo(data, offset + 0x06);

                // size of image
                uint imageSize = sec.VirtAddr + GetMultiples(sec.VirtSize, sectionAlignment);
                BitConverter.GetBytes(imageSize).CopyTo(data, offset + 0x50);

                sec.Write(data, offset + 0xF8 + SectionHeader.HeaderSize * this.NumberOfSections);

                return data;
            }
        }
    }
}

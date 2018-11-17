using System;
using System.Collections.Generic;
using System.Linq;

namespace UnofficialCrusaderPatch
{
    public static partial class SectionEditor
    {
        static List<byte> dataBuf = new List<byte>();

        public static AddressSpace GetDataPos()
        {
            return new AddressSpace(ucpSec.VirtAddr + (uint)dataBuf.Count, ucpSec.RawAddr + (uint)dataBuf.Count);
        }

        public static void AddData(IEnumerable<byte> data)
        {
            dataBuf.AddRange(data);
        }
        
        static PEHeader header;
        static SectionHeader ucpSec;

        public static void Init(byte[] input)
        {
            // find headers entry
            header = PEHeader.Find(input);
            if (header == null)
                throw new Exception("Failed to find PE header! Unsupported Version?");

            var prevSec = header.Sections.Last();

            // new code section
            ucpSec = new SectionHeader(".ucp")
            {
                VirtAddr = prevSec.VirtAddr + GetMultiples(prevSec.VirtSize, header.SectionAlignment),
                RawAddr = prevSec.RawAddr + GetMultiples(prevSec.RawSize, header.FileAlignment),

                Characteristics = 0x60000020, // executable, readonly, contains code
            };
        }
        
        public static byte[] AttachSection(byte[] input)
        {
            //if (dataBuf.Count == 0)
            //    return input;

            uint size = (uint)dataBuf.Count + 1;
            ucpSec.VirtSize = GetMultiples(size, header.SectionAlignment);
            ucpSec.RawSize = GetMultiples(size, header.FileAlignment);

            byte[] data = header.AddSection(input, ucpSec);

            dataBuf.CopyTo(data, input.Length);

            return data;
        }

        static uint GetMultiples(uint size, uint mult)
        {
            uint num = size + mult - 1;
            return num - num % mult;
        }
    }
}

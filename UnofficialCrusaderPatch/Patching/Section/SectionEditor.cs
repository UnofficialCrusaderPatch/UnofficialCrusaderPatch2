using System;
using System.Linq;

namespace UCP.Patching
{
    public static partial class SectionEditor
    {
        public static void Reset()
        {
            currentLen = 0;
        }

        static uint currentLen = 0;
        static byte[] buffer = new byte[0];

        public static AddressSpace ReserveBufferSpace(uint size)
        {
            uint newLen = currentLen + size;

            uint rawSize = GetMultiples(newLen, header.FileAlignment);
            if (buffer.Length < rawSize)
            {
                byte[] newBuffer = new byte[rawSize];
                buffer.CopyTo(newBuffer, 0);
                buffer = newBuffer;
            }

            var space = new AddressSpace(ucpSec.VirtAddr + currentLen, currentLen);
            currentLen = newLen;
            return space;
        }

        public static byte[] GetBuffer()
        {
            return buffer;
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

                Characteristics = 0xE0000020 // writable, //0x60000020, // executable, readable, contains code
            };
        }
        
        public static byte[] AttachSection(byte[] input)
        {
            if (buffer.Length == 0)
                return input;

            uint size = (uint)buffer.Length;
            ucpSec.VirtSize = GetMultiples(size, header.SectionAlignment);
            ucpSec.RawSize = GetMultiples(size, header.FileAlignment);

            byte[] data = header.AddSection(input, ucpSec);
            Buffer.BlockCopy(buffer, 0, data, (int)ucpSec.RawAddr, buffer.Length);

            return data;
        }

        static uint GetMultiples(uint size, uint mult)
        {
            uint num = size + mult - 1;
            return num - num % mult;
        }
    }
}

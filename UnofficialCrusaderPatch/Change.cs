using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnofficialCrusaderPatch
{
    abstract class Change
    {
        int address;
        public int Address { get { return this.address; } }

        byte[] data;

        public Change(int address, byte[] data)
        {
            this.address = address;
            this.data = data;
        }

        public void Edit(FileStream fs)
        {
            int filePosition = this.Address - 0x400000;
            fs.Seek(filePosition, SeekOrigin.Begin);
            fs.Write(this.data, 0, this.data.Length);
        }
    }

    class ChangeInt32 : Change
    {
        public ChangeInt32(int address, int newValue)
            : base(address, BitConverter.GetBytes(newValue))
        {
        }
    }

    class ChangeFloat : Change
    {
        public ChangeFloat(int address, float newValue)
            : base(address, BitConverter.GetBytes(newValue))
        {
        }
    }
    
    class ChangeBytes : Change
    {
        public ChangeBytes(int address, params byte[] data)
            : base(address, data)
        {
        }
    }
}

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

        string description;
        public string Description { get { return this.description; } }

        public Change(int address, byte[] data, string description)
        {
            this.address = address;
            this.data = data;
            this.description = description;
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
        public ChangeInt32(int address, int newValue, string description = null)
            : base(address, BitConverter.GetBytes(newValue), description)
        {
        }
    }

    class ChangeFloat : Change
    {
        public ChangeFloat(int address, float newValue, string description = null)
            : base(address, BitConverter.GetBytes(newValue), description)
        {
        }
    }
    
    class ChangeBytes : Change
    {
        public ChangeBytes(int address, byte[] data, string description = null)
            : base(address, data, description)
        {
        }

        public ChangeBytes(int address, string description, params byte[] data)
            : base(address, data, description)
        {
        }
    }
}

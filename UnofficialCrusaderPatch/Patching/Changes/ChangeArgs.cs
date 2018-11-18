using System.IO;

namespace UnofficialCrusaderPatch
{
    public struct ChangeArgs
    {
        public byte[] Data;
        public byte[] OriData;
        public DirectoryInfo AIVDir;

        public ChangeArgs(byte[] data, byte[] oriData, DirectoryInfo aivDir)
        {
            this.Data = data;
            this.OriData = oriData;
            this.AIVDir = aivDir;
        }
    }
}

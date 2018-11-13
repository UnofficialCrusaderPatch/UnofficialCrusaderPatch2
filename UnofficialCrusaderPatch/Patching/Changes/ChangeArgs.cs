using System.IO;

namespace UnofficialCrusaderPatch
{
    public class ChangeArgs
    {
        byte[] data;
        public byte[] Data => data;

        byte[] oriData;
        public byte[] OriData => oriData;

        DirectoryInfo aivDir;
        public DirectoryInfo AIVDir => aivDir;

        public ChangeArgs(byte[] data, byte[] oriData, DirectoryInfo aivDir)
        {
            this.data = data;
            this.oriData = oriData;
            this.aivDir = aivDir;
        }
    }
}

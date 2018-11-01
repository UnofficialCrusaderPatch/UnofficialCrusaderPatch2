using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace CodeBlox
{
    class Program
    {
        static byte[] oriData;
        static void Main(string[] args)
        {
            try
            {
                int address;
                while (true)
                {
                    Console.Write("Address: ");

                    string input = Console.ReadLine();
                    if (input.StartsWith("0x"))
                        input = input.Substring(2);

                    if (int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address))
                        break;
                }

                oriData = File.ReadAllBytes("Stronghold Crusader.exe");

                address -= 0x400000;
                if (address < 0x1000 || address > oriData.Length)
                    throw new Exception("Address is out of range! " + address);

                Console.WriteLine();
                DirectoryInfo dir = new DirectoryInfo("versions");
                foreach (FileInfo file in dir.EnumerateFiles("*.exe"))
                {
                    Console.Write(string.Format("'{0}': ", file.Name));
                    string result = CheckVersion(file, address);
                    Console.WriteLine(result);
                }
                
                int size;
                while (true)
                {
                    Console.Write("Choose size: ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out size))
                        break;
                }

                if (size <= 0 || address + size > oriData.Length)
                    throw new Exception("Size is out of range! " + size);

                byte[] buffer = new byte[size];
                Buffer.BlockCopy(oriData, address, buffer, 0, size);
                File.WriteAllBytes("data.bin", buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }

        const int maxSize = 256;
        static string CheckVersion(FileInfo file, int address)
        {
            byte[] data = new byte[file.Length];
            using (FileStream fs = file.OpenRead())
                fs.Read(data, 0, data.Length);

            int first = 2; // start with 2 bytes
            while (first < maxSize)
            {
                if (address + first > oriData.Length)
                    return "out of range";

                if (CheckBlock(data, address, first) == 1)
                    return first.ToString();

                first++;
            }

            return "none < " + maxSize;
        }

        static int CheckBlock(byte[] data, int address, int size)
        {
            byte[] block = new byte[size];
            Buffer.BlockCopy(oriData, address, block, 0, size);

            int count = 0;
            int sizeless = size - 1;
            for (int i = 0; i < data.Length - size; i++)
            {
                for (int b = 0; b < size; b++)
                {
                    if (data[i + b] == block[b])
                    {
                        if (b == sizeless)
                            count++;
                    }
                    else break;
                }
            }
            return count;
        }
    }
}

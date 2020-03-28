using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UCP.AICharacters;

namespace IDAParser
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ParseAll();

                /*using (FileStream fs = new FileStream("wiki.md", FileMode.Create))
                using (AIWriter aiw = new AIWriter(fs))
                {
                    aiw.WriteMarkdown(typeof(AIPersonality));
                }*/

                string ucpPath = Directory.GetCurrentDirectory() + "../../../../UnofficialCrusaderPatch/AIC/Resources";
                UpdateAICs(Path.GetFullPath(ucpPath));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        static void UpdateAICs(string folder)
        {
            foreach(string file in Directory.EnumerateFiles(folder, "*.aic"))
            {
                AICCollection aicc;
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    aicc = new AICCollection(fs);
                }


                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    aicc.Write(fs);
                }
                Console.WriteLine(file);
            }
        }

        static void ParseAll()
        {
            AICCollection collection = new AICCollection();

            foreach (string file in Directory.EnumerateFiles("decompiled", "*.txt"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (!Enum.TryParse(name, true, out AICIndex index))
                    continue;

                Console.WriteLine(index);
                using (StreamReader sr = new StreamReader(file))
                {
                    AICharacter aic = new AICharacter()
                    {
                        Index = (int)index,
                        Name = index.ToString(),
                        Personality = Parse(sr)
                    };
                    collection.Add((int)index, aic);
                }
            }

            using (FileStream fs = new FileStream("vanilla.aic", FileMode.Create))
                collection.Write(fs);
        }

        const string preamble = "*((_DWORD *)result + ";
        const string preambleZero = "*(_DWORD *)result";

        static AIPersonality Parse(StreamReader sr)
        {

            AIPersonality result = new AIPersonality();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                line = line.Trim(); // get rid of whitespaces
                if (line.StartsWith(preamble))
                {
                    ReadIndex(line, result);
                }
                else if (line.StartsWith(preambleZero))
                {
                    ReadIndexZero(line, result);
                }
                else
                {
                    continue;
                }
            }

            return result;
        }

        static void ReadIndex(string line, AIPersonality result)
        {
            // find field index
            int startIndex = line.IndexOf(')', preamble.Length);
            if (startIndex < 0)
            {
                Console.WriteLine("Error (field index search): " + line);
                return;
            }

            string str = line.Substring(preamble.Length, startIndex - preamble.Length).Trim();
            if (!int.TryParse(str, out int field))
            {
                Console.WriteLine("Error (field index parsing): " + line);
                return;
            }

            // find field value
            startIndex = line.IndexOf('=', startIndex);
            if (startIndex < 0)
            {
                Console.WriteLine("Error (field value search '='): " + line);
                return;
            }
            startIndex++;

            int endIndex = line.IndexOf(';', startIndex);
            if (endIndex < 0)
            {
                Console.WriteLine("Error (field value search ';'): " + line);
                return;
            }

            str = line.Substring(startIndex, endIndex - startIndex).Trim();
            if (!int.TryParse(str, out int value))
            {
                Console.WriteLine("Error (field value parsing): " + line);
                return;
            }

            result.SetByIndex(field, value);
        }



        static void ReadIndexZero(string line, AIPersonality result)
        {
            // find field value
            int startIndex = line.IndexOf('=', preambleZero.Length);
            if (startIndex < 0)
            {
                Console.WriteLine("Err0r (field value search '='): " + line);
                return;
            }
            startIndex++;

            int endIndex = line.IndexOf(';', startIndex);
            if (endIndex < 0)
            {
                Console.WriteLine("Err0r (field value search ';'): " + line);
                return;
            }

            string str = line.Substring(startIndex, endIndex - startIndex).Trim();
            if (!int.TryParse(str, out int value))
            {
                Console.WriteLine("Err0r (field value parsing): " + line);
                return;
            }

            result.SetByIndex(0, value);
        }
    }
}

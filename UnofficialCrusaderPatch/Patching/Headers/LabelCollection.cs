using System;
using System.Collections.Generic;

namespace UnofficialCrusaderPatch
{
    public class LabelCollection
    {
        Dictionary<string, BinLabel> dict = new Dictionary<string, BinLabel>(StringComparer.OrdinalIgnoreCase);

        public void Add(BinLabel label)
        {
            try
            {
                dict.Add(label.Name, label);
            }
            catch (Exception e)
            {
                throw new Exception(label.Name + "\n" + e);
            }
        }

        public int GetLabel(string labelName)
        {
            if (!dict.TryGetValue(labelName, out BinLabel label))
                throw new Exception("Label not found! " + labelName);
            return label.VirtAddress;
        }

        public void Clear() => dict.Clear();
    }
}

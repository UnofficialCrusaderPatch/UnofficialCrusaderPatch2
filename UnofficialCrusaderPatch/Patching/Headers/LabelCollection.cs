using System;
using System.Collections.Generic;

namespace UnofficialCrusaderPatch
{
    public class LabelCollection
    {
        Dictionary<string, BinLabel> dict = new Dictionary<string, BinLabel>(StringComparer.OrdinalIgnoreCase);

        public void Add(BinLabel label)
        {
            dict.Add(label.Name, label);
        }

        public int GetLabel(string labelName)
        {
            if (!dict.TryGetValue(labelName, out BinLabel label))
                throw new Exception("Label not found! " + labelName);
            return label.VirtAddress;
        }
    }
}

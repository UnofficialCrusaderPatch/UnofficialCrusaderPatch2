using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UCP.AICharacters
{
    /// <summary>
    /// A dictionary of type (AICIndex, AICharacter) which can be used to collect all AIC data
    /// </summary>
    public class AICCollection : Dictionary<AICIndex, AICharacter>
    {
        public void Write(Stream stream)
        {
            using (AIWriter aiw = new AIWriter(stream))
            {
                foreach(AICharacter c in this.Values)
                {
                    aiw.Write(c);
                    aiw.WriteLine();
                }
            }
        }

        public void Read(Stream stream)
        {
            using (AIReader air = new AIReader(stream))
            {
                AICharacter aic;
                while ((aic = air.Read<AICharacter>()) != null)
                {
                    this.Add(aic.Index, aic);
                }
            }
        }
    }
}

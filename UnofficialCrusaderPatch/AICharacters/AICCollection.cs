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
            using (LineWriter lw = new LineWriter(stream))
            {
                foreach (AICharacter aic in this.Values)
                {
                    aic.Write(lw);
                }
            }
        }

        public void Read(Stream stream)
        {
            using (LineReader lr = new LineReader(stream))
            {
                AICharacter c;
                while ((c = AICharacter.Read(lr)) != null)
                {
                    this.Add(c.Index, c);
                }
            }
        }
    }
}

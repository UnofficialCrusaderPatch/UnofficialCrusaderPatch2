using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace UCP.AICharacters
{
    /// <summary>
    /// AICharacter collection - a dictionary of type (AICIndex, AICharacter) which can be used to collect all AIC data
    /// </summary>
    public class AICCollection : Dictionary<int, AICharacter>
    {
        AIFileHeader header = new AIFileHeader();
        public AIFileHeader Header => header;

        /// <summary> Creates an empty AICharacter collection. </summary>
        public AICCollection() : base(16)
        {
        }

        /// <summary> Initialises a new AICharacter collection from the given stream. </summary>
        public AICCollection(Stream stream) : this()
        {
            this.Read(stream, false);
        }

        /// <summary> Writes the contents of this AICCollection into the stream in ASCII text encoding.</summary>
        public void Write(Stream stream)
        {
            using (AIWriter aiw = new AIWriter(stream))
            {
                aiw.Write(header);
                aiw.WriteLine();

                aiw.OpenCommentSec();
                aiw.WriteInfo(typeof(AICharacter));
                aiw.CloseCommentSec();
                aiw.WriteLine();

                foreach (AICharacter c in this.Values.OrderBy(c => c.Index))
                {
                    aiw.Write(c);
                    aiw.WriteLine();
                }
            }
        }

        /// <summary>
        /// Reads ASCII encoded AICharacters into this collection from the given stream
        /// </summary>
        /// <param name="replaceExisting">Set to true to overwrite AICharacters which already exist in this collection. </param>
        public void Read(Stream stream, bool replaceExisting)
        {
            using (AIReader air = new AIReader(stream))
            {
                var readHeader = air.Read<AIFileHeader>();
                if (readHeader != null)
                    this.header = readHeader;

                air.Reset(); // for the case that the header was in the middle or not there at all

                AICharacter aic;
                while ((aic = air.Read<AICharacter>()) != null)
                {
                    if (this.ContainsKey(aic.Index))
                    {
                        if (replaceExisting)
                        {
                            this[aic.Index] = aic;
                        }
                        else
                        {
                            throw new Exception(aic.Index + " does already exist in this collection!");
                        }
                    }
                    else
                    {
                        this.Add(aic.Index, aic);
                    }
                }
            }
        }
    }
}

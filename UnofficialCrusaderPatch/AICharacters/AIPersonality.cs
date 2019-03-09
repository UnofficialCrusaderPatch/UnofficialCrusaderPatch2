using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.AICharacters
{
    /// <summary>
    /// An array of integers which describes the personality in Crusader
    /// </summary>
    public class AIPersonality
    {
        public const string SectionTitle = "Personality";
        public const int FieldCount = 169;

        int[] fields = new int[FieldCount];
        public int this[int index]
        {
            get { return fields[index]; }
            set { fields[index] = value; }
        }

        public void Write(LineWriter lw)
        {
            lw.OpenSec(SectionTitle);

            for (int i = 0; i < FieldCount; i++)
                WriteField(lw, i);

            lw.CloseSec();
        }

        void WriteField(LineWriter lw, int index)
        {
            AIPField field = (AIPField)index;
            int value = fields[index];

            // check if we have a description of this field
            string comment;
            if (Enum.IsDefined(typeof(AIPField), index))
            {
                comment = field.ToString();
            }
            else
            {
                comment = null;
            }

            // check if this field's value is a known enum
            string valueStr;
            if (AIPFieldType.TryGet(field, out Type fieldType)
                && Enum.IsDefined(fieldType, value))
            {
                valueStr = Enum.GetName(fieldType, value);
            }
            else
            {
                valueStr = value.ToString();
            }

            lw.WriteLine(valueStr, comment);
        }

        public static AIPersonality Read(LineReader lr)
        {
            if (!lr.OpenSec(SectionTitle))
                return null;

            AIPersonality result = new AIPersonality();
            for (int i = 0; i < FieldCount; i++)
                result.ReadField(lr, i);

            lr.CloseSec();
            return result;
        }

        void ReadField(LineReader lr, int index)
        {
            AIPField field = (AIPField)index;
            string valueStr = lr.ReadLine();
            
            if (AIPFieldType.TryGet(field, out Type fieldType)
                && Enum.GetNames(fieldType).Contains(valueStr, StringComparer.OrdinalIgnoreCase))
            {
                this.fields[index] = (int)Enum.Parse(fieldType, valueStr);
                return;
            }

            this.fields[index] = int.Parse(valueStr);
        }
    }
}

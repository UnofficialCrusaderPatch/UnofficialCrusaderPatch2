using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.AICharacters
{
    public class AIFileHeader
    {
        public string DescrGer = "";
        public string DescrEng = "";
        public string DescrPol = "";
        public string DescrRus = "";

        public string DescrByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return DescrGer;
                case 1:
                default:
                    return DescrEng;
                case 2:
                    return DescrPol;
                case 3:
                    return DescrRus;

            }
        }
    }
}

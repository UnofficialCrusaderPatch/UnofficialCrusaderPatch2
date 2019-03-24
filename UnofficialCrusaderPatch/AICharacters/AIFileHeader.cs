using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.AICharacters
{
    public class AIFileHeader
    {
        public static readonly string[] GeneralInfo = new string[]
        {
            "// This file contains parameters which determine the personalities of Crusader's AI-Characters.",
            "// DO NOT SWITCH, ADD OR REMOVE ANY PARAMETER LINES! The program sets the parameters internally",
            "// by order of appearance, not by the parameter names! This is done to guarentee backwards",
            "// compatibility, as the names might be subject to change. They are just there to help you editing."
        };

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

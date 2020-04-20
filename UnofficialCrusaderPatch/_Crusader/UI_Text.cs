using System;
using System.Data;
using System.Linq;
using System.Text;
using UCP.Patching;

namespace UCP.Patching
{
    public class UI_Text
    {
        /**
         * Alpha. 0 is 0% transparency.
         */
        protected byte alpha = 0x00;

        /**
         * Font size.
         */
        protected byte fontSize = 0x12;

        /**
         * Color in hexadecimal.
         */
        protected int color = 0x00C00200;

        /**
         * Width of the textbox.
         */
        protected int width = 200;

        /**
         * X position.
         */
        protected int y = 0;

        /**
         * Y position.
         */
        protected int x = 0;


        private string text;
        private string label;


        public UI_Text(string label, string text)
        {
            this.text = text;
            this.label = label;
        }

        public UI_Text SetAlpha(byte value)
        {
            alpha = value;
            return this;
        }

        public UI_Text SetColor(int value)
        {
            color = value;
            return this;
        }

        public UI_Text SetFontSize(byte value)
        {
            fontSize = value;
            return this;
        }

        public UI_Text SetWidth(int value)
        {
            width = value;
            return this;
        }

        public UI_Text SetXPosition(int value)
        {
            x = value;
            return this;
        }

        public UI_Text SetYPosition(int value)
        {
            y = value;
            return this;
        }

        public BinAlloc GetData()
        {
            return new BinAlloc(label, null)
            {
                new BinAlloc(label+"TEXT", null)
                {
                    // Choose your color
                    Encoding.ASCII.GetBytes(text),
                    0x00, 0x00, 0x00, 0x00,
                },
                
                0x6A, alpha,
                0x6A, fontSize,
                0x68, BitConverter.GetBytes(color),
                0x68, BitConverter.GetBytes(width), // push 000000A5 { 165 }
                0xB9, BitConverter.GetBytes(y), // mov ecx,00000200 { 512 }
                0x51, // push ecx
                0xBF, BitConverter.GetBytes(x), // mov edi,00000180 { 384 }
                0x57, // push edi
                0x6A, 0x00, // push 00 { 0 }
                0x6A, 0x00, // push 00 { 0 }
                0xB9, 0x78, 0x75, 0x15, 0x02, // mov ecx,"Stronghold Crusader.exe"+1D57578 { [000000B1] }
                0xFF, 0x15, new BinRefTo("TextRenderer1", false), // call "Stronghold Crusader.exe"+6A050 { ->Stronghold Crusader.exe+6A050 }
                0xB8, new BinRefTo(label+"TEXT", false),
                0x50, // push eax
                0xB9, 0x78, 0x75, 0x15, 0x02, // mov ecx,"Stronghold Crusader.exe"+1D57578 { [000000B1] }
                0xFF, 0x15, new BinRefTo("TextRenderer2", false), // call "Stronghold Crusader.exe"+73A70 { ->Stronghold Crusader.exe+73A70 }
                0xC3 //  ret
            };
        }
    }
}
using System;
using System.Linq;
using System.Runtime.Serialization;
using static UCP.Patching.BinElements.Register;

namespace UCP.Patching.BinElements
{
    public class OpCodes
    {
        public static BinElement PUSH(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement PUSH(sbyte val)
        {
            if (val > 0)
            {
                return new byte[] { 0x6A, (byte)val };
            }
            else
            {
                return new byte[] { 0x68, (byte)Math.Abs(val) };
            }
        }

        public static BinElement PUSH(Int32 val)
        {
            if (val <= SByte.MaxValue && val >= SByte.MinValue)
            {
                return PUSH((sbyte)val);
            }
            if (val > 0x80)
            {
                return (new byte[] { 0x68 }).Concat(BitConverter.GetBytes(val)).ToArray();
            }
            else
            {
                return (new byte[] { 0x68 }).Concat(BitConverter.GetBytes(0xFFFFFFFF + val - 1)).ToArray();
            }
        }

        public static BinElement PUSH(Register reg)
        {
            switch (reg)
            {
                case EAX:
                    return 0x50;
                case ECX:
                    return 0x51;
                case EDX:
                    return 0x52;
                case EBX:
                    return 0x53;
                case ESP:
                    return 0x54;
                case EBP:
                    return 0x55;
                case ESI:
                    return 0x56;
                case EDI:
                    return 0x57;
                case ALL:
                    return 0x60;
                case FLAGS:
                    return new byte[] { 0x66, 0x9C };
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement POP(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement POP(Register reg)
        {
            switch (reg)
            {
                case EAX:
                    return 0x58;
                case ECX:
                    return 0x59;
                case EDX:
                    return 0x5A;
                case EBX:
                    return 0x5B;
                case ESP:
                    return 0x5C;
                case EBP:
                    return 0x5D;
                case ESI:
                    return 0x5E;
                case EDI:
                    return 0x5F;
                case ALL:
                    return 0x61;
                case FLAGS:
                    return new byte[] { 0x66, 0x9D };
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement CMP(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement CMP(Register reg, sbyte val)
        {
            switch (reg)
            {
                case EAX:
                    return new byte[] { 0x83, 0xF8, (byte)Math.Abs(val) };
                case ECX:
                    return new byte[] { 0x83, 0xF9, (byte)Math.Abs(val) };
                case EDX:
                    return new byte[] { 0x83, 0xFA, (byte)Math.Abs(val) };
                case EBX:
                    return new byte[] { 0x83, 0xFB, (byte)Math.Abs(val) };
                case ESP:
                    return new byte[] { 0x83, 0xFC, (byte)Math.Abs(val) };
                case EBP:
                    return new byte[] { 0x83, 0xFD, (byte)Math.Abs(val) };
                case ESI:
                    return new byte[] { 0x83, 0xFE, (byte)Math.Abs(val) };
                case EDI:
                    return new byte[] { 0x83, 0xFF, (byte)Math.Abs(val) };
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement CMP(Register reg, Int32 signedValue)
        {

            if (signedValue <= SByte.MaxValue && signedValue >= SByte.MinValue)
            {
                return CMP(reg, (sbyte)signedValue);
            }

            Int32 val = (Int32)(signedValue >= 0 ? signedValue : 0xFFFFFFFF - Math.Abs(signedValue));
            switch (reg)
            {
                case EAX:
                    return (new byte[] { 0x3D }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ECX:
                    return (new byte[] { 0x81, 0xF9 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EDX:
                    return (new byte[] { 0x81, 0xFA }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EBX:
                    return (new byte[] { 0x81, 0xFB }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ESP:
                    return (new byte[] { 0x81, 0xFC }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EBP:
                    return (new byte[] { 0x81, 0xFD }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ESI:
                    return (new byte[] { 0x81, 0xFE }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EDI:
                    return (new byte[] { 0x81, 0xFF }).Concat(BitConverter.GetBytes(val)).ToArray();
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement CMP(Register reg1, Register reg2)
        {
            switch (reg1)
            {
                case EAX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC0 };
                        case ECX:
                            return new byte[] { 0x39, 0xC8 };
                        case EDX:
                            return new byte[] { 0x39, 0xD0 };
                        case EBX:
                            return new byte[] { 0x39, 0xD8 };
                        case ESP:
                            return new byte[] { 0x39, 0xE0 };
                        case EBP:
                            return new byte[] { 0x39, 0xE8 };
                        case ESI:
                            return new byte[] { 0x39, 0xF0 };
                        case EDI:
                            return new byte[] { 0x39, 0xF8 };
                    }
                    throw new UnsupportedOperandException();

                case ECX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC1 };
                        case ECX:
                            return new byte[] { 0x39, 0xC9 };
                        case EDX:
                            return new byte[] { 0x39, 0xD1 };
                        case EBX:
                            return new byte[] { 0x39, 0xD9 };
                        case ESP:
                            return new byte[] { 0x39, 0xE1 };
                        case EBP:
                            return new byte[] { 0x39, 0xE9 };
                        case ESI:
                            return new byte[] { 0x39, 0xF1 };
                        case EDI:
                            return new byte[] { 0x39, 0xF9 };
                    }
                    throw new UnsupportedOperandException();

                case EDX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC2 };
                        case ECX:
                            return new byte[] { 0x39, 0xCA };
                        case EDX:
                            return new byte[] { 0x39, 0xD2 };
                        case EBX:
                            return new byte[] { 0x39, 0xDA };
                        case ESP:
                            return new byte[] { 0x39, 0xE2 };
                        case EBP:
                            return new byte[] { 0x39, 0xEA };
                        case ESI:
                            return new byte[] { 0x39, 0xF2 };
                        case EDI:
                            return new byte[] { 0x39, 0xFA };
                    }
                    throw new UnsupportedOperandException();

                case EBX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC3 };
                        case ECX:
                            return new byte[] { 0x39, 0xCB };
                        case EDX:
                            return new byte[] { 0x39, 0xD3 };
                        case EBX:
                            return new byte[] { 0x39, 0xDB };
                        case ESP:
                            return new byte[] { 0x39, 0xE3 };
                        case EBP:
                            return new byte[] { 0x39, 0xEB };
                        case ESI:
                            return new byte[] { 0x39, 0xF3 };
                        case EDI:
                            return new byte[] { 0x39, 0xFB };
                    }
                    throw new UnsupportedOperandException();

                case ESP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC4 };
                        case ECX:
                            return new byte[] { 0x39, 0xCC };
                        case EDX:
                            return new byte[] { 0x39, 0xD4 };
                        case EBX:
                            return new byte[] { 0x39, 0xDC };
                        case ESP:
                            return new byte[] { 0x39, 0xE4 };
                        case EBP:
                            return new byte[] { 0x39, 0xEC };
                        case ESI:
                            return new byte[] { 0x39, 0xF4 };
                        case EDI:
                            return new byte[] { 0x39, 0xFC };
                    }
                    throw new UnsupportedOperandException();

                case EBP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC5 };
                        case ECX:
                            return new byte[] { 0x39, 0xCD };
                        case EDX:
                            return new byte[] { 0x39, 0xD5 };
                        case EBX:
                            return new byte[] { 0x39, 0xDD };
                        case ESP:
                            return new byte[] { 0x39, 0xE5 };
                        case EBP:
                            return new byte[] { 0x39, 0xED };
                        case ESI:
                            return new byte[] { 0x39, 0xF5 };
                        case EDI:
                            return new byte[] { 0x39, 0xFD };
                    }
                    throw new UnsupportedOperandException();

                case ESI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC6 };
                        case ECX:
                            return new byte[] { 0x39, 0xCE };
                        case EDX:
                            return new byte[] { 0x39, 0xD6 };
                        case EBX:
                            return new byte[] { 0x39, 0xDE };
                        case ESP:
                            return new byte[] { 0x39, 0xE6 };
                        case EBP:
                            return new byte[] { 0x39, 0xEE };
                        case ESI:
                            return new byte[] { 0x39, 0xF6 };
                        case EDI:
                            return new byte[] { 0x39, 0xFE };
                    }
                    throw new UnsupportedOperandException();

                case EDI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x39, 0xC7 };
                        case ECX:
                            return new byte[] { 0x39, 0xCF };
                        case EDX:
                            return new byte[] { 0x39, 0xD7 };
                        case EBX:
                            return new byte[] { 0x39, 0xDF };
                        case ESP:
                            return new byte[] { 0x39, 0xE7 };
                        case EBP:
                            return new byte[] { 0x39, 0xEF };
                        case ESI:
                            return new byte[] { 0x39, 0xF7 };
                        case EDI:
                            return new byte[] { 0x39, 0xFF };
                    }
                    throw new UnsupportedOperandException();
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement MOV(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement MOV(Register reg, Int32 signedValue)
        {
            Int32 val = (Int32)(signedValue >= 0 ? signedValue : 0xFFFFFFFF - Math.Abs(signedValue));
            switch (reg)
            {
                case EAX:
                    return (new byte[] { 0xB8 }).Concat(BitConverter.GetBytes(val)).ToArray();

                case ECX:
                    return (new byte[] { 0xB9 }).Concat(BitConverter.GetBytes(val)).ToArray();

                case EDX:
                    return (new byte[] { 0xBA }).Concat(BitConverter.GetBytes(val)).ToArray();

                case EBX:
                    return (new byte[] { 0xBB }).Concat(BitConverter.GetBytes(val)).ToArray();

                case ESP:
                    return (new byte[] { 0xBC }).Concat(BitConverter.GetBytes(val)).ToArray();

                case EBP:
                    return (new byte[] { 0xBD }).Concat(BitConverter.GetBytes(val)).ToArray();

                case ESI:
                    return (new byte[] { 0xBE }).Concat(BitConverter.GetBytes(val)).ToArray();

                case EDI:
                    return (new byte[] { 0xBF }).Concat(BitConverter.GetBytes(val)).ToArray();
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement MOV(Register reg1, Register reg2)
        {
            switch (reg1)
            {
                case EAX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xC0 };
                        case ECX:
                            return new byte[] { 0x8B, 0xC1 };
                        case EDX:
                            return new byte[] { 0x8B, 0xC2 };
                        case EBX:
                            return new byte[] { 0x8B, 0xC3 };
                        case ESP:
                            return new byte[] { 0x8B, 0xC4 };
                        case EBP:
                            return new byte[] { 0x8B, 0xC5 };
                        case ESI:
                            return new byte[] { 0x8B, 0xC6 };
                        case EDI:
                            return new byte[] { 0x8B, 0xC7 };
                    }
                    throw new UnsupportedOperandException();

                case ECX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xC8 };
                        case ECX:
                            return new byte[] { 0x8B, 0xC9 };
                        case EDX:
                            return new byte[] { 0x8B, 0xCA };
                        case EBX:
                            return new byte[] { 0x8B, 0xCB };
                        case ESP:
                            return new byte[] { 0x8B, 0xCC };
                        case EBP:
                            return new byte[] { 0x8B, 0xCD };
                        case ESI:
                            return new byte[] { 0x8B, 0xCE };
                        case EDI:
                            return new byte[] { 0x8B, 0xCF };
                    }
                    throw new UnsupportedOperandException();

                case EDX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xD0 };
                        case ECX:
                            return new byte[] { 0x8B, 0xD1 };
                        case EDX:
                            return new byte[] { 0x8B, 0xD2 };
                        case EBX:
                            return new byte[] { 0x8B, 0xD3 };
                        case ESP:
                            return new byte[] { 0x8B, 0xD4 };
                        case EBP:
                            return new byte[] { 0x8B, 0xD5 };
                        case ESI:
                            return new byte[] { 0x8B, 0xD6 };
                        case EDI:
                            return new byte[] { 0x8B, 0xD7 };
                    }
                    throw new UnsupportedOperandException();

                case EBX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xD8 };
                        case ECX:
                            return new byte[] { 0x8B, 0xD9 };
                        case EDX:
                            return new byte[] { 0x8B, 0xDA };
                        case EBX:
                            return new byte[] { 0x8B, 0xDB };
                        case ESP:
                            return new byte[] { 0x8B, 0xDC };
                        case EBP:
                            return new byte[] { 0x8B, 0xDD };
                        case ESI:
                            return new byte[] { 0x8B, 0xDE };
                        case EDI:
                            return new byte[] { 0x8B, 0xDF };
                    }
                    throw new UnsupportedOperandException();

                case ESP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xE0 };
                        case ECX:
                            return new byte[] { 0x8B, 0xE1 };
                        case EDX:
                            return new byte[] { 0x8B, 0xE2 };
                        case EBX:
                            return new byte[] { 0x8B, 0xE3 };
                        case ESP:
                            return new byte[] { 0x8B, 0xE4 };
                        case EBP:
                            return new byte[] { 0x8B, 0xE5 };
                        case ESI:
                            return new byte[] { 0x8B, 0xE6 };
                        case EDI:
                            return new byte[] { 0x8B, 0xE7 };
                    }
                    throw new UnsupportedOperandException();

                case EBP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xE8 };
                        case ECX:
                            return new byte[] { 0x8B, 0xE9 };
                        case EDX:
                            return new byte[] { 0x8B, 0xEA };
                        case EBX:
                            return new byte[] { 0x8B, 0xEB };
                        case ESP:
                            return new byte[] { 0x8B, 0xEC };
                        case EBP:
                            return new byte[] { 0x8B, 0xED };
                        case ESI:
                            return new byte[] { 0x8B, 0xEE };
                        case EDI:
                            return new byte[] { 0x8B, 0xEF };
                    }
                    throw new UnsupportedOperandException();

                case ESI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xF0 };
                        case ECX:
                            return new byte[] { 0x8B, 0xF1 };
                        case EDX:
                            return new byte[] { 0x8B, 0xF2 };
                        case EBX:
                            return new byte[] { 0x8B, 0xF3 };
                        case ESP:
                            return new byte[] { 0x8B, 0xF4 };
                        case EBP:
                            return new byte[] { 0x8B, 0xF5 };
                        case ESI:
                            return new byte[] { 0x8B, 0xF6 };
                        case EDI:
                            return new byte[] { 0x8B, 0xF7 };
                    }
                    throw new UnsupportedOperandException();

                case EDI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x8B, 0xF8 };
                        case ECX:
                            return new byte[] { 0x8B, 0xF9 };
                        case EDX:
                            return new byte[] { 0x8B, 0xFA };
                        case EBX:
                            return new byte[] { 0x8B, 0xFB };
                        case ESP:
                            return new byte[] { 0x8B, 0xFC };
                        case EBP:
                            return new byte[] { 0x8B, 0xFD };
                        case ESI:
                            return new byte[] { 0x8B, 0xFE };
                        case EDI:
                            return new byte[] { 0x8B, 0xFF };
                    }
                    throw new UnsupportedOperandException();
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement SUB(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement SUB(Register reg, sbyte signedValue)
        {
            byte val = signedValue < 0 ? (byte)(0xFF - Math.Abs(signedValue) + 1) : (byte)signedValue;
            switch (reg)
            {
                case EAX:
                    return (new byte[] { 0x83, 0xE8, val });
                case ECX:
                    return (new byte[] { 0x83, 0xE9, val });
                case EDX:
                    return (new byte[] { 0x83, 0xEA, val });
                case EBX:
                    return (new byte[] { 0x83, 0xEB, val });
                case ESP:
                    return (new byte[] { 0x83, 0xEC, val });
                case EBP:
                    return (new byte[] { 0x83, 0xED, val });
                case ESI:
                    return (new byte[] { 0x83, 0xEE, val });
                case EDI:
                    return (new byte[] { 0x83, 0xEF, val });
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement SUB(Register reg, Int32 val)
        {
            if (val <= SByte.MaxValue && val >= SByte.MinValue)
            {
                return SUB(reg, (sbyte)val);
            }
            switch (reg)
            {
                case EAX:
                    return (new byte[] { 0x83, 0xE8 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ECX:
                    return (new byte[] { 0x83, 0xE9 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EDX:
                    return (new byte[] { 0x83, 0xEA }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EBX:
                    return (new byte[] { 0x83, 0xEB }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ESP:
                    return (new byte[] { 0x83, 0xEC }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EBP:
                    return (new byte[] { 0x83, 0xED }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ESI:
                    return (new byte[] { 0x83, 0xEE }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EDI:
                    return (new byte[] { 0x83, 0xEF }).Concat(BitConverter.GetBytes(val)).ToArray();
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement SUB(Register reg1, Register reg2)
        {
            switch (reg1)
            {
                case EAX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xC0 };
                        case ECX:
                            return new byte[] { 0x29, 0xC1 };
                        case EDX:
                            return new byte[] { 0x29, 0xC2 };
                        case EBX:
                            return new byte[] { 0x29, 0xC3 };
                        case ESP:
                            return new byte[] { 0x29, 0xC4 };
                        case EBP:
                            return new byte[] { 0x29, 0xC5 };
                        case ESI:
                            return new byte[] { 0x29, 0xC6 };
                        case EDI:
                            return new byte[] { 0x29, 0xC7 };
                    }
                    throw new UnsupportedOperandException();

                case ECX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xC8 };
                        case ECX:
                            return new byte[] { 0x29, 0xC9 };
                        case EDX:
                            return new byte[] { 0x29, 0xCA };
                        case EBX:
                            return new byte[] { 0x29, 0xCB };
                        case ESP:
                            return new byte[] { 0x29, 0xCC };
                        case EBP:
                            return new byte[] { 0x29, 0xCD };
                        case ESI:
                            return new byte[] { 0x29, 0xCE };
                        case EDI:
                            return new byte[] { 0x29, 0xCF };
                    }
                    throw new UnsupportedOperandException();

                case EDX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xD0 };
                        case ECX:
                            return new byte[] { 0x29, 0xD1 };
                        case EDX:
                            return new byte[] { 0x29, 0xD2 };
                        case EBX:
                            return new byte[] { 0x29, 0xD3 };
                        case ESP:
                            return new byte[] { 0x29, 0xD4 };
                        case EBP:
                            return new byte[] { 0x29, 0xD5 };
                        case ESI:
                            return new byte[] { 0x29, 0xD6 };
                        case EDI:
                            return new byte[] { 0x29, 0xD7 };
                    }
                    throw new UnsupportedOperandException();

                case EBX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xD8 };
                        case ECX:
                            return new byte[] { 0x29, 0xD9 };
                        case EDX:
                            return new byte[] { 0x29, 0xDA };
                        case EBX:
                            return new byte[] { 0x29, 0xDB };
                        case ESP:
                            return new byte[] { 0x29, 0xDC };
                        case EBP:
                            return new byte[] { 0x29, 0xDD };
                        case ESI:
                            return new byte[] { 0x29, 0xDE };
                        case EDI:
                            return new byte[] { 0x29, 0xDF };
                    }
                    throw new UnsupportedOperandException();

                case ESP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xE0 };
                        case ECX:
                            return new byte[] { 0x29, 0xE1 };
                        case EDX:
                            return new byte[] { 0x29, 0xE2 };
                        case EBX:
                            return new byte[] { 0x29, 0xE3 };
                        case ESP:
                            return new byte[] { 0x29, 0xE4 };
                        case EBP:
                            return new byte[] { 0x29, 0xE5 };
                        case ESI:
                            return new byte[] { 0x29, 0xE6 };
                        case EDI:
                            return new byte[] { 0x29, 0xE7 };
                    }
                    throw new UnsupportedOperandException();

                case EBP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xE8 };
                        case ECX:
                            return new byte[] { 0x29, 0xE9 };
                        case EDX:
                            return new byte[] { 0x29, 0xEA };
                        case EBX:
                            return new byte[] { 0x29, 0xEB };
                        case ESP:
                            return new byte[] { 0x29, 0xEC };
                        case EBP:
                            return new byte[] { 0x29, 0xED };
                        case ESI:
                            return new byte[] { 0x29, 0xEE };
                        case EDI:
                            return new byte[] { 0x29, 0xEF };
                    }
                    throw new UnsupportedOperandException();

                case ESI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xF0 };
                        case ECX:
                            return new byte[] { 0x29, 0xF1 };
                        case EDX:
                            return new byte[] { 0x29, 0xF2 };
                        case EBX:
                            return new byte[] { 0x29, 0xF3 };
                        case ESP:
                            return new byte[] { 0x29, 0xF4 };
                        case EBP:
                            return new byte[] { 0x29, 0xF5 };
                        case ESI:
                            return new byte[] { 0x29, 0xF6 };
                        case EDI:
                            return new byte[] { 0x29, 0xF7 };
                    }
                    throw new UnsupportedOperandException();

                case EDI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x29, 0xF8 };
                        case ECX:
                            return new byte[] { 0x29, 0xF9 };
                        case EDX:
                            return new byte[] { 0x29, 0xFA };
                        case EBX:
                            return new byte[] { 0x29, 0xFB };
                        case ESP:
                            return new byte[] { 0x29, 0xFC };
                        case EBP:
                            return new byte[] { 0x29, 0xFD };
                        case ESI:
                            return new byte[] { 0x29, 0xFE };
                        case EDI:
                            return new byte[] { 0x29, 0xFF };
                    }
                    throw new UnsupportedOperandException();
            }
            throw new UnsupportedOperandException();
        }



        public static BinElement ADD(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement ADD(Register reg, sbyte signedValue)
        {
            byte val = signedValue < 0 ? (byte)(0xFF - Math.Abs(signedValue) + 1) : (byte)signedValue;
            switch (reg)
            {
                case EAX:
                    return (new byte[] { 0x83, 0xC0, val });
                case ECX:
                    return (new byte[] { 0x83, 0xC1, val });
                case EDX:
                    return (new byte[] { 0x83, 0xC2, val });
                case EBX:
                    return (new byte[] { 0x83, 0xC3, val });
                case ESP:
                    return (new byte[] { 0x83, 0xC4, val });
                case EBP:
                    return (new byte[] { 0x83, 0xC5, val });
                case ESI:
                    return (new byte[] { 0x83, 0xC6, val });
                case EDI:
                    return (new byte[] { 0x83, 0xC7, val });
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement ADD(Register reg, Int32 val)
        {
            if (val <= SByte.MaxValue && val >= SByte.MinValue)
            {
                return ADD(reg, (sbyte)val);
            }
            switch (reg)
            {
                case EAX:
                    return (new byte[] { 0x83, 0xC0 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ECX:
                    return (new byte[] { 0x83, 0xC1 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EDX:
                    return (new byte[] { 0x83, 0xC2 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EBX:
                    return (new byte[] { 0x83, 0xC3 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ESP:
                    return (new byte[] { 0x83, 0xC4 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EBP:
                    return (new byte[] { 0x83, 0xC5 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case ESI:
                    return (new byte[] { 0x83, 0xC6 }).Concat(BitConverter.GetBytes(val)).ToArray();
                case EDI:
                    return (new byte[] { 0x83, 0xC7 }).Concat(BitConverter.GetBytes(val)).ToArray();
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement ADD(Register reg1, Register reg2)
        {
            switch (reg1)
            {
                case EAX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xC0 };
                        case ECX:
                            return new byte[] { 0x01, 0xC1 };
                        case EDX:
                            return new byte[] { 0x01, 0xC2 };
                        case EBX:
                            return new byte[] { 0x01, 0xC3 };
                        case ESP:
                            return new byte[] { 0x01, 0xC4 };
                        case EBP:
                            return new byte[] { 0x01, 0xC5 };
                        case ESI:
                            return new byte[] { 0x01, 0xC6 };
                        case EDI:
                            return new byte[] { 0x01, 0xC7 };
                    }
                    throw new UnsupportedOperandException();

                case ECX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xC8 };
                        case ECX:
                            return new byte[] { 0x01, 0xC9 };
                        case EDX:
                            return new byte[] { 0x01, 0xCA };
                        case EBX:
                            return new byte[] { 0x01, 0xCB };
                        case ESP:
                            return new byte[] { 0x01, 0xCC };
                        case EBP:
                            return new byte[] { 0x01, 0xCD };
                        case ESI:
                            return new byte[] { 0x01, 0xCE };
                        case EDI:
                            return new byte[] { 0x01, 0xCF };
                    }
                    throw new UnsupportedOperandException();

                case EDX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xD0 };
                        case ECX:
                            return new byte[] { 0x01, 0xD1 };
                        case EDX:
                            return new byte[] { 0x01, 0xD2 };
                        case EBX:
                            return new byte[] { 0x01, 0xD3 };
                        case ESP:
                            return new byte[] { 0x01, 0xD4 };
                        case EBP:
                            return new byte[] { 0x01, 0xD5 };
                        case ESI:
                            return new byte[] { 0x01, 0xD6 };
                        case EDI:
                            return new byte[] { 0x01, 0xD7 };
                    }
                    throw new UnsupportedOperandException();

                case EBX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xD8 };
                        case ECX:
                            return new byte[] { 0x01, 0xD9 };
                        case EDX:
                            return new byte[] { 0x01, 0xDA };
                        case EBX:
                            return new byte[] { 0x01, 0xDB };
                        case ESP:
                            return new byte[] { 0x01, 0xDC };
                        case EBP:
                            return new byte[] { 0x01, 0xDD };
                        case ESI:
                            return new byte[] { 0x01, 0xDE };
                        case EDI:
                            return new byte[] { 0x01, 0xDF };
                    }
                    throw new UnsupportedOperandException();

                case ESP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xE0 };
                        case ECX:
                            return new byte[] { 0x01, 0xE1 };
                        case EDX:
                            return new byte[] { 0x01, 0xE2 };
                        case EBX:
                            return new byte[] { 0x01, 0xE3 };
                        case ESP:
                            return new byte[] { 0x01, 0xE4 };
                        case EBP:
                            return new byte[] { 0x01, 0xE5 };
                        case ESI:
                            return new byte[] { 0x01, 0xE6 };
                        case EDI:
                            return new byte[] { 0x01, 0xE7 };
                    }
                    throw new UnsupportedOperandException();

                case EBP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xE8 };
                        case ECX:
                            return new byte[] { 0x01, 0xE9 };
                        case EDX:
                            return new byte[] { 0x01, 0xEA };
                        case EBX:
                            return new byte[] { 0x01, 0xEB };
                        case ESP:
                            return new byte[] { 0x01, 0xEC };
                        case EBP:
                            return new byte[] { 0x01, 0xED };
                        case ESI:
                            return new byte[] { 0x01, 0xEE };
                        case EDI:
                            return new byte[] { 0x01, 0xEF };
                    }
                    throw new UnsupportedOperandException();
                case ESI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xF0 };
                        case ECX:
                            return new byte[] { 0x01, 0xF1 };
                        case EDX:
                            return new byte[] { 0x01, 0xF2 };
                        case EBX:
                            return new byte[] { 0x01, 0xF3 };
                        case ESP:
                            return new byte[] { 0x01, 0xF4 };
                        case EBP:
                            return new byte[] { 0x01, 0xF5 };
                        case ESI:
                            return new byte[] { 0x01, 0xF6 };
                        case EDI:
                            return new byte[] { 0x01, 0xF7 };
                    }
                    throw new UnsupportedOperandException();

                case EDI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x01, 0xF8 };
                        case ECX:
                            return new byte[] { 0x01, 0xF9 };
                        case EDX:
                            return new byte[] { 0x01, 0xFA };
                        case EBX:
                            return new byte[] { 0x01, 0xFB };
                        case ESP:
                            return new byte[] { 0x01, 0xFC };
                        case EBP:
                            return new byte[] { 0x01, 0xFD };
                        case ESI:
                            return new byte[] { 0x01, 0xFE };
                        case EDI:
                            return new byte[] { 0x01, 0xFF };
                    }
                    throw new UnsupportedOperandException();
            }
            throw new UnsupportedOperandException();
        }


        public static BinElement XOR(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement XOR(Register reg1, Register reg2)
        {
            switch (reg1)
            {
                case EAX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xC0 };
                        case ECX:
                            return new byte[] { 0x31, 0xC1 };
                        case EDX:
                            return new byte[] { 0x31, 0xC2 };
                        case EBX:
                            return new byte[] { 0x31, 0xC3 };
                        case ESP:
                            return new byte[] { 0x31, 0xC4 };
                        case EBP:
                            return new byte[] { 0x31, 0xC5 };
                        case ESI:
                            return new byte[] { 0x31, 0xC6 };
                        case EDI:
                            return new byte[] { 0x31, 0xC7 };
                    }
                    throw new UnsupportedOperandException();

                case ECX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xC8 };
                        case ECX:
                            return new byte[] { 0x31, 0xC9 };
                        case EDX:
                            return new byte[] { 0x31, 0xCA };
                        case EBX:
                            return new byte[] { 0x31, 0xCB };
                        case ESP:
                            return new byte[] { 0x31, 0xCC };
                        case EBP:
                            return new byte[] { 0x31, 0xCD };
                        case ESI:
                            return new byte[] { 0x31, 0xCE };
                        case EDI:
                            return new byte[] { 0x31, 0xCF };
                    }
                    throw new UnsupportedOperandException();

                case EDX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xD0 };
                        case ECX:
                            return new byte[] { 0x31, 0xD1 };
                        case EDX:
                            return new byte[] { 0x31, 0xD2 };
                        case EBX:
                            return new byte[] { 0x31, 0xD3 };
                        case ESP:
                            return new byte[] { 0x31, 0xD4 };
                        case EBP:
                            return new byte[] { 0x31, 0xD5 };
                        case ESI:
                            return new byte[] { 0x31, 0xD6 };
                        case EDI:
                            return new byte[] { 0x31, 0xD7 };
                    }
                    throw new UnsupportedOperandException();

                case EBX:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xD8 };
                        case ECX:
                            return new byte[] { 0x31, 0xD9 };
                        case EDX:
                            return new byte[] { 0x31, 0xDA };
                        case EBX:
                            return new byte[] { 0x31, 0xDB };
                        case ESP:
                            return new byte[] { 0x31, 0xDC };
                        case EBP:
                            return new byte[] { 0x31, 0xDD };
                        case ESI:
                            return new byte[] { 0x31, 0xDE };
                        case EDI:
                            return new byte[] { 0x31, 0xDF };
                    }
                    throw new UnsupportedOperandException();

                case ESP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xE0 };
                        case ECX:
                            return new byte[] { 0x31, 0xE1 };
                        case EDX:
                            return new byte[] { 0x31, 0xE2 };
                        case EBX:
                            return new byte[] { 0x31, 0xE3 };
                        case ESP:
                            return new byte[] { 0x31, 0xE4 };
                        case EBP:
                            return new byte[] { 0x31, 0xE5 };
                        case ESI:
                            return new byte[] { 0x31, 0xE6 };
                        case EDI:
                            return new byte[] { 0x31, 0xE7 };
                    }
                    throw new UnsupportedOperandException();

                case EBP:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xE8 };
                        case ECX:
                            return new byte[] { 0x31, 0xE9 };
                        case EDX:
                            return new byte[] { 0x31, 0xEA };
                        case EBX:
                            return new byte[] { 0x31, 0xEB };
                        case ESP:
                            return new byte[] { 0x31, 0xEC };
                        case EBP:
                            return new byte[] { 0x31, 0xED };
                        case ESI:
                            return new byte[] { 0x31, 0xEE };
                        case EDI:
                            return new byte[] { 0x31, 0xEF };
                    }
                    throw new UnsupportedOperandException();

                case ESI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xF0 };
                        case ECX:
                            return new byte[] { 0x31, 0xF1 };
                        case EDX:
                            return new byte[] { 0x31, 0xF2 };
                        case EBX:
                            return new byte[] { 0x31, 0xF3 };
                        case ESP:
                            return new byte[] { 0x31, 0xF4 };
                        case EBP:
                            return new byte[] { 0x31, 0xF5 };
                        case ESI:
                            return new byte[] { 0x31, 0xF6 };
                        case EDI:
                            return new byte[] { 0x31, 0xF7 };
                    }
                    throw new UnsupportedOperandException();

                case EDI:
                    switch (reg2)
                    {
                        case EAX:
                            return new byte[] { 0x31, 0xF8 };
                        case ECX:
                            return new byte[] { 0x31, 0xF9 };
                        case EDX:
                            return new byte[] { 0x31, 0xFA };
                        case EBX:
                            return new byte[] { 0x31, 0xFB };
                        case ESP:
                            return new byte[] { 0x31, 0xFC };
                        case EBP:
                            return new byte[] { 0x31, 0xFD };
                        case ESI:
                            return new byte[] { 0x31, 0xFE };
                        case EDI:
                            return new byte[] { 0x31, 0xFF };
                    }
                    throw new UnsupportedOperandException();
            }
            throw new UnsupportedOperandException();
        }

        public static BinElement JMP(byte[] instruction)
        {
            return instruction;
        }

        public static BinElement JMP(Condition.Values cond, sbyte offset)
        {
            switch (cond)
            {
                case Condition.Values.EQUALS:
                    return new byte[] { 0x74, (byte)Math.Abs(offset) };
                case Condition.Values.NOTEQUALS:
                    return new byte[] { 0x75, (byte)Math.Abs(offset) };
                case Condition.Values.LESS:
                    return new byte[] { 0x7C, (byte)Math.Abs(offset) };
                case Condition.Values.GREATERTHANEQUALS:
                    return new byte[] { 0x7D, (byte)Math.Abs(offset) };
                case Condition.Values.LESSTHANEQUALS:
                    return new byte[] { 0x7E, (byte)Math.Abs(offset) };
                case Condition.Values.GREATER:
                    return new byte[] { 0x7F, (byte)Math.Abs(offset) };
                case Condition.Values.UNCONDITIONAL:
                    return new byte[] { 0xEB, (byte)Math.Abs(offset) };
            }
            throw new UnsupportedOperandException();
        }

        public class Condition
        {
            public enum Values
            {
                EQUALS,
                NOTEQUALS,
                LESS,
                LESSTHANEQUALS,
                GREATER,
                GREATERTHANEQUALS,
                UNCONDITIONAL
            }

            public static Condition.Values GetOpposite(Condition.Values cond)
            {
                switch (cond)
                {
                    case Condition.Values.EQUALS:
                        return Condition.Values.NOTEQUALS;
                    case Condition.Values.NOTEQUALS:
                        return Condition.Values.EQUALS;
                    case Condition.Values.LESS:
                        return Condition.Values.GREATERTHANEQUALS;
                    case Condition.Values.LESSTHANEQUALS:
                        return Condition.Values.GREATER;
                    case Condition.Values.GREATER:
                        return Condition.Values.LESSTHANEQUALS;
                    case Condition.Values.GREATERTHANEQUALS:
                        return Condition.Values.LESS;
                    default:
                        return Condition.Values.UNCONDITIONAL;
                }
            }

        }
    }


    [Serializable]
    internal class UnsupportedOperandException : Exception
    {
        public UnsupportedOperandException()
        {
        }

        public UnsupportedOperandException(string message) : base(message)
        {
        }

        public UnsupportedOperandException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedOperandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

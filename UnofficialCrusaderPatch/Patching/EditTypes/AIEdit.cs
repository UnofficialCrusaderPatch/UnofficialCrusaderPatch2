using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace UnofficialCrusaderPatch
{
    enum AIIndex
    {
        Rat = 1,
        Snake,
        Pig,
        Wolf,
        Saladin,
        Caliph,
        Sultan,
        Richard,
        Frederick,
        Philipp,
        Wazir,
        Emir,
        Nizar,
        Sheriff,
        Marshal,
        Abbot
    }

    enum AIProp
    {
        ReinfDefAfinity = 0x128,
        TotalDef = 0x170,
        WallDef = 0x180,
        DefUnit1 = 0x184,
        DefUnit2 = 0x188,
        DefUnit3 = 0x18C,
        DefUnit4 = 0x190,
        DefUnit5 = 0x194,
        DefUnit6 = 0x198,
        DefUnit7 = 0x19C,
        DefUnit8 = 0x1A0,
    }

    class AIEdit : ChangeEdit, IEnumerable<AIEdit.PropPair>
    {
        class AIProperties : Dictionary<AIProp, int> { }
        static Dictionary<AIIndex, AIProperties> aiDict = new Dictionary<AIIndex, AIProperties>();

        public static void Write(ChangeArgs args)
        {
            if (aiDict.Count == 0)
                return;

            var e = CreateEdit();
            e.Activate(args);

            aiDict.Clear();
        }

        static ChangeHeader CreateEdit()
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach(var kvp in aiDict)
                {
                    // mov eax, index
                    bw.Write((byte)0xB8);
                    bw.Write((int)kvp.Key);

                    // imul eax, 2A4
                    bw.Write((byte)0x69);
                    bw.Write((byte)0xC0);
                    bw.Write(0x2A4);

                    // add eax, esi
                    bw.Write((byte)0x01);
                    bw.Write((byte)0xF0);

                    // edit AI's properties
                    foreach (var pp in kvp.Value)
                    {
                        // mov [eax + prop], value
                        bw.Write((byte)0xC7);
                        bw.Write((byte)0x80);
                        bw.Write((int)pp.Key);
                        bw.Write((int)pp.Value);
                    }
                }
                data = ms.ToArray();
            }

            // 004D1928
            BinaryEdit be = new BinaryEdit("ai_prop")
            {
                new BinAddress("call", 0x1B+1, true),

                new BinSkip(0x1B),
                new BinHook(5)
                {
                    // ori code
                    0xE8, new BinRefTo("call"),

                    // edit ais
                    new BinBytes(data),
                }
            };

            return new DefaultHeader("ai_prop") { be };
        }





        AIIndex index;
        List<PropPair> pairs = new List<PropPair>();
        public struct PropPair
        {
            public AIProp Prop;
            public int Value;
        }

        public AIEdit(AIIndex index)
        {
            this.index = index;
        }

        public void Add(AIProp prop, int value)
        {
            pairs.Add(new PropPair() { Prop=prop, Value=value });
        }

        public IEnumerator<PropPair> GetEnumerator() => pairs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => pairs.GetEnumerator();

        public override bool Initialize(ChangeArgs args)
        {
            if (!aiDict.TryGetValue(this.index, out AIProperties dict))
            {
                dict = new AIProperties();
                aiDict.Add(this.index, dict);
            }

            foreach(PropPair pp in this.pairs)
            {
                if (dict.ContainsKey(pp.Prop))
                {
                    Patcher.AddFailure("", EditFailure.AIMultipleProp);
                    return false;
                }

                dict.Add(pp.Prop, pp.Value);
            }

            return true;
        }

        public override void Activate(ChangeArgs args)
        {
        }
    }
}

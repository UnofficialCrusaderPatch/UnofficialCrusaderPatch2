namespace UCP.Patching
{
    /**
     * FIRE BALLISTAS ATTACK MONKS AND TUNNELERS
     */
    public class Mod_Fix_FireBallista : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("u_fireballistafix", ChangeType.Bugfix, true)
            {
                new DefaultHeader("u_fireballistafix")
                {
                    new BinaryEdit("u_fireballistatunneler")
                    {
                        new BinSkip(13),
                        new BinHook(6)
                        {
                            0x83, 0xF8, 0x05, //  cmp eax,05
                            0x66, 0x9C, //  pushf
                            0x83, 0xC0, 0xEA, //  add eax,-16
                            0x66, 0x9D, //  popf
                            0x75, 0x05, //  jne short 5
                            0xB8, 0x05, 0x00, 0x00, 0x00, //  mov eax,05
                            0x83, 0xF8, 0x37, //  cmp eax,37
                        },
                    },
                    new BinaryEdit("u_fireballistamonk")
                    {
                        new BinBytes(0x00)
                    }
                }
            };
        }
    }
}
namespace UCP.Patching
{
    /*
     * AI BUY FOOD
     */
    public class Mod_Fix_AI_Buy : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            // Wazir runtime buytable 023FE5F4 +84, apples, cheese, bread, wheat
            // Emir 023FE898
            // Nizar 023FEB3C
            

            /*
            * WEAPON & ARMOR AI BUYING - found from routine at 0x4CD62C
            */
            // ai1_buytable 0x01165C78
            return new Change("ai_buy", ChangeType.Bugfix)
            {
                new DefaultHeader("ai_buy")
                {
                    // mov [EAX+84], EDI = 10
                    BinBytes.CreateEdit("ai_foodbuy_wazir", 0x89, 0xB8), // 004C951C
                
                    // mov [EAX+9C], 2
                    BinHook.CreateEdit("ai_wepbuy_marshal", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4CA5AE, runtime: 0x23FF084 + 0x9C
                    BinHook.CreateEdit("ai_wepbuy_frederick", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4C8DEA, runtime: 0x23FE0AC + 0x9C
                    BinHook.CreateEdit("ai_wepbuy_emir", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4C99AB, runtime: 023FE898 + 0x9C
                    BinHook.CreateEdit("ai_wepbuy_abbot", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4CA95B, runtime: 023FF328 + 0x9C
                }
            };
        }
    }
}
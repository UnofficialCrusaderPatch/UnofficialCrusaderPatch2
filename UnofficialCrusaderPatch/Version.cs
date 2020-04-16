using System.Collections.Generic;
using System.Text;
using System.Linq;
using UCP.AIV;
using UCP.Patching;
using UCP.Startup;

namespace UCP
{
    // friedenszeit etc

    // farben:
    // message shield
    // tribok farbe bei AI 4





    // meuchelmörder direkt auf bergfried

    // ai spielen
    // rekrutierungsintervalle
    // bauerngrenze
    // KI Handeln untereinander
    // mehr truppen für burggraben ausheben

    // Störkatapult positionierung
    // Schwein Nahrung verkaufen & Friedrich Waffen ?
    // kalif eisen?
    // Scroll-Tempo in 1.41 reduzieren

    // europäische Bogenschützen mit leicht erhöhter Reichweite.

    public class Version
    {
        public static string PatcherVersion = "2.14";

        // change version 0x424EF1 + 1
        public static readonly ChangeHeader MenuChange = new ChangeHeader()
        {
            BinRedirect.CreateEdit("menuversion", false, Encoding.ASCII.GetBytes("V1.%d UCP" + PatcherVersion + '\0'))
        };
        public static readonly ChangeHeader MenuChange_XT = new ChangeHeader()
        {
            BinRedirect.CreateEdit("menuversion", false, Encoding.ASCII.GetBytes("V1.%d-E UCP" + PatcherVersion + '\0'))
        };


        public static List<Change> Changes { get { return changes; } }
        static List<Change> changes = new List<Change>()
        {
            #region BUG FIXES

            new Mod_Fix_FireBallista().getChange(),
            new Mod_Fix_AI_Access().getChange(),
            new Mod_Fix_AI_Defense().getChange(),
            new Mod_Fix_AI_Tethers().getChange(),
            //new Mod_Fix_AI_Buy().getChange(),
            new Mod_Fix_AI_BuyWood().getChange(),
            new Mod_Fix_AI_TowerEngines().getChange(),
            new Mod_Fix_AI_AssaultSwitch().getChange(),
            new Mod_Fix_AI_Rebuild().getChange(),
            new Mod_Fix_AI_LaddermenUsage().getChange(),
            
            #endregion

            #region AI CHANGES
            
            new Mod_Change_AI_AttackLimit().getChange(),
            new Mod_Change_AI_AttackWave().getChange(),
            new Mod_Change_AI_AttackTarget().getChange(),
            new Mod_Change_AI_NoSleep().getChange(),
            //new Mod_Change_AI_Overclock().getChange(),
            new Mod_Change_AI_Demolish().getChange(),
            new Mod_Change_AI_AddAttack().getChange(),
            new Mod_Change_AI_RecruitInterval().getChange(),

            #endregion

            #region UNITS
            
            new Mod_Change_Units_Laddermen().getChange(),         
            new Mod_Change_Units_ArabianArcher().getChange(),
            new Mod_Change_Units_ArabianSwordsmen().getChange(),
            new Mod_Change_Units_Spearmen().getChange(),
            
            #endregion

            #region OTHER

            new Mod_Change_Other_FireCooldown().getChange(),
            new Mod_Change_Other_RemoveExtremeBar().getChange(),
            new Mod_Change_Other_PlayerColorChange().getChange(),
            new Mod_Change_Other_NewKeybindings().getChange(),
            new Mod_Change_Other_OverrideIdentityMenu().getChange(),
            new Mod_Change_Other_Healer().getChange(),
            new Mod_Change_Other_FreeTradePost().getChange(),
            new Mod_Change_Other_NoSiegeTentDeselection().getChange(),
            new Mod_Change_Other_AlwaysShowPlannedMoat().getChange(),
            new Mod_Change_Other_ExtendedGameSpeed().getChange(),
            new Mod_Change_Other_ResponsiveGates().getChange(),
            new Mod_Change_Other_OnlyAI().getChange(),
            new Mod_Change_Other_ArmoryMarketplaceWeaponOrderFix().getChange(),
            new Mod_Change_Other_DefaultMultiplayerSpeed().getChange(),
            new Mod_Change_Other_Strongholdify().getChange(),

            #endregion
        };

        public static void AddExternalChanges()
        {
            Version.changes.AddRange(ResourceChange.changes);
            Version.changes.AddRange(AIVChange.changes);
            Version.changes.AddRange(StartTroopChange.changes);
        }

        public static void RemoveChanges(ChangeType type)
        {
            changes.RemoveAll(x => x.Type == type);
        }
    }
}

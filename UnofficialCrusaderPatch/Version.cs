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

        /**
         * Get current change list. Only HD changes.
         */
        public static List<Change> GetChangeList()
        {
            List<Change> changeList = new List<Change>();

            // Get shallow copy of modifications.
            List<Mod> mods = Version.Modifications.GetRange(0, Version.Modifications.Count);

            // Go through modifications.
            foreach (Mod mod in mods)
            {
                changeList.Add(mod.Change);
            }

            return changeList;
        }

        public static List<Change> AdditionalExternalChanges { get { return additionalExternalChanges; } }
        private static List<Change> additionalExternalChanges = new List<Change>();

        public static List<Mod> Modifications { get { return modifications; } }
        static List<Mod> modifications = new List<Mod>()
        {
            #region BUG FIXES

            new Mod_Fix_FireBallista(),
            new Mod_Fix_AI_Access(),
            new Mod_Fix_AI_Defense(),
            new Mod_Fix_AI_Tethers(),
            //new Mod_Fix_AI_Buy(),
            new Mod_Fix_AI_BuyWood(),
            new Mod_Fix_AI_TowerEngines(),
            new Mod_Fix_AI_AssaultSwitch(),
            new Mod_Fix_AI_Rebuild(),
            new Mod_Fix_AI_LaddermenUsage(),
            
            #endregion

            #region AI CHANGES
            
            new Mod_Change_AI_AttackLimit(),
            new Mod_Change_AI_AttackWave(),
            new Mod_Change_AI_AttackTarget(),
            new Mod_Change_AI_NoSleep(),
            //new Mod_Change_AI_Overclock(),
            new Mod_Change_AI_Demolish(),
            new Mod_Change_AI_AddAttack(),
            new Mod_Change_AI_RecruitInterval(),

            #endregion

            #region UNITS
            
            new Mod_Change_Units_Laddermen(),         
            new Mod_Change_Units_ArabianArcher(),
            new Mod_Change_Units_ArabianSwordsmen(),
            new Mod_Change_Units_Spearmen(),
            
            #endregion

            #region OTHER

            new Mod_Change_Other_FireCooldown(),
            new Mod_Change_Other_RemoveExtremeBar(),
            new Mod_Change_Other_PlayerColorChange(),
            new Mod_Change_Other_NewKeybindings(),
            new Mod_Change_Other_OverrideIdentityMenu(),
            new Mod_Change_Other_Healer(),
            new Mod_Change_Other_FreeTradePost(),
            new Mod_Change_Other_NoSiegeTentDeselection(),
            new Mod_Change_Other_AlwaysShowPlannedMoat(),
            new Mod_Change_Other_ExtendedGameSpeed(),
            new Mod_Change_Other_ResponsiveGates(),
            new Mod_Change_Other_OnlyAI(),
            new Mod_Change_Other_ArmoryMarketplaceWeaponOrderFix(),
            new Mod_Change_Other_DefaultMultiplayerSpeed(),
            new Mod_Change_Other_Strongholdify(),

            #endregion
        };

        public static void AddExternalChanges()
        {
            Version.additionalExternalChanges.AddRange(ResourceChange.changes);
            Version.additionalExternalChanges.AddRange(AIVChange.changes);
            Version.additionalExternalChanges.AddRange(StartTroopChange.changes);
        }

        public static void RemoveExternalChanges(ChangeType type)
        {
            additionalExternalChanges.RemoveAll(x => x.Type == type);
        }
    }
}

using System.Collections.Generic;
using UCP.Patching;

namespace UCP.Model
{
    public class Mod
    {
        #region Central Initialization
        internal static HashSet<Mod> Items { get; }

        public static Dictionary<string, IEnumerable<string>> ModList { get; }

        static Mod()
        {
            Items = new HashSet<Mod>();
            ModList = new Dictionary<string, IEnumerable<string>>();

            // Can be repaced/reordered with individual or grouped mod initializations as needed
            InitModAILords();
            InitModBugfixes();
            InitModOther();
            InitModTroops();
        }

        private static void InitModAILords()
        {
            /*new Mod_Change_AI_AddAttack();
            new Mod_Change_AI_AttackLimit();
            new Mod_Change_AI_AttackTarget();
            new Mod_Change_AI_AttackWave();
            new Mod_Change_AI_Demolish();
            new Mod_Change_AI_NoSleep();
            new Mod_Change_AI_Overclock();
            new Mod_Change_AI_RecruitInterval();*/
        }

        private static void InitModBugfixes()
        {
            /*new Mod_Fix_AI_Access();
            new Mod_Fix_AI_AssaultSwitch();
            new Mod_Fix_AI_BuyWood();
            new Mod_Fix_AI_Defense();
            new Mod_Fix_AI_LaddermenUsage();
            new Mod_Fix_AI_Rebuild();
            new Mod_Fix_AI_Tethers();
            new Mod_Fix_AI_TowerEngines();
            new Mod_Fix_FireBallista();*/
        }

        private static void InitModOther()
        {
            /*new Mod_Change_Other_AlwaysShowPlannedMoat();
            new Mod_Change_Other_ArmoryMarketplaceWeaponOrderFix();
            new Mod_Change_Other_DefaultMultiplayerSpeed();
            new Mod_Change_Other_ExtendedGameSpeed();
            new Mod_Change_Other_FireCooldown();
            new Mod_Change_Other_FreeTradePost();
            new Mod_Change_Other_Healer();
            new Mod_Change_Other_NewKeybindings();
            new Mod_Change_Other_NoSiegeTentDeselection();
            new Mod_Change_Other_OnlyAI();
            new Mod_Change_Other_OverrideIdentityMenu();
            new Mod_Change_Other_PlayerColorChange();
            new Mod_Change_Other_RemoveExtremeBar();
            new Mod_Change_Other_ResponsiveGates();
            new Mod_Change_Other_Strongholdify();*/
        }

        private static void InitModTroops()
        {
            /*new Mod_Change_Units_ArabianArcher();
            new Mod_Change_Units_ArabianSwordsmen();
            new Mod_Change_Units_Laddermen();
            new Mod_Change_Units_Spearmen();*/
        }

        #endregion

        #region Mod configuration members
        protected string identifier { get; set; }

        public string Identifier { get => identifier; }

        protected List<string> changeList { get; set; }

        public IEnumerable<string> ChangeList { get => changeList; }

        /**
         * The HD Change of the modification.
         */
        protected Change change;

        /**
         * The Extreme Change of the modification.
         */
        protected Change extremeChange;

        /**
         * The HD Change of the modification.
         */
        public Change Change { get { return change; } }

        /**
         * The Extreme Change of the modification.
         */
        public Change ExtremeChange { get { return extremeChange; } }

        public Mod(string identifier)
        {
            this.identifier = identifier;
            change = CreateChange();
            extremeChange = CreateExtremeChange();

            Items.Add(this);
            ModList.Add(this.Identifier, this.ChangeList);
        }

        /**
         * Initialize Extreme change on demand.
         * This is needed due to how the UI is stitched together tightly with Changes,
         * and how the GlobalLabels work.
         * Extreme changes are not initialized, only the HD changes at startup, and the
         * UI is built with the HD changes and NOT with the Extreme changes.
         *
         * Extreme changes are inited when the patcher runs on the Extreme file, and
         * before that, the GlobalLabels collection is emptied to avoid duplication
         * exception.
         */
        public void InitExtremeChange()
        {
            extremeChange = CreateExtremeChange();
        }

        /**
         * Creates the HD change, but also should cache it.
         */
        protected virtual Change CreateChange()
        {
            // Overridden.
            return null;
        }

        /**
         * Creates the Extreme change, but also should cache it.
         */
        protected virtual Change CreateExtremeChange()
        {
            // Overridden.
            return null;
        }

        #endregion


        #region Mod installation methods
        public void ApplyChanges(Dictionary<string, double?> modConfiguration)
        {
            if (this.change == null)
            {
                return;
            }

            foreach (DefaultSubChange header in this.change)
            {
                double? value;
                if (modConfiguration.TryGetValue(header.Identifier, out value))
                {
                    if (header is ValuedSubChange valueHeader && value != null)
                    {
                        valueHeader.Value = value.Value;
                    }
                    header.IsEnabled = true;
                }
            }
        }

        public void ApplyExtremeChanges(Dictionary<string, double?> modConfiguration)
        {
            if (this.extremeChange == null)
            {
                return;
            }

            foreach (DefaultSubChange header in this.extremeChange)
            {
                double? value;
                if (modConfiguration.TryGetValue(header.Identifier, out value))
                {
                    if (header is ValuedSubChange valueHeader && value != null)
                    {
                        valueHeader.Value = value.Value;
                    }
                    header.IsEnabled = true;
                }
            }
        }

        public void Disable()
        {
            if (this.change != null)
            {
                foreach (DefaultSubChange header in this.Change)
                {
                    header.IsEnabled = false;
                }
            }
            if (this.extremeChange != null)
            {
                foreach (DefaultSubChange header in this.extremeChange)
                {
                    header.IsEnabled = false;
                }
            }
        }

        public void Install(ChangeArgs args)
        {
            if (change != null)
            {
                change.Activate(args);
            }
        }

        public void InstallExtreme(ChangeArgs args)
        {
            if (extremeChange != null)
            {
                extremeChange.Activate(args);
            }
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using UCP.Model;

namespace UCP.Views.Controls
{
    static class UCPControls
    {
        internal static List<ControlConfig> Controls { get; private set; }

        static UCPControls() {
            Controls = new List<ControlConfig>();
        }

        internal static void ApplyConfiguration(UCPConfig config)
        {
            if (config == null)
            {
                return;
            }

            List<ChangeConfiguration> changeConfigs = config.GenericMods;
            foreach (ControlConfig cfg in UCPControls.Controls)
            {
                ConfigureControlConfig(changeConfigs, cfg);
            }
        }

        internal static List<ChangeConfiguration> GetModConfiguration()
        {
            List<ChangeConfiguration> changeConfigList = new List<ChangeConfiguration>();

            foreach (ControlConfig controlConfig in Controls.Where(x => x.Enabled() == true))
            {
                changeConfigList.Add(buildChangeConfiguration(controlConfig.Identifier, controlConfig.SubControlConfigList));
            }
            return changeConfigList;
        }

        private static ChangeConfiguration buildChangeConfiguration(string identifier, List<SubControlConfig> subControlConfigs)
        {
            ChangeConfiguration changeConfig = new ChangeConfiguration
            {
                Identifier = identifier,
                SubChanges = new Dictionary<string, double?>()
            };
            if (subControlConfigs != null)
            {
                foreach (SubControlConfig subControlConfig in subControlConfigs.Where(x => x.Enabled() == true))
                {
                    changeConfig.SubChanges.Add(subControlConfig.Identifier, subControlConfig.Value());
                }
            }
            return changeConfig;
        }

        private static void ConfigureControlConfig(List<ChangeConfiguration> changeConfigs, ControlConfig cfg)
        {
            if (!changeConfigs.Exists(x => x.Identifier.Equals(cfg.Identifier)))
            {
                foreach (SubControlConfig subCfg in cfg.SubControlConfigList)
                {
                    subCfg.SetEnabled(false);
                }
                return;
            }

            ChangeConfiguration currentConfig = changeConfigs.Single(x => x.Identifier.Equals(cfg.Identifier));
            foreach (SubControlConfig subCfg in cfg.SubControlConfigList)
            {
                ConfigureSubControlConfig(currentConfig.SubChanges, subCfg);
            }
        }

        private static void ConfigureSubControlConfig(Dictionary<string, double?> subChanges, SubControlConfig subCfg)
        {
            if (subChanges == null || !subChanges.ContainsKey(subCfg.Identifier))
            {
                subCfg.SetEnabled(false);
                return;
            }
            
            subCfg.SetEnabled(true);
            if (subChanges[subCfg.Identifier].HasValue)
            {
                subCfg.SetValue(subChanges[subCfg.Identifier].Value);
            }
        }
    }
}

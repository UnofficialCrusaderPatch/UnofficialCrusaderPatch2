import { BackendModConfig, ChangeConfig, ModConfig } from "./Config";

// initializes the initial state for the GUI
export const createStateFromProps = (config: BackendModConfig[]): Readonly<ModConfig> => {
  const initialState: ModConfig = {};

  config.forEach(function(mod: BackendModConfig) {
    const currentMod: BackendModConfig = mod;
    const currentIdentifier: string = mod.modIdentifier;
    const enabledChangeConfigs: ChangeConfig[] = [];

    currentMod.changes.forEach(function(item) {
      if (item.selectionType === 'RADIO') {
        enabledChangeConfigs.push({ identifier: item.identifier, enabled: item.defaultValue !== "false", value: item.defaultValue });
      }
      if (item.selectionType === 'CHECKBOX') {
        enabledChangeConfigs.push({ identifier: item.identifier, enabled: item.defaultValue !== "false" });
      }
      if (item.selectionType === 'SLIDER') {
        enabledChangeConfigs.push({ identifier: item.identifier, enabled: item.defaultValue !== "false", value: item.selectionParameters.default });
      }
    });
    if (enabledChangeConfigs.length > 0) {
      initialState[currentIdentifier] = enabledChangeConfigs;
    } else {
      delete initialState[currentIdentifier];
    }
  });
  return initialState;
}
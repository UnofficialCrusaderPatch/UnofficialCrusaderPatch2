import { BackendModConfig, ChangeConfig, ModConfig } from "./Config";

// initializes the initial state for the GUI
export const createInitialStateFromProps = (config: BackendModConfig[]): Readonly<ModConfig> => {
  const initialState: ModConfig = {};

  // initialize entry for each mod in the config
  config.forEach(function(mod: BackendModConfig) {
    const currentMod: BackendModConfig = mod;
    const currentIdentifier: string = mod.modIdentifier;
    const changeConfigs: ChangeConfig[] = [];

    // initialize entry for each change in a mod
    currentMod.changes.forEach(function(item) {
      if (item.selectionType === 'RADIO') {
        changeConfigs.push({ identifier: item.identifier, enabled: item.defaultValue !== "false", value: item.defaultValue });
      }
      if (item.selectionType === 'CHECKBOX') {
        changeConfigs.push({ identifier: item.identifier, enabled: item.defaultValue !== "false" });
      }
      if (item.selectionType === 'SLIDER') {
        changeConfigs.push({ identifier: item.identifier, enabled: item.defaultValue !== "false", value: item.selectionParameters.default });
      }
    });

    // assign change configs to the mod within state object
    initialState[currentIdentifier] = changeConfigs;
  });
  return initialState;
}
import { BackendModConfig, ModState } from "./Config";

export interface AppState {
  [identifier: string]: ModState;
}

// initializes the initial state for the GUI
export const createInitialStateFromProps = (config: BackendModConfig[]): Readonly<AppState> => {
  const initialState: AppState = {};

  // initialize entry for each mod in the config
  config.forEach(function(mod: BackendModConfig) {
    const currentMod: BackendModConfig = mod;
    const currentIdentifier: string = mod.modIdentifier;
    const modState: ModState = {};
    // const changeState: ChangeState = {};

    // initialize entry for each change in a mod
    currentMod.changes.forEach(function(item) {
      if (item.selectionType === 'RADIO') {
        modState[item.identifier] = { enabled: item.defaultValue !== "false", value: item.defaultValue };
      }
      if (item.selectionType === 'CHECKBOX') {
        modState[item.identifier] = { enabled: item.defaultValue !== "false" };
      }
      if (item.selectionType === 'SLIDER') {
        modState[item.identifier] = { enabled: item.defaultValue !== "false", value: item.selectionParameters.default };
      }
    });

    // assign change configs to the mod within state object
    initialState[currentIdentifier] = modState;
  });
  return initialState;
}
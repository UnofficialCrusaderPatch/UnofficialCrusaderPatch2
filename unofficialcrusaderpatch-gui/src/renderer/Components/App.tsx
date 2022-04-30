import * as React from 'react';
import { TabLayout } from './Layout/TabView/TabLayout';
import { Header } from './Header';
import { Provider, TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';
import { ChangeConfig, ModConfig, rootReducer } from './reducers';
import { AnyAction, configureStore, Store } from '@reduxjs/toolkit';

export interface AppProps {
  config: BackendModConfig[];
}

export type RadioOption = {
  description: string;
  type: 'string' | 'color';
  value: string;
}

export type RadioParams = {
  options: ArrayOfTwoOrMore<RadioOption>;
};

export type SliderParams = {
  default: number;
  interval: number;
  maximum: number;
  minimum: number;
  suggested: number;
};

export type SelectionParams = RadioParams | SliderParams;

export type BaseChangeConfig = {
  compatibility? : string[];
  defaultValue: string;
  description: string;
  detailedDescription?: string;
  identifier: string;
}

export type CheckboxChangeConfig = {
  selectionType: 'CHECKBOX'
} & BaseChangeConfig;

export type RadioChangeConfig = {
  selectionType: 'RADIO'
  selectionParameters: RadioParams
} & BaseChangeConfig;

export type SliderChangeConfig = {
  selectionType: 'SLIDER'
  selectionParameters: SliderParams
} & BaseChangeConfig;

export type BackendChangeConfig = CheckboxChangeConfig | RadioChangeConfig | SliderChangeConfig;

export type ArrayOfOneOrMore<T> = [T, ...T[]];
export type ArrayOfTwoOrMore<T> = [T, T, ...T[]];

export type BackendModConfig = {
  changes: ArrayOfOneOrMore<BackendChangeConfig>;
  detailedDescription?: string;
  modDescription: string;
  modIdentifier: string;
  modSelectionRule?: string | null | undefined;
  modType: string;
}

export let store: Store<ModConfig, AnyAction>;
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

/*
 * Root element for the UCP GUI
 */
export class AppLayout extends React.Component<AppProps, {}> {
  constructor(props: AppProps) {
    super(props);
    store = configureStore({ reducer: rootReducer, preloadedState: createStateFromProps(props.config)});
  }

  render() {
    console.log(store.getState());
    return (
      <Provider store={store}>
        <Header/>
        {this.getTabLayout(this.props.config)}
      </Provider>
    );
  }

  // returns the Tab-based layout for the GUI
  getTabLayout = (configList: BackendModConfig[]): React.ReactElement => {
    const uniqueModTypes: string[] = this.getUniqueModTypes(configList);
    return (
      <TabLayout options={configList} modTypes={uniqueModTypes}></TabLayout>
    );
  };

  // returns the list of unique mod types ie. Bugfixes, AIV, Other
  getUniqueModTypes = (configList: BackendModConfig[]): string[] => {
    const uniqueModTypes: string[] = [];

    configList.forEach((config) => {
      if (uniqueModTypes.indexOf(config.modType) === -1) {
        uniqueModTypes.push(config.modType);
      }
    });

    return uniqueModTypes;
  };
}

const createStateFromProps = (config: BackendModConfig[]): Readonly<{}> => {
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
        enabledChangeConfigs.push({ identifier: item.identifier, enabled: item.defaultValue !== "false", value: item.selectionParameters!.default });
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
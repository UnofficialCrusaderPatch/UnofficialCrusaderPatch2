import * as React from 'react';
import { Provider, TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

import { AnyAction, Store, configureStore } from '@reduxjs/toolkit';

import { Header } from './Header';
import { TabLayout } from './Layout/TabView/TabLayout';
import { BackendModConfig, ModConfig } from './Config';
import { rootReducer } from './Reducer';
import { createStateFromProps } from './State';

export interface AppProps {
  config: BackendModConfig[];
}

export let store: Store<ModConfig, AnyAction>;
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

/*
 * Root element for the UCP GUI
 */
export class AppLayout extends React.Component<AppProps> {
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
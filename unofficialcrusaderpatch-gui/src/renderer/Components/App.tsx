import * as React from 'react';
import { Provider, TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

import { AnyAction, Store, configureStore } from '@reduxjs/toolkit';

import { Header } from './Header';
import { TabLayout } from './Layout/TabView/TabLayout';
import { BackendModConfig, ModConfig } from './Config';
import { rootReducer } from './Reducer';
import { createInitialStateFromProps } from './State';

export let store: Store<ModConfig, AnyAction>;

// create typed methods specific to state
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;


// config as input for the AppLayout element
export interface AppProps {
  config: BackendModConfig[];
}

/*
 * Root element for the UCP GUI
 */
export class AppLayout extends React.Component<AppProps> {
  constructor(props: AppProps) {
    super(props);

    // create a store with initial state given the config
    store = configureStore({ reducer: rootReducer, preloadedState: createInitialStateFromProps(props.config)});
  }

  // return the content wrapped in a Provider so that components re-render when state is updated
  render() {
    console.log(store.getState());
    return (
      <Provider store={store}>
        <Header/>
        {this.getTabLayout(this.props.config)}
      </Provider>
    );
  }

  // returns the Tab-based layout for the GUI, one tab per unique mod type
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
import * as React from 'react';

import { TabLayout } from './Layout/TabView/TabLayout';
import { BackendModConfig } from './Config';
import { AppState, createInitialStateFromProps } from './State';


// config as input for the AppLayout element
export interface AppProps {
  config: BackendModConfig[];
}

/*
 * Root element for the UCP GUI
 */
export class AppLayout extends React.Component<AppProps, AppState> {
  constructor(props: AppProps) {
    super(props);

    this.state = createInitialStateFromProps(props.config);
  }

  // return the content wrapped in a Provider so that components re-render when state is updated
  render() {
    return this.getTabLayout(this.props.config, this.state);
  }

  // returns the Tab-based layout for the GUI, one tab per unique mod type
  getTabLayout = (configList: BackendModConfig[], state: AppState): React.ReactElement => {
    const uniqueModTypes: string[] = this.getUniqueModTypes(configList);
    return (
      <TabLayout options={configList} modTypes={uniqueModTypes} initialState={state}></TabLayout>
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
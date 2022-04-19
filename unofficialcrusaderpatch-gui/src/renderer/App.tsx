import * as React from 'react';
import { View } from './View';
import { TabLayout } from './View/TabLayout';

export interface AppProps {
  config: any[];
}

/*
 * Root element for the UCP GUI
 */
export class AppLayout extends React.Component<AppProps, {}> {
  render() {
    return (
      <React.Fragment>
        <View>{this.getTabLayout(this.props.config)}</View>
      </React.Fragment>
    );
  }

  // returns the Tab-based layout for the GUI
  getTabLayout = (configList: any[]): React.ReactElement => {
    const uniqueModTypes = this.getUniqueModTypes(configList);
    return (
      <TabLayout options={configList} modTypes={uniqueModTypes}></TabLayout>
    );
  };

  // returns the list of unique mod types ie. Bugfixes, AIV, Other
  getUniqueModTypes = (configList: any[]): string[] => {
    const uniqueModTypes: string[] = [];
    configList.forEach((config) => {
      if (uniqueModTypes.indexOf(config.modType) === -1) {
        uniqueModTypes.push(config.modType);
      }
    });
    return uniqueModTypes;
  };
}

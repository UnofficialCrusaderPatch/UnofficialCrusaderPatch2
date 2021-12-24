import * as React from 'react';
import { View } from './View';
import { TabLayout } from './View/TabLayout';

export interface AppProps {
  config: any[];
}

export class AppLayout extends React.Component<AppProps, {}> {
  render() {
    return (
      <React.Fragment>
        <View>{this.getTabLayout(this.props.config)}</View>
      </React.Fragment>
    );
  }

  getTabLayout = (configList: any[]): React.ReactElement => {
    const uniqueModTypes = this.getUniqueModTypes(configList);
    return (
      <TabLayout options={configList} modTypes={uniqueModTypes}></TabLayout>
    );
  };

  getUniqueModTypes = (configList: any[]): string[] => {
    const uniqueModTypes: string[] = [];
    configList.forEach((config, index) => {
      if (uniqueModTypes.indexOf(config.modType) === -1) {
        uniqueModTypes.push(config.modType);
      }
    });
    return uniqueModTypes;
  };
}

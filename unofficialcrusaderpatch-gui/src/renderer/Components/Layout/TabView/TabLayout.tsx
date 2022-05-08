import * as React from 'react';

import { BackendModConfig } from '../../Config';

import { TabContent } from './TabContent';
import { TabHeader } from './TabHeader';

/**
 * Renders the Tab-based layout for the GUI
 */
export class TabLayout extends React.Component<{
  options: BackendModConfig[];
  modTypes: string[];
}> {

  render() {
    return (
      <React.Fragment>
        <ul className='nav nav-tabs' id='myTabHeaders'>
          {this.getTabHeaders(this.props.modTypes)}
        </ul>
        <div className='tab-content' id='myTabContent'>
          {this.getTabContent(this.props.options, this.props.modTypes)}
        </div>
      </React.Fragment>
    );
  }

  // renders the set of Tab headers
  getTabHeaders(modTypes: string[]) {
    return modTypes.map((currentModType) => {
      const elementKey: string = 'ucp-tab-header-' + currentModType;
      return (
      <TabHeader
        modType={currentModType}
        key={elementKey}
      />
      );
    });
  }

  // renders the set of Tab contents
  getTabContent(options: BackendModConfig[], modTypes: string[]) {
    return modTypes.map((currentModType, currentModIndex) => {
      const modTypeOptions = options.filter(
        (option) => option.modType === currentModType
      );
      const elementKey: string = 'ucp-tab-content-' + currentModType + '-' + currentModIndex;
      return (
        <TabContent
          modList={modTypeOptions}
          modType={currentModType}
          key={elementKey}
        />
      );
    });
  }
}

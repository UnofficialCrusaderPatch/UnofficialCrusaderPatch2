import * as React from 'react';
import { TabContent } from './TabContent';
import { TabHeader } from './TabHeader';

/**
 * Renders the Tab-based layout for the GUI
 */
export class TabLayout extends React.Component<{
  options: any[];
  modTypes: string[];
}> {
  componentDidMount() {}

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
    return modTypes.map((currentModType) => (
      <TabHeader
        modType={currentModType}
        key={'ucp-tab-header-' + currentModType}
      />
    ));
  }

  // renders the set of Tab contents
  getTabContent(options: any[], modTypes: string[]) {
    return modTypes.map((currentModType, currentModIndex) => {
      const modTypeOptions = options.filter(
        (option) => option.modType === currentModType
      );
      return (
        <TabContent
          modList={modTypeOptions}
          modType={currentModType}
          key={'ucp-tab-content-' + currentModType + '-' + currentModIndex}
        />
      );
    });
  }
}

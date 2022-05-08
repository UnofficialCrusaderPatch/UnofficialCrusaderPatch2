import * as React from 'react';

import { BackendModConfig } from '../../Config';
import { ModLayout } from '../../Options/ModLayout';

/**
 * Renders the Tab content corresponding to a single mod type
 */ 
export class TabContent extends React.Component<{
  modList: BackendModConfig[];
  modType: string;
}> {

  render() {
    const ariaLabel: string = 'ucp-tab-' + this.props.modType;
    return (
      <React.Fragment>
        <div
          className='tab-pane fade'
          id={this.props.modType}
          key={this.props.modType}
          role='tabpanel'
          aria-labelledby={ariaLabel}
        >
          {this.getOptionElements(this.props.modList, this.props.modType)}
        </div>
      </React.Fragment>
    );
  }

  getOptionElements(modList: BackendModConfig[], _: string): React.ReactNode {
    return modList.map((mod, modIndex) => {
      const elementKey: string = 'modlayout-' + mod.modIdentifier + '-' + modIndex;
      return <ModLayout mod={mod} modIndex={modIndex} key={elementKey}></ModLayout>;
    });
  }
}

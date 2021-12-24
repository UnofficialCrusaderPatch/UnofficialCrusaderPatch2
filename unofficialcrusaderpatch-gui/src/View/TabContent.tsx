import * as React from 'react';
import { ModLayout } from 'src/Components/Options/ModLayout';

export class TabContent extends React.Component<{
  modList: any[];
  modType: string;
}> {
  componentDidMount() {}

  render() {
    return (
      <React.Fragment>
        <div
          className='tab-pane fade'
          id={this.props.modType}
          key={this.props.modType}
          role='tabpanel'
          aria-labelledby={'ucp-tab-' + this.props.modType}
        >
          {this.getOptionElements(this.props.modList, this.props.modType)}
        </div>
      </React.Fragment>
    );
  }

  getOptionElements(modList: any[], modType: string): React.ReactNode {
    return modList.map((mod, modIndex) => {
      return <ModLayout mod={mod} modIndex={modIndex} key={'modlayout-' + mod.modIdentifier + '-' + modIndex}></ModLayout>;
    });
  }
}

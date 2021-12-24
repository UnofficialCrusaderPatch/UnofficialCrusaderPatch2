import * as React from 'react';
import { ModBody } from './ModBody';
import { ModHeader } from './ModHeader';

export class ModLayout extends React.Component<{mod: any, modIndex: number}> {
  componentDidMount() {}

  render() {
    const modType = this.props.mod.modType;
    const modIndex = this.props.modIndex;
    const modIdentifier = this.props.mod.modIdentifier;
    return (
      <React.Fragment key={'mod-root' + modType + '-' + modIdentifier + '-' + modIndex}>
        <ModHeader mod={this.props.mod} modIndex={modIndex}></ModHeader>
        <ModBody mod={this.props.mod} modIndex={modIndex}></ModBody>
      </React.Fragment>
    );
  }
}

import * as React from 'react';
import { ModBody } from './ModBody';
import { ModHeader } from './ModHeader';

/**
 * Container for a single mod element with a header and a body
 */
export class ModLayout extends React.Component<{mod: any, modIndex: number}> {
  componentDidMount() {}

  render() {
    const modType: string = this.props.mod.modType;
    const modIndex: number = this.props.modIndex;
    const modIdentifier: string = this.props.mod.modIdentifier;
    const elementKey: string = 'mod-root' + modType + '-' + modIdentifier + '-' + modIndex;
    return (
      <React.Fragment key={elementKey}>
        <ModHeader mod={this.props.mod} modIndex={modIndex}></ModHeader>
        <ModBody mod={this.props.mod} modIndex={modIndex}></ModBody>
      </React.Fragment>
    );
  }
}

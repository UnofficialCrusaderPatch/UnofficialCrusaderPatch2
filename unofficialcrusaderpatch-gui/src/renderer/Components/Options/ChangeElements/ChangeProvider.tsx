import * as React from 'react';
import { Checkbox } from './Checkbox';
import { Radio } from './Radio';
import { Slider } from './Slider';

/**
 * Renders a single radio option
 */
export class ChangeProvider extends React.Component<{ mod: any, change: any }> {
  componentDidMount() {}

  render() {
    switch (this.props.change.selectionType) {
      case 'CHECKBOX':
        return <Checkbox mod={this.props.mod} change={this.props.change}></Checkbox>;
      case 'RADIO':
        return <Radio mod={this.props.mod} change={this.props.change}></Radio>
      case 'SLIDER':
        return <Slider mod={this.props.mod} change={this.props.change}></Slider>;
    }
    return <React.Fragment/>;
  }
}

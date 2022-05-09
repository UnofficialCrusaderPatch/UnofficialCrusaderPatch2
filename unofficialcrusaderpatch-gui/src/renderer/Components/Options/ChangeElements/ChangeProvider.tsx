import * as React from 'react';

import { BackendChangeConfig, BackendModConfig } from '../../Config';

import { Checkbox } from './Checkbox';
import { Radio } from './Radio';
import { Slider } from './Slider';

/**
 * Renders a single option based on its selection type
 */
export class ChangeProvider extends React.Component<{ mod: BackendModConfig, change: BackendChangeConfig, isEnabled: boolean, selectedValue?: string | number, onchange: (enabled: boolean, identifier: string) => void }> {

  render() {
    switch (this.props.change.selectionType) {
      case 'CHECKBOX':
        return <Checkbox mod={this.props.mod} change={this.props.change} isChecked={this.props.isEnabled} onchange={this.props.onchange.bind(this)}></Checkbox>;
      case 'RADIO':
        return <Radio mod={this.props.mod} change={this.props.change} selected={this.props.selectedValue as string} onchange={this.props.onchange.bind(this)}></Radio>
      case 'SLIDER':
        return <Slider mod={this.props.mod} change={this.props.change} selectedValue={this.props.selectedValue as number} hasHeader={true} isEnabled={this.props.isEnabled} onchange={this.props.onchange.bind(this)}></Slider>;
    }
    return <React.Fragment/>;
  }
}

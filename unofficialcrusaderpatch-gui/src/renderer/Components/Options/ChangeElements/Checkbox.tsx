import * as React from 'react';

import { BackendModConfig, CheckboxChangeConfig } from '../../Config';

/**
 * Renders a single checkbox option
 */
export class Checkbox extends React.Component<{ mod: BackendModConfig, change: CheckboxChangeConfig, isChecked: boolean, onchange: (enabled: boolean, identifier: string) => void }> {

  render() {
    const elementUniqueId: string = this.props.mod.modIdentifier + '-' + this.props.change.identifier;
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          <input
            className='form-check-input ucp-select'
            type='checkbox'
            name={elementUniqueId}
            checked={this.props.isChecked}
            id={elementUniqueId}
            key={elementUniqueId}
            onChange={(e) => this.props.onchange(e.currentTarget.checked, this.props.change.identifier)}
          />
          <label
            className='form-check-label ucp-change-text'
            htmlFor={elementUniqueId}
          >
            {this.props.change.description}
          </label>
        </div>
      </React.Fragment>
    );
  }
}

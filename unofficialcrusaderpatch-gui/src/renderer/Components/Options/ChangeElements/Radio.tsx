import * as React from 'react';

import { BackendModConfig, RadioChangeConfig, RadioOption } from '../../Config';

/**
 * Renders a single radio option
 */
export class Radio extends React.Component<{ mod: BackendModConfig, change: RadioChangeConfig, selected: string, onchange: (enabled: boolean, identifier: string, value: string) => void }> {

  render() {
    const mod: BackendModConfig = this.props.mod;
    const change: RadioChangeConfig = this.props.change;

    const containerElementUniqueId: string = mod.modIdentifier + '-' + change.identifier;

    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          {change.selectionParameters.options.map((option: RadioOption, optionIndex: number) => {
            return <React.Fragment key={containerElementUniqueId + "-selectionParameter-" + optionIndex}>
              <input
                className='form-check-input ucp-select'
                type='radio'
                name={containerElementUniqueId}
                value={option.value}
                id={containerElementUniqueId}
                key={containerElementUniqueId + '-selectionParameter-input-' + optionIndex}
                checked={option.value === this.props.selected}
                onChange={(e) => this.props.onchange(e.currentTarget.checked, this.props.change.identifier, option.value)}
              />
              <label
                className='form-check-label ucp-change-text'
                htmlFor={containerElementUniqueId}
              >
                { typeof option.description === 'string' && option.description }
              </label>
            </React.Fragment>;
          })}
        </div>
      </React.Fragment>
    );
  }
}

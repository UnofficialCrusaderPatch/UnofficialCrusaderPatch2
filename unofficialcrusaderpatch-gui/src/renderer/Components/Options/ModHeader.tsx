import * as React from 'react';

import { BackendModConfig, ModState } from '../Config';

/**
 * Header element for a single mod element
 */
export class ModHeader extends React.Component<{
  mod: BackendModConfig,
  modIndex: number,
  config: ModState,
  onchange: (enabled: boolean) => void
}> {

  render() {
    const modType = this.props.mod.modType;
    const modIndex = this.props.modIndex;
    const modIdentifier = this.props.mod.modIdentifier;
    const modDescription = this.props.mod.modDescription;
    return (
      <React.Fragment>
        <div className='ucp-select-header' key={'ucp-header' + modType + '-' + modIdentifier + '-' + modIndex}>
          <input
            className='form-check-input ucp-select'
            type='checkbox'
            value={modIdentifier}
            id={modType + '-' + modIdentifier}
            key={'input' + '-' + modType + '-' + modIdentifier + '-' + modIndex}
            onChange={e => {this.props.onchange(e.currentTarget.checked)}}
            checked={Object.values(this.props.config).some(x => x.enabled === true)}
          />
          <label
            className='form-check-label ucp-select-text'
            htmlFor={modType + '-' + modIdentifier}
            key={'label' + modType + '-' + modIdentifier + '-' + modIndex}
          >
            {modDescription}
          </label>
          <span
            className='badge rounded-pill bg-info text-dark ucp-select-info'
            data-bs-toggle='collapse'
            key={'info-' + modType + '-' + modIdentifier + '-' + modIndex}
            data-bs-target={'#' + modType + '-' + modIdentifier + '-' + modIndex}
            aria-expanded='false'
            aria-controls={'#' + modType + '-' + modIndex}
          >
            Info
          </span>
        </div>
      </React.Fragment>
    )
  }
}

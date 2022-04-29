import * as React from 'react';

/**
 * Header element for a single mod element
 */
export class ModHeader extends React.Component<{mod: any, modIndex: number, onchange: (enabled: boolean, modIdentifier: string) => void}> {
  componentDidMount() {}

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
            onChange={e => {this.props.onchange(e.currentTarget.checked, modIdentifier)}}
            //onChange={e => {store.dispatch({type:modIdentifier, value: e.currentTarget.checked})}}
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

import * as React from 'react';

/**
 * Renders a single radio option
 */
export class Radio extends React.Component<{ mod: any, change: any }> {
  componentDidMount() {}

  render() {
    const mod: any = this.props.mod;
    const change: any = this.props.change;

    const containerElementUniqueId: string = mod.modIdentifier + '-' + change.identifier;

    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          {change.selectionParameters.options.map((option: any, optionIndex: number) => {
            return <React.Fragment key={containerElementUniqueId + "-selectionParameter-" + optionIndex}>
              <input
                className='form-check-input ucp-select'
                type='radio'
                name={containerElementUniqueId}
                value={option}
                id={containerElementUniqueId}
                key={containerElementUniqueId + '-selectionParameter-input-' + optionIndex}
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

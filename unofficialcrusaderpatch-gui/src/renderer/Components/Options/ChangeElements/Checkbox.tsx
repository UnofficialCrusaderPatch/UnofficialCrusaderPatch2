import * as React from 'react';

/**
 * Renders a single checkbox option
 */
export class Checkbox extends React.Component<{ mod: any, change: any }> {
  componentDidMount() {}

  render() {
    const elementUniqueId: string = this.props.mod.modIdentifier + '-' + this.props.change.identifier;
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          <input
            className='form-check-input ucp-select'
            type='checkbox'
            name={elementUniqueId}
            defaultChecked={this.props.change.defaultValue}
            id={elementUniqueId}
            key={elementUniqueId}
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

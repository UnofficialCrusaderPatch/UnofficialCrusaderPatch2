import * as React from 'react';

/**
 * Renders a single slider option
 */
export class Slider extends React.Component<{ mod: any, change: any }> {
  componentDidMount() {}

  render() {
    const elementUniqueId: string = this.props.mod.modIdentifier + '-' + this.props.change.identifier;
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          <input
            className='ucp-slider'
            type='range'
            defaultValue={this.props.change.selectionParameters.suggested}
            min={this.props.change.selectionParameters.minimum}
            max={this.props.change.selectionParameters.maximum}
            step={this.props.change.selectionParameters.interval}
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

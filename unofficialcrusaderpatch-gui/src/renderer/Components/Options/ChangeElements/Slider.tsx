import * as React from 'react';
import { BackendModConfig, SliderChangeConfig } from 'renderer/Components/App';

/**
 * Renders a single slider option
 */
export class Slider extends React.Component<{ mod: BackendModConfig, change: SliderChangeConfig, selectedValue: number, onchange: (enabled: boolean, identifier: string, value: number) => void }> {
  componentDidMount() {}

  render() {
    const elementUniqueId: string = this.props.mod.modIdentifier + '-' + this.props.change.identifier;
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          <input
            className='ucp-slider'
            type='range'
            min={this.props.change.selectionParameters.minimum}
            max={this.props.change.selectionParameters.maximum}
            step={this.props.change.selectionParameters.interval}
            id={elementUniqueId}
            key={elementUniqueId}
            value={this.props.selectedValue}
            onChange={(e) => this.props.onchange(parseInt(e.currentTarget.value) !== this.props.change.selectionParameters.default, this.props.change.identifier, parseInt(e.currentTarget.value))}
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

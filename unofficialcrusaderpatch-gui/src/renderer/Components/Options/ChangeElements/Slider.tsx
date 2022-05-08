import * as React from 'react';
import { BackendModConfig, SliderChangeConfig } from 'renderer/Components/App';

/**
 * Renders a single slider option
 */
export class Slider extends React.Component<{ mod: BackendModConfig, change: SliderChangeConfig, hasHeader?: boolean, isEnabled: boolean, selectedValue: number, onchange: (enabled: boolean, identifier: string, value?: number) => void }> {
  componentDidMount() {}

  render() {
    const elementUniqueId: string = this.props.mod.modIdentifier + '-' + this.props.change.identifier;
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          {this.getSlider(elementUniqueId, this.props.hasHeader)}
        </div>
      </React.Fragment>
    );
  }

  getSlider = (elementUniqueId: string, hasHeader?: boolean) => {
    return <React.Fragment>
      {hasHeader && this.getHeader(elementUniqueId)}
      <div>
        <span className='ucp-slider-range'>{this.props.change.selectionParameters.minimum}</span>
        <input
          className='ucp-slider ucp-slider-range'
          type='range'
          min={this.props.change.selectionParameters.minimum}
          max={this.props.change.selectionParameters.maximum}
          step={this.props.change.selectionParameters.interval}
          id={elementUniqueId}
          key={elementUniqueId}
          value={this.props.selectedValue}
          onChange={(e) => this.onChangeHandler(e, elementUniqueId)}
        />
        <span className='ucp-slider-range'>{this.props.change.selectionParameters.maximum}</span>
      </div>
      <div className='ucp-slider-values'>
        <span>  Current value: </span><span className='ucp-slider-range' id={elementUniqueId + '-value'}>{this.props.selectedValue}</span>
        <span>  Default value: {this.props.change.selectionParameters.default}</span>
        <span>  Suggested: {this.props.change.selectionParameters.suggested}</span>
      </div>
      <label
        className='form-check-label ucp-change-text'
        htmlFor={elementUniqueId}
      >
        {!hasHeader && this.props.change.description}
        {this.props.change.detailedDescription}
      </label>
    </React.Fragment>
  }

  getHeader = (elementUniqueId: string) => {
    return (
      <React.Fragment>
        <input
          type='checkbox'
          id={elementUniqueId + '-header'}
          checked={this.props.isEnabled}
          className='ucp-change-header'
          onChange={(e) => this.props.onchange(e.currentTarget.checked, this.props.change.identifier)}
        />
        <label
        className='form-check-label ucp-change-text ucp-change-header'
        htmlFor={elementUniqueId + '-header'}
      >
        {this.props.change.description}
      </label>
      </React.Fragment>
    );
  }

  onChangeHandler = (e: React.ChangeEvent<HTMLInputElement>, elementUniqueId: string) => {
    this.props.onchange(parseInt(e.currentTarget.value) !== this.props.change.selectionParameters.default, this.props.change.identifier, parseInt(e.currentTarget.value))
    document.getElementById(`${elementUniqueId}-value`)!.innerText = e.currentTarget.value;
  }
}

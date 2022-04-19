import * as React from 'react';

/**
 * Body element for a single mod element
 */
export class ModBody extends React.Component<{mod: any, modIndex: number}> {
  componentDidMount() {}

  // render the mod as the set of its comprising changes followed by a comprehensive detailed description
  render() {
    const modType = this.props.mod.modType;
    const modIndex = this.props.modIndex;
    const modIdentifier = this.props.mod.modIdentifier;
    const detailedDescription = this.props.mod.detailedDescription;
    return (
      <React.Fragment>
        <div
          id={modType + '-' + modIdentifier + '-' + modIndex}
          key={modType + '-' + modIdentifier + '-' + modIndex}
          className='accordion-collapse collapse description modBody'
          aria-labelledby={modType + '-' + modIdentifier}
        >
          {this.getBody(this.props.mod)}
          {detailedDescription !== undefined && detailedDescription}
        </div>
      </React.Fragment>
    )
  }

  // returns the body elements that make up a single mod
  getBody(mod: any): React.ReactNode {
    if (mod.changes === []) {
      return null;
    }

    /**
     * when a mod contains a single change render it as a single element
     * and use detailed description as the text to show
     * when the only change is a checkbox do not render a nested checkbox
     */
    if (mod.changes.length === 1) {
      const change = mod.changes[0];
      return (
        <React.Fragment key={'ucp-change-' + change.identifier}>
          {change.selectionType === 'RADIO' && this.getRadioChange(mod, change)}
          {change.selectionType === 'SLIDER' && this.getSliderChange(mod, change)}
          {change.selectionType === 'CHECKBOX' && change.description}
          {change.detailedDescription !== undefined && change.detailedDescription}
        </React.Fragment>
      );
    } else {
      return this.getChangeElements(mod);
    }
  }

  // with more than 1 change, each will have their own toggle checkbox and 
  // for non-checkbox changes nested selection options as well
  getChangeElements(mod: any): React.ReactNode {
    if (mod.changes === []) {
      return null;
    }

    return mod.changes.map((change: any) => {
      return <React.Fragment key={'ucp-change-' + change.identifier}>
        {change.selectionType === 'CHECKBOX' && this.getSelectChange(mod, change)}
        {change.selectionType === 'RADIO' && this.getRadioChange(mod, change)}
        {change.selectionType === 'SLIDER' && this.getSliderChange(mod, change)}
        {change.detailedDescription !== undefined && change.detailedDescription}
      </React.Fragment>;
    });
  }

  // constructs a single checkbox option
  getSelectChange(mod: any, change: any): React.ReactNode {
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          <input
            className='form-check-input ucp-select'
            type='checkbox'
            name={mod.modIdentifier + '-' + change.identifier}
            defaultChecked={change.defaultValue}
            id={mod.modIdentifier + '-' + change.identifier}
            key={mod.modIdentifier + '-' + change.identifier}
          />
          <label
            className='form-check-label ucp-change-text'
            htmlFor={mod.modIdentifier + '-' + change.identifier}
          >
            {change.description}
          </label>
        </div>
      </React.Fragment>
    );
  }

  // constructs a single radio option
  getRadioChange(mod: any, change: any): React.ReactNode {
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          {change.selectionParameters.options.map((option: any, optionIndex: number) => {
            return <React.Fragment key={mod.modIdentifier + '-' + change.identifier + "-selectionParameter-" + optionIndex}>
              <input
                className='form-check-input ucp-select'
                type='radio'
                name={mod.modIdentifier + '-' + change.identifier}
                value={option}
                id={mod.modIdentifier + '-' + change.identifier}
                key={mod.modIdentifier + '-' + change.identifier + '-selectionParameter-input-' + optionIndex}
              />
              <label
                className='form-check-label ucp-change-text'
                htmlFor={mod.modIdentifier + '-' + change.identifier}
              >
                { typeof option.description === 'string' && option.description }
              </label>
            </React.Fragment>;
          })}
        </div>
      </React.Fragment>
    );
  }

  // constructs a single slider option
  getSliderChange(mod: any, change: any): React.ReactNode {
    return (
      <React.Fragment>
        <div className='ucp-select-header'>
          <input
            className='ucp-slider'
            type='range'
            defaultValue={change.selectionParameters.suggested}
            min={change.selectionParameters.minimum}
            max={change.selectionParameters.maximum}
            step={change.selectionParameters.interval}
            id={mod.modIdentifier + '-' + change.identifier}
            key={mod.modIdentifier + '-' + change.identifier}
          />
          <label
            className='form-check-label ucp-change-text'
            htmlFor={mod.modIdentifier + '-' + change.identifier}
          >
            {change.description}
          </label>
        </div>
      </React.Fragment>
    );
  }
}

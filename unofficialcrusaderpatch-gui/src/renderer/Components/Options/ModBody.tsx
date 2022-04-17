import * as React from 'react';

export class ModBody extends React.Component<{mod: any, modIndex: number}> {
  componentDidMount() {}

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

  getBody(mod: any): React.ReactNode {
    if (mod.changes === []) {
      return null;
    }

    if (mod.changes.length === 1) {
      const change = mod.changes[0];
      return (
        <React.Fragment key={'ucp-change-' + change.identifier}>
          {change.selectionType === 'radio' && this.getRadioChange(mod, change)}
          {change.selectionType === 'slider' && this.getSliderChange(mod, change)}
          {change.selectionType === 'checkbox' && change.description !== undefined && change.description}
          {change.detailedDescription !== undefined && change.detailedDescription}
        </React.Fragment>
      );
    } else {
      return this.getChangeElements(mod);
    }

    // more than 1 change, so each will have its own toggle checkbox
  }

  getChangeElements(mod: any): React.ReactNode {
    if (mod.changes === []) {
      return null;
    }

    return mod.changes.map((change: any) => {
      return <React.Fragment key={'ucp-change-' + change.identifier}>
        {change.selectionType === 'checkbox' && this.getSelectChange(mod, change)}
        {change.selectionType === 'radio' && this.getRadioChange(mod, change)}
        {change.selectionType === 'slider' && this.getSliderChange(mod, change)}
        {change.detailedDescription !== undefined && change.detailedDescription}
      </React.Fragment>;
    });
  }

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
            htmlFor={change.modIdentifier + '-' + change.identifier}
          >
            {change.description}
          </label>
        </div>
      </React.Fragment>
    );
  }

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
                htmlFor={change.modIdentifier + '-' + change.identifier}
              >
                { typeof option.description === 'string' && option.description }
              </label>
            </React.Fragment>;
          })}
        </div>
      </React.Fragment>
    );
  }

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
            htmlFor={change.modIdentifier + '-' + change.identifier}
          >
            {change.description}
          </label>
        </div>
      </React.Fragment>
    );
  }
}

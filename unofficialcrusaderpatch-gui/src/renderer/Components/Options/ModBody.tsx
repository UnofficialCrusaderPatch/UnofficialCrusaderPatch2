import * as React from 'react';

import { BackendChangeConfig, BackendModConfig, ModState } from '../Config';

import { ChangeProvider } from './ChangeElements/ChangeProvider';
import { Radio } from './ChangeElements/Radio';
import { Slider } from './ChangeElements/Slider';

/**
 * Body element for a single mod element
 */
export class ModBody extends React.Component<{
  mod: BackendModConfig,
  modIndex: number,
  config: ModState,
  onchange: ( enabled: boolean, identifier: string) => void
}> {

  // render the mod as the set of its comprising changes followed by a comprehensive detailed description
  render() {
    const modType: string = this.props.mod.modType;
    const modIndex: number = this.props.modIndex;
    const modIdentifier: string = this.props.mod.modIdentifier;
    const detailedDescription: string | undefined = this.props.mod.detailedDescription;

    const elementUniqueId: string = modType + '-' + modIdentifier + '-' + modIndex;
    const ariaLabel: string = modType + '-' + modIdentifier;
    return (
      <React.Fragment>
        <div
          id={elementUniqueId}
          key={elementUniqueId}
          className='accordion-collapse collapse description modBody'
          aria-labelledby={ariaLabel}
        >
          {this.getBody(this.props.mod, this.props.config, this.props.onchange)}
          {detailedDescription !== undefined && detailedDescription}
        </div>
      </React.Fragment>
    )
  }

  // returns the body elements that make up a single mod
  getBody(mod: BackendModConfig, config: ModState, onchange: (enabled: boolean, identifier: string) => void): React.ReactNode {

    /**
     * when a mod contains a single change render it as a single element
     * and use detailed description as the text to show
     * when the only change is a checkbox do not render a nested checkbox
     */
    if (mod.changes.length === 1) {
      const change: BackendChangeConfig = mod.changes[0];
      const changeValue: string | number | undefined = config[change.identifier].value;
      const isEnabled: boolean = config[change.identifier].enabled;
      const elementKey: string = 'ucp-change-' + change.identifier;
      return (
        <React.Fragment key={elementKey}>
          {change.selectionType === 'RADIO' && <Radio mod={mod} change={change} selected={changeValue as string} onchange={onchange}></Radio>}
          {change.selectionType === 'SLIDER' && <Slider mod={mod} change={change} selectedValue={changeValue as number} isEnabled={isEnabled} onchange={onchange}></Slider>}
          {change.selectionType === 'CHECKBOX' && change.description}
          {change.detailedDescription !== undefined && change.detailedDescription}
        </React.Fragment>
      );
    } else {
      return this.getChangeElements(mod, config, onchange);
    }
  }

  // with more than 1 change, each will have their own toggle checkbox and 
  // for non-checkbox changes nested selection options as well
  getChangeElements(mod: BackendModConfig, changeConfig: ModState, onchange: (enabled: boolean, identifier: string) => void): React.ReactNode {
    return mod.changes.map((change: BackendChangeConfig) => {
      const identifier: string = change.identifier;
      const elementKey: string = 'ucp-change-' + identifier;
      return <React.Fragment key={elementKey}>
        <ChangeProvider mod={mod} change={change} isEnabled={changeConfig[identifier].enabled} selectedValue={changeConfig[identifier].value} onchange={onchange}/>
        {change.detailedDescription !== undefined && change.detailedDescription}
      </React.Fragment>;
    });
  }
}

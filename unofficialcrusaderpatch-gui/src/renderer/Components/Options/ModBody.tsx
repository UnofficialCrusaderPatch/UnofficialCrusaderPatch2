import * as React from 'react';
import { BackendChangeConfig, BackendModConfig } from '../App';
import { ChangeConfig, ModConfig } from '../reducers';
import { ChangeProvider } from './ChangeElements/ChangeProvider';
import { Radio } from './ChangeElements/Radio';
import { Slider } from './ChangeElements/Slider';

/**
 * Body element for a single mod element
 */
export class ModBody extends React.Component<{mod: any, modIndex: number, config: ModConfig, onchange: ( enabled: boolean, identifier: string) => void}> {
  componentDidMount() {}

  // render the mod as the set of its comprising changes followed by a comprehensive detailed description
  render() {
    const modType: string = this.props.mod.modType;
    const modIndex: number = this.props.modIndex;
    const modIdentifier: string = this.props.mod.modIdentifier;
    const detailedDescription: string = this.props.mod.detailedDescription;

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
  getBody(mod: BackendModConfig, config: ModConfig, onchange: (enabled: boolean, identifier: string) => void): React.ReactNode {

    /**
     * when a mod contains a single change render it as a single element
     * and use detailed description as the text to show
     * when the only change is a checkbox do not render a nested checkbox
     */
    if (mod.changes.length === 1) {
      const changeConfig: ChangeConfig = config[mod.modIdentifier][0];
      const change: BackendChangeConfig = mod.changes[0];
      const elementKey: string = 'ucp-change-' + change.identifier;
      return (
        <React.Fragment key={elementKey}>
          {change.selectionType === 'RADIO' && <Radio mod={mod} change={change} selected={changeConfig.value as string} onchange={onchange}></Radio>}
          {change.selectionType === 'SLIDER' && <Slider mod={mod} change={change} selectedValue={changeConfig.value as number} onchange={onchange}></Slider>}
          {change.selectionType === 'CHECKBOX' && change.description}
          {change.detailedDescription !== undefined && change.detailedDescription}
        </React.Fragment>
      );
    } else {
      return this.getChangeElements(mod, config[mod.modIdentifier], onchange);
    }
  }

  // with more than 1 change, each will have their own toggle checkbox and 
  // for non-checkbox changes nested selection options as well
  getChangeElements(mod: BackendModConfig, changeConfig: ChangeConfig[], onchange: (enabled: boolean, identifier: string) => void): React.ReactNode {
    return mod.changes.map((change: BackendChangeConfig) => {
      const elementKey: string = 'ucp-change-' + change.identifier;
      return <React.Fragment key={elementKey}>
        <ChangeProvider mod={mod} change={change} selectedValue={changeConfig.filter(x => x.identifier === change.identifier)[0].value} onchange={onchange}/>
        {change.detailedDescription !== undefined && change.detailedDescription}
      </React.Fragment>;
    });
  }
}

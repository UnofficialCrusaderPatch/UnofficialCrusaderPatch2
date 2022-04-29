import * as React from 'react';
import { ChangeProvider } from './ChangeElements/ChangeProvider';
import { Radio } from './ChangeElements/Radio';
import { Slider } from './ChangeElements/Slider';

/**
 * Body element for a single mod element
 */
export class ModBody extends React.Component<{mod: any, modIndex: number, onchange: (config: { enabled: boolean, identifier: string}[]) => void}> {
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
      const elementKey: string = 'ucp-change-' + change.identifier;
      return (
        <React.Fragment key={elementKey}>
          {change.selectionType === 'RADIO' && <Radio mod={mod} change={change}></Radio>}
          {change.selectionType === 'SLIDER' && <Slider mod={mod} change={change}></Slider>}
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
      const elementKey: string = 'ucp-change-' + change.identifier;
      return <React.Fragment key={elementKey}>
        <ChangeProvider mod={mod} change={change}/>
        {change.detailedDescription !== undefined && change.detailedDescription}
      </React.Fragment>;
    });
  }
}

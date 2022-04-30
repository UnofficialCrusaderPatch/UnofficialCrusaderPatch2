import * as React from 'react';
import { BackendModConfig } from '../App';
import { ChangeConfig, ModConfig } from '../reducers';
import { ModBody } from './ModBody';
import { ModHeader } from './ModHeader';

/**
 * Container for a single mod element with a header and a body
 */
export class ModLayout extends React.Component<{mod: BackendModConfig, modIndex: number}, ModConfig > {
  constructor(props: {mod: BackendModConfig, modIndex: number}){
    super(props);
    this.state = this.getInitialState(props.mod);
  }

  componentDidMount() {}

  render() {
    const modType: string = this.props.mod.modType;
    const modIndex: number = this.props.modIndex;
    const modIdentifier: string = this.props.mod.modIdentifier;
    const elementKey: string = 'mod-root' + modType + '-' + modIdentifier + '-' + modIndex;
    return (
      <React.Fragment key={elementKey}>
        <ModHeader mod={this.props.mod} modIndex={modIndex} config={this.state} onchange={this.toggleMod.bind(this)}></ModHeader>
        <ModBody mod={this.props.mod} modIndex={modIndex} config={this.state} onchange={this.toggleChange.bind(this)}></ModBody>
      </React.Fragment>
    );
  }


  // toggle change enabled/disabled status when enabling/disabling the entire mod
  toggleMod = (isEnabled: boolean, identifier: string) => {
    const currentState: ModConfig = JSON.parse(JSON.stringify(this.state));
    const changes: ChangeConfig[] = currentState[this.props.mod.modIdentifier];
    const newChanges = changes.map(function(change: ChangeConfig) {
      return {
        ...change,
        enabled: isEnabled
      }
    });
    this.setState({ [identifier]: newChanges});
  };


  // toggle change enabled/disabled status
  toggleChange = (isEnabled: boolean, identifier: string, value?: string | number) => {
    const currentState: ModConfig = JSON.parse(JSON.stringify(this.state));
    const changes: ChangeConfig[] = currentState[this.props.mod.modIdentifier];
    const newChanges = changes.map(function(change: ChangeConfig) {
      if (change.identifier === identifier) {
        if (change.value === undefined) {
          return {
            ...change,
            enabled: isEnabled,
          }
        } else {
          return {
            ...change,
            enabled: isEnabled,
            value: value
          }
        }
      } else {
        return {
          ...change
        }
      }
    })
    console.log(isEnabled);
    console.log(identifier);
    console.log(newChanges);
    this.setState({ [identifier]: newChanges});
  };

  // handleChange = async (enabled: boolean) => {

  // };

  getModValues = (config: { enabled: boolean, identifier: string}[]) => {
    if (config) {
      console.log(config);
    }
  };

  getInitialState(mod: BackendModConfig): Readonly<ModConfig> {
    const changeConfigState: ChangeConfig[] = [];
    mod.changes.forEach(function(change){
      if (change.defaultValue === "true") {
        if (change.selectionType === "CHECKBOX") {
          changeConfigState.push({identifier: change.identifier, enabled: true});
        } else if (change.selectionType === "SLIDER") {
          changeConfigState.push({
            identifier: change.identifier,
            enabled: true,
            value: change.selectionParameters?.default
          });
        }
      } else if (change.selectionType === "RADIO") {
          changeConfigState.push({
            identifier: change.identifier,
            enabled: true,
            value: change.defaultValue
          });
      }
    });

    return {
      [mod.modIdentifier]: changeConfigState
    };
  };
}

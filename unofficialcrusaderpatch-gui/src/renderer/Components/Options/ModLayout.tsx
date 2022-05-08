import * as React from 'react';
import { BackendChangeConfig, BackendModConfig } from '../App';
import { ModState } from '../reducers';
import { ModBody } from './ModBody';
import { ModHeader } from './ModHeader';

/**
 * Container for a single mod element with a header and a body
 */
export class ModLayout extends React.Component<{mod: BackendModConfig, modIndex: number}, ModState > {
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
  toggleMod = (isEnabled: boolean) => {
    const currentState: ModState = JSON.parse(JSON.stringify(this.state));

    if (!isEnabled) {
      const newState: ModState = {};
      Object.keys(currentState).forEach(function(identifier: string) {
        newState[identifier] = currentState[identifier];
        newState[identifier].enabled = false;
        newState[identifier].value = currentState[identifier].value;
      });
      this.setState(newState);
      return;
    } else {
      const newChanges: ModState = {};
      this.props.mod.changes.forEach(function(change: BackendChangeConfig) {
        const defaultValue = change.defaultValue;
        const currentValue: string | number | undefined = currentState[change.identifier].value;

        if (defaultValue !== "false") {
          switch (change.selectionType) {

            // always re-enable if default is true
            case 'CHECKBOX':
              newChanges[change.identifier] = { enabled: true};
              break;

            // always re-enable if default is true
            // if the value has been changed from default selection use that, otherwise set to default
            case 'RADIO':
              if (currentValue !== defaultValue) {
                newChanges[change.identifier] = { enabled: true, value: currentValue};
              } else {
                newChanges[change.identifier] = { enabled: true, value: defaultValue};
              }
              break;

            // enable it if default is true
            // if user has set a value use that
            // if the current value is the default (vanilla) then disable
            // if the current value is also suggested value enable and use that
            case 'SLIDER':
              if (currentValue !== change.selectionParameters.suggested || currentValue !== change.selectionParameters.default) {
                newChanges[change.identifier] = { enabled: true, value: currentValue};
              } else if (currentValue === change.selectionParameters.default) {
                newChanges[change.identifier] = { enabled: false, value: currentValue};
              } else {
                newChanges[change.identifier] = { enabled: true, value: defaultValue};
              }
              break;
          }
        }
      });
      this.setState(newChanges);
      return;
    }
  };


  // toggle change enabled/disabled status
  toggleChange = (isEnabled: boolean, identifier: string, value?: string | number) => {
    const currentState: ModState = JSON.parse(JSON.stringify(this.state));
    const newChanges: ModState = {};
    Object.keys(currentState).forEach(function(currentIdentifier: string) {
      if (currentIdentifier === identifier) {
        if (currentState[currentIdentifier].value === undefined) {
          newChanges[currentIdentifier] =
          {
            ...currentState[currentIdentifier],
            enabled: isEnabled,
            value: value ?? currentState[currentIdentifier].value
          }
        } else {
          newChanges[currentIdentifier] = {
            ...currentState[currentIdentifier],
            enabled: isEnabled,
            value: value ?? currentState[currentIdentifier].value
          }
        }
      } else {
        newChanges[currentIdentifier] = {
          ...currentState[currentIdentifier]
        }
      }
    })
    this.setState(newChanges);
  };


  getInitialState(mod: BackendModConfig): ModState {
    const changeConfigState: ModState = {};
    mod.changes.forEach(function(change){
      if (change.defaultValue === "true") {
        if (change.selectionType === "CHECKBOX") {
          changeConfigState[change.identifier] = { enabled: true};
        } else if (change.selectionType === "SLIDER") {
          changeConfigState[change.identifier] =
          {
            enabled: true,
            value: change.selectionParameters?.default
          };
        }
      } else if (change.selectionType === "RADIO") {
          changeConfigState[change.identifier] =
          {
            enabled: true,
            value: change.defaultValue
          };
      }
    });
    return changeConfigState;
  };
}

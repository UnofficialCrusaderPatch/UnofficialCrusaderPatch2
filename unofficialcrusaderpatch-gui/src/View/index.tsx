import * as React from 'react';
import { Header } from '../Components/Header';

export class View extends React.Component<{}> {
  componentDidMount() {}

  render() {
    return (
      <React.Fragment>
        <Header></Header>
        {this.props.children}
      </React.Fragment>
    )
  }
}

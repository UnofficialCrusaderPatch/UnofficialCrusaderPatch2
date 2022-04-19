import * as React from 'react';
import { Header } from '../Components/Header';

/*
 * Render the content below the app header
 */
export class View extends React.Component<{}> {
  componentDidMount() {}

  render() {
    return (
      <React.Fragment>
        <Header/>
        {this.props.children}
      </React.Fragment>
    )
  }
}

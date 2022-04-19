import * as React from 'react';

/**
 * Renders a Tab header corresponding to a single mod type
 */ 
export class TabHeader extends React.Component<{ modType: string }> {
  componentDidMount() {}

  render() {
    return (
      <React.Fragment>
        <li
          className='nav-item'
          key={'ucp-tab-' + this.props.modType + '-list-item'}
        >
          <a
            className='nav-link'
            id={'ucp-tab-' + this.props.modType}
            key={'ucp-tab-' + this.props.modType}
            href={'#' + this.props.modType}
            role='tab'
            data-bs-toggle='tab'
            data-bs-target={'#' + this.props.modType}
          >
            {this.props.modType}
          </a>
        </li>
      </React.Fragment>
    );
  }
}

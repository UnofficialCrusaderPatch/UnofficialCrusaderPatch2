import * as React from 'react';

/**
 * Renders a Tab header corresponding to a single mod type
 */ 
export class TabHeader extends React.Component<{ modType: string }> {
  componentDidMount() {}

  render() {
    const elementUniqueId: string = 'ucp-tab-' + this.props.modType;
    const listElementKey: string = elementUniqueId + '-list-item';
    return (
      <React.Fragment>
        <li
          className='nav-item'
          key={listElementKey}
        >
          <a
            className='nav-link'
            id={elementUniqueId}
            key={elementUniqueId}
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

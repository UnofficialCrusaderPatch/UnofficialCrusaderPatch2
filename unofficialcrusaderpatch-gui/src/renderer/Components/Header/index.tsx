import * as React from 'react';

export class Header extends React.Component {

  render() {
    return (
      <React.Fragment>
        <h2 id="ucp-title">Unofficial Crusader Patch</h2>
        {
          false &&
          <div>
            {this.getGithubElement()}
            {this.getDiscordElement()}
          </div>
        }
      </React.Fragment>
    );
  }

  getGithubElement() {
    return (
      <React.Fragment>
        <a href="https://github.com/Sh0wdown/UnofficialCrusaderPatch"><i className="fab fa-github"></i></a>
      </React.Fragment>
    );
  }

  getDiscordElement() {
    return (
      <React.Fragment>
        <a href="https://discord.gg/vmy7CBR"><object data="https://discordapp.com/api/guilds/426318193603117057/widget.png?style=shield" type="image/png"/></a>
      </React.Fragment>
    );
  }
}

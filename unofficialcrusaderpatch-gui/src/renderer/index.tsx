/**
 * This file will automatically be loaded by webpack and run in the "renderer" context.
 * To learn more about the differences between the "main" and the "renderer" context in
 * Electron, visit:
 *
 * https://electronjs.org/docs/tutorial/application-architecture#main-and-renderer-processes
 *
 * By default, Node.js integration in this file is disabled. When enabling Node.js integration
 * in a renderer process, please be aware of potential security implications. You can read
 * more about security risks here:
 *
 * https://electronjs.org/docs/tutorial/security
 *
 * To enable Node.js integration in this file, open up `main.js` and enable the `nodeIntegration`
 * flag:
 *
 * ```
 *  // Create the browser window.
 *  mainWindow = new BrowserWindow({
 *    width: 800,
 *    height: 600,
 *    webPreferences: {
 *      nodeIntegration: true
 *    }
 *  });
 * ```
 */
import 'bootstrap';
import './app.css';
import './scss/app.scss';
import { AppLayout } from './App';
import * as React from 'react';
import * as ReactDOM from 'react-dom';

const initialize = async () => {
  return window.electron.getConfig('English');
};


const getWindow = (result: object[]): React.ReactElement => {
  console.log(result);
  return React.createElement(AppLayout, { config: result });
};

// render the AppLayout UI element within the <div> tag with id=ucp
initialize().then((result) => ReactDOM.render(getWindow(result), document.getElementById("ucp")));

# Unofficial Crusader Patch GUI 101

## Project Toolkit

The UnofficialCrusaderPatchGUI (_unofficialcrusaderpatch-gui-1.0.0-alpha_) is an NodeJS project.

- Built upon the Electron React Boilerplate tempalte
- Uses Electron framework to render within a desktop application context
- Uses EdgeJS to interact with C# .NET DLL (the UnofficialCrusaderPatch backend)
- Uses React.js, TypeScript, Webpack
- Uses Bootstrap for additional element styling

### Setting up your development environment

- Install NodeJS (which includes the npm package manager) and ensure it is added to path.
  - Running the command `node -v` should print out your installed NodeJS version.
  - Running the command `npm -v` should print out your installed npm version.
- Clone the repository from github to your local workspace
- Build the `UnofficialCrusaderPatch` project first! Copy the DLL to the root directory of this project (in the same folder as this README)
- Open a terminal (command prompt) window in your workspace
- Run the command `npm ci` to install all project dependencies with the versions specified in the `package-lock.json` file. This is essential for a reproducible build.
- Run the command `npm run start` to start the application. You will see many lines of logging appear in your command prompt window following which the application window will eventually open.
  - This project uses Webpack which allows most changes to automatically take effect without recompiling. If you see a blank window or message `Devtools disconnected` after making a change then using the `View` menu inside the application click `Reload`.

## File Structure

An electron project consists of two key components - the `main` process and the `renderer` process.

- The `main` process:

  - This is all the code in the `src/main` directory. This code has access to Node variables and commands like `require()` and is the `server-side` portion of the application.
  - The `src/main/preload.ts` file exposes a very specific `contextBridge` object which is used to expose an API through the `renderer` process can communicate with the server-side code and make use of Node functionality.

- The `renderer` process:
  - This is all the code in the `src/renderer` directory. This code has no access to Node variables and its only means of accessing such functionality is through the aforementioned `contextBridge`.
  - The rendering code is written using React.js and TypeScript and ultimately renders HTML.

## Creating the GUI

Within this project the view is built using a combination of React elements, HTML, and client-side logic. Just like in regular HTML there is a single root element for the view that contains all child elements and can form a nested hierarchy.

- The project config uses the `src/rendererindex.ejs` HTML template as the initial page document
- The `AppLayout` renders inside the `div` (with `id=ucp`) as the root element for the GUI elements
- The `AppLayout` is the top-level container element. Inside the `AppLayout` there is a `Header` element containing the UCP title elements and underneath a `TabLayout` containing core UCP options display.
- The `TabLayout` consists of a set of `TabHeader` and `TabContent` elements (one per mod type, where 'mod type' describes different overarching themes of mods such as Bugfixes, AIV, AIC, etc)
- The `TabContent` elements is the pane wherein all the different options (mods) are displayed. The hierarchy is `TabContent` => `ModLayout` (individual mods) => 'ChangeElements' (to be discussed further)
- Each `ModLayout` consists of a `ModHeader` and a `ModBody`
  - `ModHeader` represents the single checkbox and description that is used to enable/disable a single Mod
  - `ModBody` consists of:
    - Mod descriptions
    - Individual change elements (`Radio`, `Checkbox`, `Slider`)
    - Each change and change element has their own configuration (default value, selection parameters, etc.) that gets applied at this level

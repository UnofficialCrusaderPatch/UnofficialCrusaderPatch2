/**
 * Declaration file for electron context bridge
 */

declare global {
  interface Window {
    electron: {
      getConfig: (language: string) => any;
    };
  }
}

export {}
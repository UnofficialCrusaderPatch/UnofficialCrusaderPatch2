declare global {
  interface Window {
    electron: {
      getConfig: (language: string) => any;
    };
  }
}

export {}
/* eslint-disable @typescript-eslint/no-var-requires */
const { contextBridge } = require('electron');
const path = require('path');
const edge = require('electron-edge-js');

/**
 * Retrieve UCP config from the UnofficialCrusaderPatch.dll
 * @param language language to use with UCP
 */
function getUCPConfig(language: string) {
  let ucpPath: string;
  if (process.env.PORTABLE_EXECUTABLE_DIR) {
    ucpPath = path.join(process.env.PORTABLE_EXECUTABLE_DIR, 'UnofficialCrusaderPatch.dll');
  } else {
    ucpPath = path.join(path.resolve('.'), 'UnofficialCrusaderPatch.dll');
  }
  
  // declare edge function for calling .NET dll
  const clrMethod = edge.func({
    assemblyFile: ucpPath,
    typeName: 'UCP.API.UCPContract',
    methodName: 'Invoke', // This must be Func<object,Task<object>>
  });

  // .NET dll calls should be asynchronous to avoid blocking the rendering thread (UI)
  return new Promise(function (resolve, reject) {
    clrMethod(language, function (error: any, result: any) {
      if (error) reject(error);
      resolve(result);
    });
  });
}

// this method forms the contract for the API for methods that can be called from renderer process
contextBridge.exposeInMainWorld('electron', {
  getConfig: async (language: string) => getUCPConfig(language),
});

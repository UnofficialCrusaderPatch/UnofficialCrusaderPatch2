const { contextBridge } = require('electron');

var edge = require('electron-edge-js');
var clrMethod = edge.func({
  assemblyFile: 'UnofficialCrusaderPatch.dll',
  typeName: 'UCP.API.Startup',
  methodName: 'Invoke', // This must be Func<object,Task<object>>
});

function getUCPConfig(language: string) {
  return new Promise(function (resolve, reject) {
    clrMethod(language, function (error: any, result: any) {
      if (error) reject(error);
      resolve(result);
    });
  });
}

contextBridge.exposeInMainWorld('electron', {
  getConfig: async (language: string) => getUCPConfig(language),
});

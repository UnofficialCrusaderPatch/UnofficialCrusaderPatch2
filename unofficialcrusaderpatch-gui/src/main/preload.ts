const { contextBridge } = require('electron');
const path = require('path');
var edge = require('electron-edge-js');

function getUCPConfig(language: string) {
  let execPath = path.dirname(process.execPath);
  console.log(process.env.PORTABLE_EXECUTABLE_DIR);
  console.log(path.resolve('.'));
  console.log(execPath);

  let ucpPath: string;
  if (process.env.PORTABLE_EXECUTABLE_DIR) {
    ucpPath = path.join(process.env.PORTABLE_EXECUTABLE_DIR, 'UnofficialCrusaderPatch.dll');
  } else {
    ucpPath = path.join(path.resolve('.'), 'UnofficialCrusaderPatch.dll');
  }
  
  var clrMethod = edge.func({
    assemblyFile: ucpPath,
    typeName: 'UCP.API.Startup',
    methodName: 'Invoke', // This must be Func<object,Task<object>>
  });

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

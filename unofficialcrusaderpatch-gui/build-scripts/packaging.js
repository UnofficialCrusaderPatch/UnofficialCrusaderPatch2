module.exports = function(extractPath, electronVersion, platform, arch, done) {
    const fs = require('fs');
    const path = require('path');
    const dllPath = 'UnofficialCrusaderPatch.dll';
    const destPath = path.join(extractPath, dllPath);
    fs.copyFile(dllPath, destPath, (err) => {
        if (err) throw err;
        console.log('Copied UnofficialCrusaderPatch.dll');
    });
    done();
}
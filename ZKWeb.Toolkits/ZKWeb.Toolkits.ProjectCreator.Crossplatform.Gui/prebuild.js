var fs = require("fs-extra");
const util = require('util');
const child_process = require('child_process');

var commnad = 'dotnet build .\\DatabaseUtils\\DatabaseUtils.csproj ';
var from ='.\\DatabaseUtils\\bin\\Release\\netcoreapp2.0';
var to='.\\src\\assets';

child_process.exec(commnad,
    (error, stdout) => {
        if (!error) {
            console.log('build success');
            fs.copy(from, to, function (err) {
                if (err) return console.error(err);
                console.log('complete!');
            });
        } else {
            console.error(error);
        }
    });
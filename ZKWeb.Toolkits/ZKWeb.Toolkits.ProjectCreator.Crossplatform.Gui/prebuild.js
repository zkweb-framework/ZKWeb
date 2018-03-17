var fs = require("fs-extra");
var path=require("path");
const util = require("util");
const child_process = require("child_process");

var commnad = "dotnet build -c Release "+path.join(".","DatabaseUtils","DatabaseUtils.csproj");

var from =path.join(".","DatabaseUtils","bin","Release","netcoreapp2.0");
var to=path.join(".","src","assets");


child_process.exec(commnad,
    (error, stdout) => {
        if (!error) {
            console.log("build success");
            fs.copy(from, to, function (err) {
                if (err) return console.error(err);
                console.log("complete!");
            });
        } else {
            console.log(error);
        }
    });

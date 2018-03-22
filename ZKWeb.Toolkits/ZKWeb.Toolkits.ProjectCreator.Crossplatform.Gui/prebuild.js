const fs = require("fs-extra");
const path = require("path");
const childProcess = require("child_process");
const util = require("util");
const debuglog = util.debuglog("prebuild");

var commnad = "dotnet publish -c Release " + path.join(".", "DatabaseUtils", "DatabaseUtils.csproj");
var from = path.join(".", "DatabaseUtils", "bin", "Release", "netcoreapp2.0","publish");
var to = path.join(".", "src", "assets","dbUtil");

childProcess.exec(commnad,
  (error, stdout) => {
    if (!error) {
      debuglog("build success");
      fs.copy(from, to, function (err) {
        if (err) {
          return debuglog(err);
        } else {
          debuglog("complete!");
        }
      });
    } else {
      debuglog(error);
    }
  });


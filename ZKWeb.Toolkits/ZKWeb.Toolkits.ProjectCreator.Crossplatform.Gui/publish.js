/*import { request } from "http";

"use strict";
var packager = require("electron-packager");
var path=require("path");

var options = {
    "dir": "./",
    "app-copyright": "魂祭心",
    "app-version": "1.0.0",
    "asar": false,
    "icon": "./src/assets/icon.ico",
    "name": "Zkweb Project Creator",
    "out": "./build",
    "overwrite": true,
    "prune": false,
    "version": "1.0.0",
    "ignore":function(str){
    //--ignore=\"(src|config|.gitignore|build|.cs|.pdb|node_modules|prebuild.js|readme.md|tsconfig.json|tslint.json|package-lock.json
       var p= path.parse('/home/user/dir/file.txt');
       ignorePath(p,"src");
       ignorePath(p,"src");
    }
};

function keep(path){
  
}

function ignorePath(path,word){
   return path.indexOf(key)!==-1;
}

function ignoreFile(path,){

}

function ignoreExtend(path,ext){

}

packager(options, function done_callback(err, appPaths) {
   
});*/
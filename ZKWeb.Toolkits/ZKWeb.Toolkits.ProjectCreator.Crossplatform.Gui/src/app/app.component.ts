import { Component } from '@angular/core';
import { CreateProjectParameters } from './CreateProjectParameters';
import '../assets/sass/style.scss';
import { Input } from '@angular/core';
import { remote } from 'electron';
import { TranslateService } from "@ngx-translate/core";
import { baseConnection } from './dataBaseConnection/baseConnection';
import { mySqlConnection } from './dataBaseConnection/mysqlConnection';
import { sqlLiteConnection } from './dataBaseConnection/sqlLiteConnection';
import { msSqlConnection } from './dataBaseConnection/msSqlConnection';
import { postgreSQLConnection } from './dataBaseConnection/postgreSQLConnection';
import { mongoConnection } from './dataBaseConnection/mongoConnection';
const child_process = require('child_process');
const app = require('electron').remote.app;
const EventEmitter = require('events');
const path=require('path');  
class MessageEmitter extends EventEmitter { }

@Component({
    selector: 'app',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})
export class AppComponent {
  

    @Input()
    public parameters: CreateProjectParameters;
    @Input()
    public enableDatabase:any;
    private language: string;
    private eventEmitter: MessageEmitter;
    private rootPath: string;
    constructor(public translateService: TranslateService) {
        this.language = app.getLocale();
        this.rootPath=app.getAppPath();
        this.parameters = new CreateProjectParameters();
        this.parameters.ProjectType = 'AspNetCore';
        this.parameters.ORM = 'NHibernate';
        this.parameters.Database = "MSSQL";
        this.eventEmitter = new MessageEmitter();
        this.enableDatabase={
            MSSQL:true, 
            MySQL:true, 
            SQLite:true, 
            PostgreSQL:true, 
            InMemory:true, 
            MongoDB:true,
        };
    }

    ngOnInit() {
        this.translateService.addLangs(["zh-CN", "en-US"]);
        this.translateService.setDefaultLang("zh-CN");
        this.translateService.use(this.language);
         
        this.eventEmitter.on("error", (msg: string,) => {
            remote.dialog.showErrorBox("error", msg)
        });
        this.eventEmitter.on("info", (msg: string) => {
            remote.dialog.showMessageBox({
                type:"info",
                title:"info",
                message :msg
            });
        });
    }
    public createProject(): void {
        var toolPath=this.findTools();
        if(!toolPath) {
            this.translateService.get("ToolsFoderFail", {}).subscribe((res: string) => {
                remote.dialog.showErrorBox("error", res)
            });
            return ;
        }
        var result = this.parameters.Check();
        if (result) {
            this.translateService.get(result, {}).subscribe((res: string) => {
                console.log(res);
                remote.dialog.showErrorBox("error", res)
            });
            return ;
        } else {
            var parameStr = [
                "--t=" + this.parameters.ProjectType,
                "--n=" + this.parameters.ProjectName,
                "--m=" + this.parameters.ORM,
                "--b=" + this.parameters.Database,
                "--c=" + "\"this.parameters.ConnectionString" + "\"",
                "--o=" + this.parameters.OutputDirectory
            ].join(' ');
            if (this.parameters.ProjectDescription) {
                parameStr += " " + "--d=" + this.parameters.ProjectDescription;
            }
            if (this.parameters.ProjectDescription) {
                parameStr += " " + "--u=" + this.parameters.UseDefaultPlugins;
            }

            var commnad = 'dotnet '+toolPath+'/Template/ProjectCreator.Cmd.NetCore/ZKWeb.Toolkits.ProjectCreator.Cmd.dll ' + parameStr;
            console.log(commnad);
            child_process.exec(commnad,
                (error: any, stdout: any) => {
                    if (error) {
                        this.eventEmitter.emit("error", "fail");
                    }else{
                        this.eventEmitter.emit("info", stdout);
                    }
                });

        }
    }

    public  findTools():string {
        var folders=this.rootPath.split('\\');
        var index=folders.indexOf('Tools');
        if(index!=-1){
          return  folders.slice(0,index).join('\\');
        }
        return "";
    }
    public testConnection(): void {
        var connection: baseConnection;
        switch (this.parameters.Database) {
            case "MSSQL":
                connection = new msSqlConnection(this.translateService, this.parameters.ConnectionString);
                break;
            case "SQLite":
                connection = new sqlLiteConnection(this.translateService, this.parameters.ConnectionString);
                break;
            case "MySQL":
                connection = new mySqlConnection(this.translateService, this.parameters.ConnectionString);
                break;
            case "PostgreSQL":
                connection = new postgreSQLConnection(this.translateService, this.parameters.ConnectionString);
                break;
            case "MongoDB":
                connection = new mongoConnection(this.translateService, this.parameters.ConnectionString);
                break;
            case "InMemory":
                return;
            default:
                remote.dialog.showErrorBox("error", "database not support")
                return;
        }
        if (connection) {
            try {
                connection.parser();
            } catch{
                this.translateService.get('notValidConnectionString', {}).subscribe((res: string) => {
                    remote.dialog.showErrorBox("error", res)
                });
            }
            connection.testConnect(this.eventEmitter);
        }
    }


    public changeOrm(orm: string):void {
        var invalidDatabases = this.parameters.AvailableDatabases[orm];
        if (-1 == invalidDatabases.indexOf(this.parameters.Database)) {
            this.parameters.Database="";
        }
        var keys:Array<string>=[];
        for(var m in this.enableDatabase){
            keys.push(m)
        }

        keys.forEach((item)=>{
            this.enableDatabase[item]=false;
        });
        
        invalidDatabases.forEach((item:any)=>{
            this.enableDatabase[item]=true;
        });

        console.log(this.enableDatabase)
    }

    public pluginSelect(): void {
        var selectFile = remote.dialog.showOpenDialog({ properties: ['openFile'] });
        if (selectFile && selectFile.length > 0) {
            this.parameters.UseDefaultPlugins = selectFile[0];
        }
    }

    public outPutSelect(): void {
        var selectFile = remote.dialog.showOpenDialog({ properties: ['openDirectory'] });
        if (selectFile && selectFile.length > 0) {
            this.parameters.OutputDirectory = selectFile[0];
        }
    }

}

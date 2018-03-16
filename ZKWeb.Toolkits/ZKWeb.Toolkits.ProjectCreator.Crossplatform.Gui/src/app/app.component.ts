import { Component } from '@angular/core';
import { CreateProjectParameters } from './CreateProjectParameters';
import '../assets/sass/style.scss';
import { Input } from '@angular/core';
import { remote } from 'electron';
import { TranslateService } from "@ngx-translate/core";
const child_process = require('child_process');
const app = require('electron').remote.app;
const EventEmitter = require('events');
const path = require('path');

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
    public enableDatabase: any;
    
    private language: string;
    private eventEmitter: MessageEmitter;
    private rootPath: string;
    private isDataBaseChecking: boolean;

    constructor(public translateService: TranslateService) {
        this.language = app.getLocale();
        this.rootPath = app.getAppPath();
        this.parameters = new CreateProjectParameters();
        this.parameters.ProjectType = 'AspNetCore';
        this.parameters.ORM = 'NHibernate';
        this.parameters.Database = "MSSQL";
        this.isDataBaseChecking = false;
        this.eventEmitter = new MessageEmitter();
        this.enableDatabase = {
            MSSQL: true,
            MySQL: true,
            SQLite: true,
            PostgreSQL: true,
            InMemory: true,
            MongoDB: true,
        };
    }

    ngOnInit() {
        this.translateService.addLangs(["zh-CN", "en-US"]);
        this.translateService.setDefaultLang("zh-CN");
        this.translateService.use(this.language);

        this.eventEmitter.on("error", (msg: string, ) => {
            remote.dialog.showErrorBox("error", msg)
        });
        this.eventEmitter.on("info", (msg: string) => {
            remote.dialog.showMessageBox({
                type: "info",
                title: "info",
                message: msg
            });
        });
    }

    public createProject(): void {
        var toolPath = this.findTools();
        if (!toolPath) {
            this.translateService.get("ToolsFoderFail", {}).subscribe((res: string) => {
                this.eventEmitter.emit("error", res);
            });
            return;
        }
        var result = this.parameters.Check();
        if (!result.isSuccess) {
            this.translateService.get(result.msgPrefix, {}).subscribe((res: string) => {
                console.log(res);
                this.eventEmitter.emit("error", res + (result.args ? result.args : ""));
            });
            return;
        }

        var commnad=this.createCommand(toolPath);
        console.log(commnad);

        child_process.exec(commnad,
            (error: any, stdout: any) => {
                if (error) {
                    this.eventEmitter.emit("error", "fail");
                } else {
                    this.eventEmitter.emit("info", stdout);
                }
            });

    }

    public testConnection(): void {
        if (!this.isDataBaseChecking) {
            this.isDataBaseChecking = true;
            try {
                var utilPath = path.join(this.rootPath, 'dist', 'assets', 'DatabaseUtils.dll');
                var commnad = 'dotnet  ' + utilPath + ' ' + this.parameters.Database + ' "' + this.parameters.ConnectionString + '"';
                child_process.exec(commnad,
                    (error: any, stdout: any, stderr: any) => {
                        if (error) {
                            console.error(error);
                            console.error(stderr);
                            //this.eventEmitter.emit("info", commnad);
                            this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
                                this.eventEmitter.emit("error", res);
                            });
                        } else {
                            console.log(stdout);
                            this.eventEmitter.emit("info", "success");
                        }
                        this.isDataBaseChecking = false;
                    });
            } catch{
                this.isDataBaseChecking = false;
            }
        }
    }

    public changeOrm(orm: string): void {
        var invalidDatabases = this.parameters.AvailableDatabases[orm];
        if (-1 == invalidDatabases.indexOf(this.parameters.Database)) {
            this.parameters.Database = "";
        }
        var keys: Array<string> = [];
        for (var m in this.enableDatabase) {
            keys.push(m)
        }

        keys.forEach((item) => {
            this.enableDatabase[item] = false;
        });

        invalidDatabases.forEach((item: any) => {
            this.enableDatabase[item] = true;
        });
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

    private createCommand(toolPath:string):string{
        var parametersStr = [
            "--t=" + this.parameters.ProjectType,
            "--n=" + this.parameters.ProjectName,
            "--m=" + this.parameters.ORM,
            "--b=" + this.parameters.Database,
            "--c=" + "\"this.parameters.ConnectionString" + "\"",
            "--o=" + this.parameters.OutputDirectory
        ].join(' ');
        if (this.parameters.ProjectDescription) {
            parametersStr += " " + "--d=" + this.parameters.ProjectDescription;
        }
        if (this.parameters.ProjectDescription) {
            parametersStr += " " + "--u=" + this.parameters.UseDefaultPlugins;
        }

        toolPath = path.join(toolPath, "ProjectCreator.Cmd.NetCore", "ZKWeb.Toolkits.ProjectCreator.Cmd.dll");
        return 'dotnet ' + toolPath + ' ' + parametersStr;

    }
    
    private findTools(): string {
        var folders = this.rootPath.split(path.sep);
        var index = folders.indexOf('Tools');
        if (index != -1) {
            return folders.slice(0, index + 1).join(path.sep);
        }
        return "";
    }

}

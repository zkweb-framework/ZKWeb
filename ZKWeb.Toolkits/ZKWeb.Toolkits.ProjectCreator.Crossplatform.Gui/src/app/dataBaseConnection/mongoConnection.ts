import { baseConnection } from './baseConnection';
import { TranslateService } from '@ngx-translate/core';
var MongoClient = require('mongodb').MongoClient;

export class mongoConnection implements baseConnection {
    ip: string;
    port: string;
    user: string;
    password: string;
    connectionString:string;
    constructor(public translateService: TranslateService,connectonString: string){
        this.connectionString = connectonString;
    }
    parser(): void {
    }
    testConnect(messageEvent: any): void {
        MongoClient.connect(this.connectionString, (err: any) => {
            if (err) {
                this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
                    messageEvent.emit("error", res)
                });
            }else{
                messageEvent.emit("info", "success");
            }
        });
    }

}
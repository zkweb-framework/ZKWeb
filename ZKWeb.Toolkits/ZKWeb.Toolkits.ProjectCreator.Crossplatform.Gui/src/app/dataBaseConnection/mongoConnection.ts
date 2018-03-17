import { TranslateService } from "@ngx-translate/core";
import { IBaseConnection } from "./baseConnection";
const MongoClient = require("mongodb").MongoClient;

export class MongoConnection implements IBaseConnection {

    public ip: string;
    public port: number;
    public user: string;
    public password: string;
    public connectionString: string;

    constructor(public translateService: TranslateService, connectonString: string) {
        this.connectionString = connectonString;
    }
    parser(): void {
    }
    testConnect(messageEvent: any): void {
        MongoClient.connect(this.connectionString, (err: any) => {
            if (err) {
                this.translateService.get("dataBaseTestFail", {}).subscribe((res: string) => {
                    messageEvent.emit("error", res);
                });
            } else {
                messageEvent.emit("info", "success");
            }
        });
    }

}

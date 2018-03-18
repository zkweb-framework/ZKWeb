import { TranslateService } from "@ngx-translate/core";
import { IBaseConnection } from "./baseConnection";
const fs = require("fs");

export class SqlLiteConnection implements IBaseConnection {

  public ip: string;
  public port: number;
  public user: string;
  public password: string;
  public connectionString: string;

  constructor(public translateService: TranslateService, connectonString: string) {
      this.connectionString = connectonString;
  }
  /**
   * 
   * @param connectonString Data Source=server;Initial Catalog=db;User ID=test;Password=test;
   */
  parser(): void {
    const phrases = this.connectionString.toLowerCase().split(";");
    const config = {};
    for (const phrase of phrases) {
      if (phrase) {
        const kv = phrase.split("=");
        config[kv[0].trim()] = kv[1].trim();
      }
    }
    this.ip = config["data source"];
  }

  testConnect(messageEvent: any): void {
    if (fs.existsSync(this.ip)) {
      this.translateService.get("dataBaseTestFail", {}).subscribe((res: string) => {
        messageEvent.emit("error", res);
      });
    } else {
      messageEvent.emit("info", "success");
    }
  }

}

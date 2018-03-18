import { TranslateService } from "@ngx-translate/core";
import { IBaseConnection } from "./baseConnection";
const sql = require("mssql"); 

export class MsSqlConnection implements IBaseConnection {

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
    this.port = config["port"] ? parseInt(config["port"], 10) : 1433;
    this.user = config["user id"];
    this.password = config["password"];
  }

  testConnect(messageEvent: any): void {
    try {
      sql.connect({
        user: this.user,
        password: this.password,
        server: this.ip,
        port: this.port.toString(),
      }, (err: any) => {
        if (err) {
          this.translateService.get("dataBaseTestFail", {}).subscribe((res: string) => {
            messageEvent.emit("error", res);
          });
        } else {
          messageEvent.emit("info", "success");
        }
        sql.close();
      });
    } catch {
      this.translateService.get("dataBaseTestFail", {}).subscribe((res: string) => {
        messageEvent.emit("error", res);
      });
      sql.close();
    }
  }

}

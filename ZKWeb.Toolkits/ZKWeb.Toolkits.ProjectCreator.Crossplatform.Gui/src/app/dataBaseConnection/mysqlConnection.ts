import { TranslateService } from "@ngx-translate/core";
import { IBaseConnection } from "./baseConnection";
const mysql = require("mysql");

export class MySqlConnection implements IBaseConnection {

  ip: string;
  port: number;
  public user: string;
  public password: string;
  connectionString: string;

  constructor(public translateService: TranslateService, connectonString: string) {
    this.connectionString = connectonString;
  }

  parser(): void {
    const phrases = this.connectionString.toLowerCase().split(";");
    const config = {};
    for (const phrase of phrases) {
      if (phrase) {
        const kv = phrase.split("=");
        config[kv[0].trim()] = kv[1].trim();
      }
    }
    this.ip = config["server"];
    this.port = config["port"] ? parseInt(config["port"], 10) : 3306;
    this.user = config["user"];
    this.password = config["password"];
  }

  testConnect(messageEvent: any): void {
    let connection: any = null;
    try {
      connection = mysql.createConnection({
        host: this.ip,
        port: this.port.toString(),
        user: this.user,
        password: this.password,
      });

      connection.connect({}, (err: any) => {
        if (err) {
          this.translateService.get("dataBaseTestFail", {}).subscribe((res: string) => {
            messageEvent.emit("error", res);
          });
        } else {
          messageEvent.emit("info", "success");
        }
        connection.destroy();
      });
    } catch {
      this.translateService.get("dataBaseTestFail", {}).subscribe((res: string) => {
        messageEvent.emit("error", res);
      });
      connection.destroy();
    }
  }

}

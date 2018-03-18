import { TranslateService } from "@ngx-translate/core";
import { IBaseConnection } from "./baseConnection";
const util = require("util");
const debuglog = util.debuglog("postgreSQL");
export class PostgreSQLConnection implements IBaseConnection {

  public user: string;
  public password: string;
  public connectionString: string;
  public ip: string;
  public port: number;
  public db: string;

  constructor(public translateService: TranslateService, connectonString: string) {
      this.connectionString = connectonString;
  }
  /**
   * 
   * @param connectonString PORT=5432;DATABASE=Demo;HOST=localhost;PASSWORD=root;USER ID=postgres
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
    this.ip = config["host"];
    this.port = config["port"] ? parseInt (config["port"], 10) : 5432;
    this.user = config["user id"];
    this.password = config["password"];
    this.db = config["database"];
  }

  testConnect(messageEvent: any): void {
    debuglog(messageEvent);
  }

}

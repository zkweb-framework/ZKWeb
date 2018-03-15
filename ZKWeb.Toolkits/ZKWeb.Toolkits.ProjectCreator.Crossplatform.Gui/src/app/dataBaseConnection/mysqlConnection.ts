import { baseConnection } from './baseConnection';
import { TranslateService } from '@ngx-translate/core';
var mysql = require('mysql');

export class mySqlConnection implements baseConnection {

  ip: string;
  port: number;
  public user: string;
  public password: string;
  connectionString: string;

  constructor(public translateService: TranslateService, connectonString: string) {
    this.connectionString = connectonString;
  }
  /**
   * 
   * @param connectonString Server=localhost;Database=drivetop_base; User=otnp80;Password=123;Use Procedure Bodies=false;Charset=utf8;Allow Zero Datetime=True; Pooling=false; Max Pool Size=50; 
   */
  parser(): void {
    var phrases = this.connectionString.toLowerCase().split(';');
    var config = {};
    for (var i = 0; i < phrases.length; i++) {
      if (phrases[i]) {
        var kv = phrases[i].split('=');
        config[kv[0].trim()] = kv[1].trim();
      }
    }
    this.ip = config["server"];
    this.port = config["port"]?parseInt(config["port"]):3306;
    this.user = config["user"];
    this.password = config["password"];
  }

  testConnect(messageEvent: any): void {
    try {
      var connection = mysql.createConnection({
        host: this.ip,
        port: this.port.toString(),
        user: this.user,
        password: this.password
      });

      connection.connect({},(err:any)=>{
        if (err) {
          this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
            messageEvent.emit("error", res);
          });
        }else{
          messageEvent.emit("info", "success");
        }
        connection.destroy();
      });
    } catch{
      this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
        messageEvent.emit("error", res);
      });
      connection.destroy();
    }
  }

}
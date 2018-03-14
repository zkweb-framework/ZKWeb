import { baseConnection } from './baseConnection';
import { TranslateService } from '@ngx-translate/core';
var pg = require('pg');

export class postgreSQLConnection implements baseConnection {

  ip: string;
  port: string;
  public user: string;
  public password: string;
  connectionString:string;
  
  constructor(public translateService: TranslateService,connectonString: string){
      this.connectionString = connectonString;
  }
  /**
   * 
   * @param connectonString Server=192.168.1.100;Port=5432;UserId=mike;Password=secret;Database=mikedb;
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
    this.port = config["port"];
    this.user = config["userid"];
    this.password = config["password"];
  }


  testConnect(messageEvent: any): void {
    try{
      pg.connect(this.connectionString, (err:any) =>{
        if(err) {
          this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
            messageEvent.emit("error", res)
          });
        }else{
          messageEvent.emit("info", "success");
        }
        pg.close();
      });
    }catch{
      this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
        messageEvent.emit("error", res)
      });
    }
  }

}
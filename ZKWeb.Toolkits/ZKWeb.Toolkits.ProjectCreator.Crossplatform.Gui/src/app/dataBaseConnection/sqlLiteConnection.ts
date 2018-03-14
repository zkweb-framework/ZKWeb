import { baseConnection } from './baseConnection';
import { TranslateService } from '@ngx-translate/core';
var fs = require('fs');

export class sqlLiteConnection implements baseConnection {

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
   * @param connectonString Data Source=server;Initial Catalog=db;User ID=test;Password=test;
   */
  parser(): void {
    var phrases = this.connectionString.toLowerCase().split(';');
    var config = {};
    for (var i = 0; i < phrases.length; i++) {
      if(phrases[i]){
        var kv = phrases[i].split('=');
        config[kv[0].trim()] = kv[1].trim();
      }
    }
    this.ip = config["data source"];
  }


  testConnect(messageEvent: any): void {
    if (fs.existsSync(this.ip)) {
      this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
        messageEvent.emit("error", res)
      });
    }else{
      messageEvent.emit("info", "success");
    }
  }

}
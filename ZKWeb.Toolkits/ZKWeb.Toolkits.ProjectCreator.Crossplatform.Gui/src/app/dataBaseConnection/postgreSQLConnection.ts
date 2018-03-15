import { baseConnection } from './baseConnection';
import { TranslateService } from '@ngx-translate/core';
var Client = require('pg2')

export class postgreSQLConnection implements baseConnection {


  public user: string;
  public password: string;
  public connectionString:string;
  public ip: string;
  public port: number;
  public db: string;
  constructor(public translateService: TranslateService,connectonString: string){
      this.connectionString = connectonString;
  }
  /**
   * 
   * @param connectonString PORT=5432;DATABASE=Demo;HOST=localhost;PASSWORD=root;USER ID=postgres
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
    this.ip = config["host"];
    this.port = config["port"]?parseInt(config["port"]):5432;
    this.user = config["user id"];
    this.password = config["password"];
    this.db = config["database"];
  }


  testConnect(messageEvent: any): void {
    try{
      var c = new Client({
        host:  this.ip,
        port:this.port,
        user: this.user,
        password:this.password,
        db: "postgres"
      });
      c.on("ready",(a:any)=>{

      })
      c.on("error",(a:any)=>{

      })
      c.connect((err:any)=> {
        if(err) {
          this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
            messageEvent.emit("error", res)
          });
        }else{
          messageEvent.emit("info", "success");
        }
        c.destroy();
      });
    }catch{
      this.translateService.get('dataBaseTestFail', {}).subscribe((res: string) => {
        messageEvent.emit("error", res)
      });
      c.destroy();
    }
  }

}

export interface baseConnection {

    ip: string;
    port: string;
    user: string;
    password: string;
    connectionString:string;
    parser(): void;

    testConnect(messageEvent: any): void;

}
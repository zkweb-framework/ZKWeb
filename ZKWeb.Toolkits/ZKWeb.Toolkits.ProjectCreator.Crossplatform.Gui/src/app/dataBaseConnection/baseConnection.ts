
export interface baseConnection {

    ip: string;
    port: number;
    user: string;
    password: string;
    connectionString:string;
    parser(): void;

    testConnect(messageEvent: any): void;

}
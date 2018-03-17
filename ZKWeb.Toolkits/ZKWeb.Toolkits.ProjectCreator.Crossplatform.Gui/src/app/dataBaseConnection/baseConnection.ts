
export interface IBaseConnection {

    ip: string;
    port: number;
    user: string;
    password: string;
    connectionString: string;
    parser(): void;

    testConnect(messageEvent: any): void;

}

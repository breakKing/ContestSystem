import { IRole } from "./role";

export interface ICurrentUser {
    id: number;
    username: string;
    roles: IRole[];
}
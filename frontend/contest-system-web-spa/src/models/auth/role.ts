export interface IRole {
    name: Roles.USER | Roles.MODERATOR | Roles.ADMIN;
};

export enum Roles {
    USER = "user",
    MODERATOR = "moderator",
    ADMIN = "admin"
};
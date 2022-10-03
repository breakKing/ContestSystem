import { RouteObject } from "react-router-dom";
import LoginPage from "../../pages/auth/LoginPage";
import SignUpPage from "../../pages/auth/SignUpPage";

export enum AuthRoutePaths {
    LOGIN = "/login",
    SIGN_UP = "/sign-up"
};

export const authRoutes: RouteObject[] = [
    {
        path: AuthRoutePaths.LOGIN,
        element: <LoginPage/>
    },
    {
        path: AuthRoutePaths.SIGN_UP,
        element: <SignUpPage/>
    },
];
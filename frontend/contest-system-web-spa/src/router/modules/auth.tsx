import { RouteObject } from "react-router-dom";
import AboutPage from "../../pages/AboutPage";
import LoginPage from "../../pages/auth/LoginPage";

export enum AuthRoutePaths {
    LOGIN = "/login",
    ABOUT = "/about"
};

export const authRoutes: RouteObject[] = [
    {
        path: AuthRoutePaths.LOGIN,
        element: <LoginPage/>
    },
    {
        path: AuthRoutePaths.ABOUT,
        element: <AboutPage/>
    }
];
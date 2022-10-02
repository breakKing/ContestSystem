import { RouteObject } from "react-router-dom";
import HomePage from "../pages/HomePage";
import SignUpPage from "../pages/auth/SignUpPage";
import { AuthRoutePaths, authRoutes } from "./modules/auth";

enum MainRoutePaths {
    HOME = "/",
    SIGN_UP = "/sign-up"
};

export const publicRoutes: RouteObject[] = [
    {
        path: MainRoutePaths.HOME,
        element: <HomePage/>
    },
    {
        path: MainRoutePaths.SIGN_UP,
        element: <SignUpPage/>
    },
    ...authRoutes
];

export const userRoutes: RouteObject[] = [

];

export const moderatorRoutes: RouteObject[] = [

];

export const adminRoutes: RouteObject[] = [

];

export const RoutePaths = { ...MainRoutePaths, ...AuthRoutePaths };
export type RoutePathsList = typeof RoutePaths;
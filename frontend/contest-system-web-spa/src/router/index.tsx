import { RouteObject } from "react-router-dom";
import HomePage from "../pages/HomePage";
import { AuthRoutePaths, authRoutes } from "./modules/auth";
import AboutPage from "../pages/AboutPage";

enum MainRoutePaths {
    HOME = "/",
    ABOUT = "/about"
};

export const publicRoutes: RouteObject[] = [
    {
        path: MainRoutePaths.HOME,
        element: <HomePage/>
    },
    {
        path: MainRoutePaths.ABOUT,
        element: <AboutPage/>
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
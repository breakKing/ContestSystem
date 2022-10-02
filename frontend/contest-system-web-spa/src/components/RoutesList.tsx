import { FunctionComponent } from "react";
import { Route, RouteObject, Routes } from "react-router-dom";
import { useAppSelector } from "../hooks/redux";
import { Roles } from "../models/auth/role";
import { adminRoutes, moderatorRoutes, publicRoutes, userRoutes } from "../router";

interface RoutesListProps {
    children?: JSX.Element
}

const RoutesList: FunctionComponent<RoutesListProps> = (props: RoutesListProps) => {
    const { currentUser } = useAppSelector(state => state.auth);

    function insertRolesSpecificRoutes() : JSX.Element[] {
        let routes: JSX.Element[] = [];
        
        if (currentUser == null) {
            return routes;
        }

        let roles: string[] = currentUser.roles.map(r => r.name);
        
        if (roles.includes(Roles.USER)) {
            routes.push((
                <Routes> {
                    generateRoutes(userRoutes)
                }
                </Routes>
            ))
        }

        if (roles.includes(Roles.MODERATOR)) {
            routes.push((
                <Routes> {
                    generateRoutes(moderatorRoutes)
                }
                </Routes>
            ))
        }

        if (roles.includes(Roles.ADMIN)) {
            routes.push((
                <Routes> {
                    generateRoutes(adminRoutes)
                }
                </Routes>
            ))
        }

        return routes;
    }
    
    let index: number = 0;
    function generateRoutes(routesList: RouteObject[] | undefined) : JSX.Element[] {
        if (routesList === undefined) {
            return [];
        }
    
        return (
            routesList.map(r => {
                return (
                    <Route
                        key = {++index}
                        path = { r.path }
                        element = { r.element }
                        children = { generateRoutes(r.children) }/>
                );
            })
        );
    }
    
    return (
        <>
            <Routes> {
                generateRoutes(publicRoutes)
            }
            </Routes>
            { insertRolesSpecificRoutes() }
        </>
        
    );
}
 
export default RoutesList;

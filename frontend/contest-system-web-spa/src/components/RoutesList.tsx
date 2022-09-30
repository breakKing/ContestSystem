import { FunctionComponent } from "react";
import { Route, RouteObject, Routes } from "react-router-dom";
import routes from "../router";

interface RoutesListProps {
    children?: JSX.Element
}

const RoutesList: FunctionComponent<RoutesListProps> = (props: RoutesListProps) => {
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
        <Routes> {
            generateRoutes(routes)
        }
        </Routes>
    );
}
 
export default RoutesList;


import { FunctionComponent } from "react";
import { BrowserRouter, Route, RouteObject, Routes } from "react-router-dom";
import routes from "../router";

const RouterComponent: FunctionComponent = () => {
    function generateRoutes(routesList: RouteObject[] | undefined) : JSX.Element[] | null {
        if (routesList === undefined) {
            return null;
        }
    
        return (
            routesList.map(r => {
                return (
                    <Route
                        key = { r.path }
                        path = { r.path }
                        element = { r.element }
                        children = { generateRoutes(r.children)}/>
                );
            })
        );
    }
    
    return ( 
        <BrowserRouter>
            <Routes> {
                generateRoutes(routes)
            }
            </Routes>
        </BrowserRouter>
    );
}
 
export default RouterComponent;


import { FunctionComponent } from "react";
import { Image } from 'antd';
import { Link } from "react-router-dom";
import { RoutePaths } from "../../../router";

const Logo: FunctionComponent = () => {

    return (
        <Link to={RoutePaths.HOME}>
            <Image className="logo"
                src="logo.svg"
                preview={ false }
            />
        </Link> 
    );
}
 
export default Logo;
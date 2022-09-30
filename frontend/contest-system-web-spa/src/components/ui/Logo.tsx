import { FunctionComponent } from "react";
import { Image } from 'antd';
import { Link } from "react-router-dom";

interface LogoProps {
    
}
 
const Logo: FunctionComponent<LogoProps> = (props: LogoProps) => {

    return (
        <Link to="/">
            <Image className="logo"
                src="logo.svg"
                preview={ false }
            />
        </Link> 
    );
}
 
export default Logo;
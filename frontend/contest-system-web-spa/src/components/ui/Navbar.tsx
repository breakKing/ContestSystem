import { Button, Col, Row } from "antd";
import { FunctionComponent } from "react";
import Logo from "./Logo";
import "../../styles/Navbar.scss";

interface NavbarProps {
    
}
 
const Navbar: FunctionComponent<NavbarProps> = (props: NavbarProps) => {
    return ( 
        <Row>
            <Col>
                <Logo/>
            </Col>
            <Col flex="auto"/>
            <Col>
                <Button
                    type="primary">
                        Войти
                </Button>
            </Col>
        </Row>
    );
}
 
export default Navbar;
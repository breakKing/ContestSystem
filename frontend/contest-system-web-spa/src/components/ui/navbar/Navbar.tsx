import { FunctionComponent } from "react";
import { Col, Layout, Row } from "antd";
import LeftSection from "./LeftSection";
import RightSection from "./RightSection";
import Logo from "./Logo";

import "../../../styles/Navbar.scss";

const Navbar: FunctionComponent = () => {
    return ( 
        <Layout className="navbar">
            <Row align="middle" justify="space-around">
                <Col xs={18} sm={18}  md={18} lg={4}>
                    <Logo/>
                </Col>
                <Col xs={0} sm={0} md={0} lg={10}>
                    <LeftSection/>
                </Col>
                <Col xs={6} sm={6} md={6} lg={10}>
                    <RightSection/>
                </Col>
            </Row>
        </Layout>
    );
}
 
export default Navbar;
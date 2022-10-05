import { FunctionComponent } from "react";
import { Col, Layout, Menu, Row } from "antd";
import { leftMenuItems } from "./MenuItems";

const LeftSection: FunctionComponent = () => {
    return (
        <Layout>
            <Row align="middle" justify="start">
                <Col xs={0} sm={0} md={0} lg={24}>
                    <Menu
                        theme="dark"
                        mode="horizontal"
                        selectable={false}
                        items={leftMenuItems}/>
                </Col>
            </Row>
        </Layout>
        
    )
};

export default LeftSection;
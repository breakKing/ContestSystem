import { FunctionComponent } from "react";
import { Col, Image, Layout, Row } from 'antd';
import { Link } from "react-router-dom";
import { RoutePaths } from "../../../router";

const Logo: FunctionComponent = () => {

    return (
        <Layout>
            <Row align="middle" justify="start">
                <Col span={24}>
                    <Link to={RoutePaths.HOME}>
                        <Image
                            src="logo.svg"
                            preview={ false }
                        />
                    </Link> 
                </Col>
            </Row>
        </Layout>
    );
}
 
export default Logo;
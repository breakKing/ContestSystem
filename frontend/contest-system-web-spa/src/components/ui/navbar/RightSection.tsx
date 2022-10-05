import { BarsOutlined } from "@ant-design/icons";
import { Button, Col, Drawer, Layout, Menu, MenuProps, Row } from "antd";
import { FunctionComponent, useState } from "react";
import { Link } from "react-router-dom";
import { useAppSelector } from "../../../hooks/redux";
import { RoutePaths } from "../../../router";
import { leftMenuItems, rightMenuItems } from "./MenuItems";

const RightSection: FunctionComponent = () => {
    const [visible, setVisible] = useState<boolean>(false);
    const {currentUser} = useAppSelector(state => state.auth);

    const menuItems: MenuProps["items"] = [
        {
            key: "home",
            label: <Link to={RoutePaths.HOME}>Главная</Link>
        },
        ...leftMenuItems || [],
        ...rightMenuItems(currentUser) || []
    ]

    const showDrawer = () => {
        setVisible(true);
    }

    const onDrawerClose = () => {
        setVisible(false);
    }

    const onDrawerMenuItemClicked = () => {
        setVisible(false);
    }
    
    return (
        <Layout>
            <Row align="middle" justify="end">
                <Col xs={0} sm={0} md={0} lg={24}>
                    <Menu
                        theme="dark"
                        className="right-menu"
                        mode="horizontal"
                        selectable={false}
                        items={rightMenuItems(currentUser)}/>
                </Col>
                <Col xs={24} sm={24} md={24} lg={0}>
                    <Row align="middle" justify="end">
                        <Button
                            type="primary"
                            icon={<BarsOutlined />}
                            onClick={showDrawer}/>
                    </Row>
                </Col>
                <Drawer
                    title="ContestSystem"
                    placement="top"
                    closable={true}
                    size="large"
                    onClose={onDrawerClose}
                    open={visible}
                >
                    <Menu
                        mode="inline"
                        theme="dark"
                        items={menuItems}
                        selectable={false}
                        onClick={onDrawerMenuItemClicked}
                    />
                </Drawer>
            </Row>
        </Layout>
    );
}

export default RightSection;
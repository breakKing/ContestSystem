import { Button, Col, Drawer, Menu, MenuProps, Row } from "antd";
import { FunctionComponent, useState } from "react";
import LeftSection from "./LeftSection";
import RightSection from "./RightSection";
import Logo from "./Logo";
import { BarsOutlined } from "@ant-design/icons";
import { leftMenuItems, rightMenuItems } from "./MenuItems";
import { Link } from "react-router-dom";

import "../../../styles/Navbar.scss";
import { RoutePaths } from "../../../router";
import { useAppSelector } from "../../../hooks/redux";

const Navbar: FunctionComponent = () => {
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
        <Row className="navbar">
            <Col xs={18} sm={18}  md={18} lg={6}>
                <Logo/>
            </Col>
            <Col xs={0} sm={0} md={0} lg={9}>
                <LeftSection/>
            </Col>
            <Col xs={0} sm={0} md={0} lg={9}>
                <RightSection/>
            </Col>
            <Col xs={6} sm={6} md={6} lg={0}>
                <div className="burger__wrapper">
                    <Button
                        type="primary"
                        icon={<BarsOutlined />}
                        onClick={showDrawer}
                    />
                </div>
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
    );
}
 
export default Navbar;
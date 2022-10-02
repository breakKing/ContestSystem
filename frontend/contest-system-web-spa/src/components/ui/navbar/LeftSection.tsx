import { FunctionComponent } from "react";
import { Menu } from "antd";
import { leftMenuItems } from "./MenuItems";

const LeftSection: FunctionComponent = () => {
    return (
        <Menu
            theme="dark"
            mode="horizontal"
            selectable={false}
            items={leftMenuItems}>
        </Menu>
    )
};

export default LeftSection;
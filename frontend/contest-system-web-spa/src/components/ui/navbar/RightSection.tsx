import { Menu } from "antd";
import { FunctionComponent } from "react";
import { useAppSelector } from "../../../hooks/redux";
import { rightMenuItems } from "./MenuItems";

const RightSection: FunctionComponent = () => {
    const {currentUser} = useAppSelector(state => state.auth);
    
    return (
        <Menu
            className="right"
            theme="dark"
            mode="horizontal"
            selectable={false}
            items={rightMenuItems(currentUser)}
        >
        </Menu>
    );
}

export default RightSection;
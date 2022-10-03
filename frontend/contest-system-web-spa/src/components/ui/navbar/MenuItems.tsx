import { UserOutlined } from "@ant-design/icons";
import { MenuProps } from "antd";
import { Link } from "react-router-dom";
import { ICurrentUser } from "../../../models/auth/current-user";
import { RoutePaths } from "../../../router";

type MenuItem = Required<MenuProps>["items"][number];

function getItem(
    key: React.Key,
    label: React.ReactNode,
    icon?: React.ReactNode,
    children?: MenuItem[]
): MenuItem {
    return {
        key,
        label,
        icon,
        children
    } as MenuItem;
}

export const leftMenuItems: MenuItem[] = [
    getItem("about", <Link to={RoutePaths.ABOUT}>О нас</Link>)
];

export const rightMenuItems = (user: ICurrentUser | null) => {
    if (user === null) {
        return [
            getItem("sign-up", <Link to={RoutePaths.SIGN_UP}>Регистрация</Link>),
            getItem("login", <Link to={RoutePaths.LOGIN}>Войти</Link>)
        ] as MenuItem[];
    }

    return [
        getItem("profile", <span>{user.username}</span>, <UserOutlined size={1}/>, [
            getItem("profile-open", <span>Профиль</span>),
            getItem("profile-settings", <span>Настройки</span>),
            getItem("profile-logout", <span>Выйти</span>)
        ])
    ] as MenuItem[];
}

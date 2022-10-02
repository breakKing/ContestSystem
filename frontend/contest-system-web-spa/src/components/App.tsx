import { ConfigProvider, Layout } from "antd";
import { FunctionComponent } from "react";
import RoutesList from "./RoutesList";
import Navbar from "./ui/navbar/Navbar";

import "../styles/App.scss";

const App: FunctionComponent = () => {
    const { Header, Content, Footer } = Layout;

    ConfigProvider.config({
        theme: {
            primaryColor: "#59B7CE",
        }
    });

    return (
        <Layout className="layout">
            <Header className="header">
                <Navbar />
            </Header>
            <Content className="content">
                <RoutesList />
            </Content>
            <Footer className="footer">
                ContestSystem ©2022
            </Footer>
        </Layout>
    )
}

export default App;
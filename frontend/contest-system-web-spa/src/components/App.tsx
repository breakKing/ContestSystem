import { Layout } from "antd";
import { FunctionComponent } from "react";
import RoutesList from "./RoutesList";
import "../styles/App.scss";
import Navbar from "./ui/Navbar";

const App: FunctionComponent = () => {
    const { Header, Content, Footer } = Layout;

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
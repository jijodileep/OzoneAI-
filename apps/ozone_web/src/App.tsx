import { Layout, Menu, Typography, theme } from 'antd'
import {
  DashboardOutlined,
  SettingOutlined,
  AppstoreOutlined,
} from '@ant-design/icons'
import { FinancialYearSwitcher } from './components/FinancialYearSwitcher'
import './App.css'

const { Header, Sider, Content } = Layout

export default function App() {
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken()

  return (
    <Layout className="app-shell">
      <Sider breakpoint="lg" collapsedWidth={64} theme="light" className="app-sider">
        <div className="app-brand">OzoneAI</div>
        <Menu
          mode="inline"
          defaultSelectedKeys={['home']}
          items={[
            { key: 'home', icon: <DashboardOutlined />, label: 'Home' },
            { key: 'masters', icon: <AppstoreOutlined />, label: 'Masters' },
            { key: 'settings', icon: <SettingOutlined />, label: 'Settings' },
          ]}
        />
      </Sider>
      <Layout>
        <Header className="app-header" style={{ background: colorBgContainer }}>
          <Typography.Text type="secondary">Admin shell</Typography.Text>
          <FinancialYearSwitcher apiBaseUrl="" />
        </Header>
        <Content className="app-content">
          <div
            className="app-panel"
            style={{
              background: colorBgContainer,
              borderRadius: borderRadiusLG,
            }}
          >
            <Typography.Title level={3} style={{ marginTop: 0 }}>
              Welcome
            </Typography.Title>
            <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
              Blank admin app is ready. Wire modules here after auth (E2/E3).
            </Typography.Paragraph>
          </div>
        </Content>
      </Layout>
    </Layout>
  )
}

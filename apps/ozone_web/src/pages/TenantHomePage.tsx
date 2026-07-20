import { useEffect, useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { Alert, Button, Layout, Menu, Typography, theme } from 'antd'
import {
  DashboardOutlined,
  SettingOutlined,
  AppstoreOutlined,
} from '@ant-design/icons'
import { FinancialYearSwitcher } from '../components/FinancialYearSwitcher'
import { clearTenantToken, getTenantToken, tenantAuthHeaders } from '../auth/tenantToken'
import '../App.css'

const { Header, Sider, Content } = Layout

type MeResponse = {
  id?: string
  username?: string
  role?: string
  companyKey?: string
  companyId?: string
  impersonatedBy?: string | null
}

export function TenantHomePage() {
  const navigate = useNavigate()
  const token = getTenantToken()
  const [me, setMe] = useState<MeResponse | null>(null)
  const [error, setError] = useState<string>()
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken()

  useEffect(() => {
    if (!token) return
    let cancelled = false
    async function load() {
      try {
        const res = await fetch('/v1/auth/me', { headers: tenantAuthHeaders() })
        if (res.status === 401) {
          clearTenantToken()
          navigate('/login', { replace: true })
          return
        }
        if (!res.ok) throw new Error('Failed to load session')
        if (!cancelled) setMe((await res.json()) as MeResponse)
      } catch {
        if (!cancelled) setError('Could not load tenant session.')
      }
    }
    void load()
    return () => {
      cancelled = true
    }
  }, [token, navigate])

  if (!token) {
    return <Navigate to="/login" replace />
  }

  function logout() {
    clearTenantToken()
    navigate('/login', { replace: true })
  }

  function endImpersonation() {
    clearTenantToken()
    navigate('/super-admin', { replace: true })
  }

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
          <Typography.Text type="secondary">
            {me?.companyKey ?? '…'} · {me?.username ?? '…'}
          </Typography.Text>
          <div style={{ display: 'flex', gap: 16, alignItems: 'center' }}>
            <FinancialYearSwitcher apiBaseUrl="" />
            <Link to="/super-admin">Super Admin</Link>
            <Button size="small" onClick={logout}>
              Sign out
            </Button>
          </div>
        </Header>
        <Content className="app-content">
          {me?.impersonatedBy ? (
            <Alert
              type="warning"
              showIcon
              style={{ marginBottom: 16 }}
              message={`Impersonating ${me.companyKey ?? 'tenant'}`}
              action={
                <Button size="small" onClick={endImpersonation}>
                  End session
                </Button>
              }
            />
          ) : null}
          <div
            className="app-panel"
            style={{
              background: colorBgContainer,
              borderRadius: borderRadiusLG,
            }}
          >
            {error ? <Alert type="error" message={error} showIcon style={{ marginBottom: 16 }} /> : null}
            <Typography.Title level={3} style={{ marginTop: 0 }}>
              Welcome{me?.username ? `, ${me.username}` : ''}
            </Typography.Title>
            <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
              Tenant shell for {me?.companyKey ?? '…'} ({me?.role ?? '…'}).
            </Typography.Paragraph>
          </div>
        </Content>
      </Layout>
    </Layout>
  )
}

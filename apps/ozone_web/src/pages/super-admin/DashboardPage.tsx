import { useEffect, useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { Button, Layout, Typography, theme } from 'antd'
import { clearPlatformToken, getPlatformToken, platformAuthHeaders } from '../../auth/platformToken'

const { Header, Content } = Layout

type MeResponse = {
  id?: string
  username?: string
  role?: string
  scope?: string
}

export function SuperAdminDashboardPage() {
  const navigate = useNavigate()
  const token = getPlatformToken()
  const [me, setMe] = useState<MeResponse | null>(null)
  const [companyCount, setCompanyCount] = useState<number | null>(null)
  const [error, setError] = useState<string>()
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken()

  useEffect(() => {
    if (!token) return
    let cancelled = false
    async function load() {
      try {
        const [meRes, countRes] = await Promise.all([
          fetch('/v1/platform/me', { headers: platformAuthHeaders() }),
          fetch('/catalog/companies/count', { headers: platformAuthHeaders() }),
        ])
        if (meRes.status === 401 || countRes.status === 401) {
          clearPlatformToken()
          navigate('/super-admin/login', { replace: true })
          return
        }
        if (!meRes.ok || !countRes.ok) {
          throw new Error('Failed to load platform session')
        }
        const meJson = (await meRes.json()) as MeResponse
        const countJson = (await countRes.json()) as { companies: number }
        if (!cancelled) {
          setMe(meJson)
          setCompanyCount(countJson.companies)
        }
      } catch {
        if (!cancelled) setError('Could not load Super Admin session.')
      }
    }
    void load()
    return () => {
      cancelled = true
    }
  }, [navigate, token])

  if (!token) {
    return <Navigate to="/super-admin/login" replace />
  }

  function logout() {
    clearPlatformToken()
    navigate('/super-admin/login', { replace: true })
  }

  return (
    <Layout className="app-shell">
      <Header className="app-header" style={{ background: colorBgContainer }}>
        <Typography.Text strong>OzoneAI Super Admin</Typography.Text>
        <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
          <Typography.Text type="secondary">{me?.username ?? '…'}</Typography.Text>
          <Button onClick={logout}>Sign out</Button>
        </div>
      </Header>
      <Content className="app-content">
        <div
          className="app-panel"
          style={{ background: colorBgContainer, borderRadius: borderRadiusLG }}
        >
          <Typography.Title level={3} style={{ marginTop: 0 }}>
            Platform console
          </Typography.Title>
          {error ? (
            <Typography.Paragraph type="danger">{error}</Typography.Paragraph>
          ) : (
            <>
              <Typography.Paragraph type="secondary">
                Catalog-scoped session ({me?.scope ?? '…'} / {me?.role ?? '…'}).
              </Typography.Paragraph>
              <Typography.Paragraph>
                Registered companies: <strong>{companyCount ?? '…'}</strong>
              </Typography.Paragraph>
            </>
          )}
          <Link to="/">Tenant admin shell</Link>
        </div>
      </Content>
    </Layout>
  )
}

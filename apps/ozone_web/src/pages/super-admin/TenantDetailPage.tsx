import { useCallback, useEffect, useState } from 'react'
import { Link, Navigate, useNavigate, useParams } from 'react-router-dom'
import { Alert, Button, Descriptions, Layout, Space, Tag, Typography, message, theme } from 'antd'
import { clearPlatformToken, getPlatformToken, platformAuthHeaders } from '../../auth/platformToken'

const { Header, Content } = Layout

type TenantDetail = {
  id: string
  name: string
  companyKey: string
  databaseName: string
  status: string
  legacyMigrationStatus: string
  timeZoneId?: string | null
  planName?: string | null
  createdAt: string
  lastUsedAt?: string | null
  activeUsers30d: number
  totalUsersCached: number
  schemaVersion?: string | null
  credentials: Array<{
    id: string
    role: string
    host: string
    port: number
    username: string
    passwordSet: boolean
    isActive: boolean
  }>
}

export function SuperAdminTenantDetailPage() {
  const { companyId } = useParams<{ companyId: string }>()
  const navigate = useNavigate()
  const token = getPlatformToken()
  const [detail, setDetail] = useState<TenantDetail | null>(null)
  const [error, setError] = useState<string>()
  const [busy, setBusy] = useState(false)
  const [accessMsg, setAccessMsg] = useState<string>()
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken()

  const load = useCallback(async () => {
    if (!companyId) return
    const res = await fetch(`/v1/platform/tenants/${companyId}`, {
      headers: platformAuthHeaders(),
    })
    if (res.status === 401) {
      clearPlatformToken()
      navigate('/super-admin/login', { replace: true })
      return
    }
    if (res.status === 404) {
      setError('Tenant not found.')
      return
    }
    if (!res.ok) throw new Error('Failed to load tenant')
    setDetail((await res.json()) as TenantDetail)
  }, [companyId, navigate])

  useEffect(() => {
    if (!token) return
    void load().catch(() => setError('Could not load tenant detail.'))
  }, [token, load])

  if (!token) {
    return <Navigate to="/super-admin/login" replace />
  }

  async function setStatus(action: 'suspend' | 'activate') {
    if (!companyId) return
    setBusy(true)
    setAccessMsg(undefined)
    try {
      const res = await fetch(`/v1/platform/tenants/${companyId}/${action}`, {
        method: 'POST',
        headers: platformAuthHeaders(),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string } | null
        throw new Error(body?.error ?? `${action} failed`)
      }
      setDetail((await res.json()) as TenantDetail)
      message.success(action === 'suspend' ? 'Tenant suspended' : 'Tenant activated')
    } catch (e) {
      message.error(e instanceof Error ? e.message : 'Action failed')
    } finally {
      setBusy(false)
    }
  }

  async function checkLoginAccess() {
    if (!detail) return
    const res = await fetch('/v1/auth/company-access', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ companyKey: detail.companyKey }),
    })
    const body = (await res.json()) as { allowed: boolean; reason?: string; status?: string }
    setAccessMsg(
      body.allowed
        ? `Login allowed (status ${body.status}).`
        : `Login blocked: ${body.reason ?? 'forbidden'}`,
    )
  }

  const statusColor =
    detail?.status === 'Active'
      ? 'success'
      : detail?.status === 'Suspended'
        ? 'warning'
        : 'default'

  return (
    <Layout className="app-shell">
      <Header className="app-header" style={{ background: colorBgContainer }}>
        <Typography.Text strong>OzoneAI Super Admin</Typography.Text>
        <Link to="/super-admin">← Tenants</Link>
      </Header>
      <Content className="app-content">
        <div
          className="app-panel"
          style={{ background: colorBgContainer, borderRadius: borderRadiusLG }}
        >
          {error ? <Alert type="error" message={error} showIcon /> : null}
          {!error && !detail ? <Typography.Text type="secondary">Loading…</Typography.Text> : null}
          {detail ? (
            <>
              <div style={{ display: 'flex', justifyContent: 'space-between', gap: 16 }}>
                <div>
                  <Typography.Title level={3} style={{ marginTop: 0, marginBottom: 4 }}>
                    {detail.name}
                  </Typography.Title>
                  <Space>
                    <Tag color={statusColor}>{detail.status}</Tag>
                    <Typography.Text type="secondary">{detail.companyKey}</Typography.Text>
                  </Space>
                </div>
                <Space>
                  {detail.status === 'Active' ? (
                    <Button danger loading={busy} onClick={() => void setStatus('suspend')}>
                      Suspend
                    </Button>
                  ) : null}
                  {detail.status === 'Suspended' ? (
                    <Button type="primary" loading={busy} onClick={() => void setStatus('activate')}>
                      Activate
                    </Button>
                  ) : null}
                  <Button onClick={() => void checkLoginAccess()}>Test login access</Button>
                </Space>
              </div>

              {accessMsg ? (
                <Alert
                  style={{ marginTop: 16 }}
                  type={accessMsg.startsWith('Login allowed') ? 'success' : 'warning'}
                  message={accessMsg}
                  showIcon
                />
              ) : null}

              <Descriptions
                bordered
                size="small"
                column={1}
                style={{ marginTop: 24 }}
                items={[
                  { key: 'db', label: 'Database', children: detail.databaseName },
                  { key: 'plan', label: 'Plan', children: detail.planName ?? '—' },
                  { key: 'tz', label: 'Timezone', children: detail.timeZoneId ?? '—' },
                  {
                    key: 'created',
                    label: 'Created',
                    children: new Date(detail.createdAt).toLocaleString(),
                  },
                  {
                    key: 'last',
                    label: 'Last used',
                    children: detail.lastUsedAt
                      ? new Date(detail.lastUsedAt).toLocaleString()
                      : '—',
                  },
                  { key: 'active', label: 'Active users (30d)', children: detail.activeUsers30d },
                  { key: 'total', label: 'Total users', children: detail.totalUsersCached },
                  { key: 'schema', label: 'Schema version', children: detail.schemaVersion ?? '—' },
                  {
                    key: 'legacy',
                    label: 'Legacy migration',
                    children: detail.legacyMigrationStatus,
                  },
                  {
                    key: 'creds',
                    label: 'DB credentials',
                    children: detail.credentials
                      .map(
                        (c) =>
                          `${c.role}: ${c.username}@${c.host}:${c.port}${c.isActive ? '' : ' (inactive)'}`,
                      )
                      .join(' · ') || '—',
                  },
                ]}
              />
            </>
          ) : null}
        </div>
      </Content>
    </Layout>
  )
}

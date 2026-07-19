import { useCallback, useEffect, useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import {
  Alert,
  Button,
  Form,
  Input,
  Layout,
  Modal,
  Table,
  Typography,
  message,
  theme,
} from 'antd'
import { clearPlatformToken, getPlatformToken, platformAuthHeaders } from '../../auth/platformToken'

const { Header, Content } = Layout

type MeResponse = {
  id?: string
  username?: string
  role?: string
  scope?: string
}

type TenantRow = {
  id: string
  name: string
  companyKey: string
  databaseName: string
  status: string
  createdAt: string
  lastUsedAt?: string | null
  activeUsers30d: number
  totalUsersCached: number
}

type CreateTenantForm = {
  name: string
  companyKey: string
  adminUsername: string
  adminPassword: string
  adminDisplayName?: string
  timeZoneId?: string
}

export function SuperAdminDashboardPage() {
  const navigate = useNavigate()
  const token = getPlatformToken()
  const [me, setMe] = useState<MeResponse | null>(null)
  const [tenants, setTenants] = useState<TenantRow[]>([])
  const [error, setError] = useState<string>()
  const [createOpen, setCreateOpen] = useState(false)
  const [creating, setCreating] = useState(false)
  const [refreshing, setRefreshing] = useState(false)
  const [form] = Form.useForm<CreateTenantForm>()
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken()

  const loadTenants = useCallback(async () => {
    const res = await fetch('/v1/platform/tenants', { headers: platformAuthHeaders() })
    if (res.status === 401) {
      clearPlatformToken()
      navigate('/super-admin/login', { replace: true })
      return
    }
    if (!res.ok) throw new Error('Failed to load tenants')
    setTenants((await res.json()) as TenantRow[])
  }, [navigate])

  useEffect(() => {
    if (!token) return
    let cancelled = false
    async function load() {
      try {
        const meRes = await fetch('/v1/platform/me', { headers: platformAuthHeaders() })
        if (meRes.status === 401) {
          clearPlatformToken()
          navigate('/super-admin/login', { replace: true })
          return
        }
        if (!meRes.ok) throw new Error('Failed to load platform session')
        const meJson = (await meRes.json()) as MeResponse
        if (!cancelled) setMe(meJson)
        await loadTenants()
      } catch {
        if (!cancelled) setError('Could not load Super Admin session.')
      }
    }
    void load()
    return () => {
      cancelled = true
    }
  }, [navigate, token, loadTenants])

  if (!token) {
    return <Navigate to="/super-admin/login" replace />
  }

  function logout() {
    clearPlatformToken()
    navigate('/super-admin/login', { replace: true })
  }

  async function refreshMetrics() {
    setRefreshing(true)
    try {
      const res = await fetch('/v1/platform/tenants/metrics/refresh', {
        method: 'POST',
        headers: platformAuthHeaders(),
      })
      if (res.status === 401) {
        clearPlatformToken()
        navigate('/super-admin/login', { replace: true })
        return
      }
      if (!res.ok) throw new Error('Metrics refresh failed')
      const body = (await res.json()) as { tenantsUpdated: number; tenantsFailed: number }
      message.success(`Metrics updated (${body.tenantsUpdated} ok, ${body.tenantsFailed} failed)`)
      await loadTenants()
    } catch (e) {
      message.error(e instanceof Error ? e.message : 'Metrics refresh failed')
    } finally {
      setRefreshing(false)
    }
  }

  async function onCreate(values: CreateTenantForm) {
    setCreating(true)
    try {
      const res = await fetch('/v1/platform/tenants', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...platformAuthHeaders(),
        },
        body: JSON.stringify({
          name: values.name,
          companyKey: values.companyKey,
          adminUsername: values.adminUsername,
          adminPassword: values.adminPassword,
          adminDisplayName: values.adminDisplayName,
          timeZoneId: values.timeZoneId || 'Asia/Kolkata',
        }),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string } | null
        throw new Error(body?.error ?? `Create failed (${res.status})`)
      }
      message.success('Tenant provisioned')
      setCreateOpen(false)
      form.resetFields()
      await loadTenants()
    } catch (e) {
      message.error(e instanceof Error ? e.message : 'Create failed')
    } finally {
      setCreating(false)
    }
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
          <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
            <div>
              <Typography.Title level={3} style={{ marginTop: 0, marginBottom: 4 }}>
                Tenants
              </Typography.Title>
              <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
                Catalog-scoped ({me?.scope ?? '…'} / {me?.role ?? '…'})
              </Typography.Paragraph>
            </div>
            <div style={{ display: 'flex', gap: 8 }}>
              <Button onClick={() => void refreshMetrics()} loading={refreshing}>
                Refresh metrics
              </Button>
              <Button type="primary" onClick={() => setCreateOpen(true)}>
                Create tenant
              </Button>
            </div>
          </div>

          {error ? <Alert type="error" message={error} showIcon style={{ marginBottom: 16 }} /> : null}

          <Table
            rowKey="id"
            dataSource={tenants}
            pagination={false}
            columns={[
              {
                title: 'Name',
                dataIndex: 'name',
                render: (name: string, row: TenantRow) => (
                  <Link to={`/super-admin/tenants/${row.id}`}>{name}</Link>
                ),
              },
              { title: 'Key', dataIndex: 'companyKey' },
              { title: 'Database', dataIndex: 'databaseName' },
              { title: 'Status', dataIndex: 'status' },
              {
                title: 'Last used',
                dataIndex: 'lastUsedAt',
                render: (v: string | null | undefined) =>
                  v ? new Date(v).toLocaleString() : '—',
              },
              {
                title: 'Active users (30d)',
                dataIndex: 'activeUsers30d',
              },
              {
                title: 'Total users',
                dataIndex: 'totalUsersCached',
              },
              {
                title: 'Created',
                dataIndex: 'createdAt',
                render: (v: string) => new Date(v).toLocaleString(),
              },
              {
                title: '',
                key: 'open',
                render: (_: unknown, row: TenantRow) => (
                  <Link to={`/super-admin/tenants/${row.id}`}>Open</Link>
                ),
              },
            ]}
          />

          <div style={{ marginTop: 16 }}>
            <Link to="/">Tenant admin shell</Link>
          </div>
        </div>
      </Content>

      <Modal
        title="Create tenant"
        open={createOpen}
        onCancel={() => setCreateOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={onCreate}
          initialValues={{
            adminUsername: 'admin',
            timeZoneId: 'Asia/Kolkata',
          }}
        >
          <Form.Item label="Company name" name="name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item
            label="Company key"
            name="companyKey"
            extra="Lowercase; becomes ozone_t_{key}"
            rules={[
              { required: true },
              {
                pattern: /^[a-z][a-z0-9_]{1,31}$/,
                message: '2–32 chars: a-z, then a-z/0-9/_',
              },
            ]}
          >
            <Input placeholder="acme" />
          </Form.Item>
          <Form.Item label="Admin username" name="adminUsername" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item
            label="Admin password"
            name="adminPassword"
            rules={[{ required: true, min: 8 }]}
          >
            <Input.Password />
          </Form.Item>
          <Form.Item label="Admin display name" name="adminDisplayName">
            <Input />
          </Form.Item>
          <Form.Item label="Timezone" name="timeZoneId">
            <Input />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={creating}>
            Provision database
          </Button>
        </Form>
      </Modal>
    </Layout>
  )
}

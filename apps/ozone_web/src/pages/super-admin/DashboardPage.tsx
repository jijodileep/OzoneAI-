import { useCallback, useEffect, useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import {
  Alert,
  Button,
  Form,
  Input,
  InputNumber,
  Layout,
  Modal,
  Select,
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

type PlanRow = {
  id: string
  name: string
  maxUsers: number
  maxGodowns: number
  isActive: boolean
}

type CreateTenantForm = {
  name: string
  companyKey: string
  adminUsername: string
  adminPassword: string
  adminDisplayName?: string
  adminEmail?: string
  planId?: string
  timeZoneId?: string
  legalName?: string
  address?: string
  phone?: string
  email?: string
  taxType?: string
  currencyCode?: string
  dbMode: 'Provisioned' | 'External'
  dbHost?: string
  dbPort?: number
  databaseName?: string
  dbUsername?: string
  dbPassword?: string
  sslMode?: string
}

export function SuperAdminDashboardPage() {
  const navigate = useNavigate()
  const token = getPlatformToken()
  const [me, setMe] = useState<MeResponse | null>(null)
  const [tenants, setTenants] = useState<TenantRow[]>([])
  const [plans, setPlans] = useState<PlanRow[]>([])
  const [error, setError] = useState<string>()
  const [createOpen, setCreateOpen] = useState(false)
  const [creating, setCreating] = useState(false)
  const [refreshing, setRefreshing] = useState(false)
  const [testEmailOpen, setTestEmailOpen] = useState(false)
  const [testingEmail, setTestingEmail] = useState(false)
  const [form] = Form.useForm<CreateTenantForm>()
  const [emailForm] = Form.useForm<{ toAddress: string }>()
  const dbMode = Form.useWatch('dbMode', form)
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

  const loadPlans = useCallback(async () => {
    const res = await fetch('/catalog/plans', { headers: platformAuthHeaders() })
    if (res.ok) {
      setPlans((await res.json()) as PlanRow[])
    }
  }, [])

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
        await Promise.all([loadTenants(), loadPlans()])
      } catch {
        if (!cancelled) setError('Could not load Super Admin session.')
      }
    }
    void load()
    return () => {
      cancelled = true
    }
  }, [navigate, token, loadTenants, loadPlans])

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
          adminEmail: values.adminEmail,
          planId: values.planId || null,
          timeZoneId: values.timeZoneId || 'Asia/Kolkata',
          legalName: values.legalName,
          address: values.address,
          phone: values.phone,
          email: values.email,
          taxType: values.taxType || 'GST',
          currencyCode: values.currencyCode || 'INR',
          dbMode: values.dbMode,
          dbHost: values.dbHost,
          dbPort: values.dbPort,
          databaseName: values.databaseName,
          dbUsername: values.dbUsername,
          dbPassword: values.dbPassword,
          sslMode: values.sslMode || null,
        }),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string; detail?: string } | null
        throw new Error(body?.error ?? body?.detail ?? `Create failed (${res.status})`)
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

  async function onTestEmail(values: { toAddress: string }) {
    setTestingEmail(true)
    try {
      const res = await fetch('/v1/platform/email/test', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...platformAuthHeaders(),
        },
        body: JSON.stringify({ toAddress: values.toAddress }),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string; detail?: string } | null
        throw new Error(body?.error ?? body?.detail ?? 'Test email failed')
      }
      const body = (await res.json()) as { message: string }
      message.success(body.message)
      setTestEmailOpen(false)
      emailForm.resetFields()
    } catch (e) {
      message.error(e instanceof Error ? e.message : 'Test email failed')
    } finally {
      setTestingEmail(false)
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
            <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
              <Button onClick={() => setTestEmailOpen(true)}>Test SMTP</Button>
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
        width={640}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={onCreate}
          initialValues={{
            adminUsername: 'admin',
            timeZoneId: 'Asia/Kolkata',
            taxType: 'GST',
            currencyCode: 'INR',
            dbMode: 'Provisioned',
            dbPort: 5432,
          }}
        >
          <Form.Item label="Company name" name="name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item
            label="Company key"
            name="companyKey"
            extra="Lowercase; default DB name ozone_t_{key}"
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
          <Form.Item label="Plan" name="planId">
            <Select
              allowClear
              placeholder="Default active plan"
              options={plans
                .filter((p) => p.isActive)
                .map((p) => ({ value: p.id, label: `${p.name} (${p.maxUsers} users)` }))}
            />
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
          <Form.Item
            label="Admin email"
            name="adminEmail"
            extra="Used for forgot-password"
            rules={[{ type: 'email', message: 'Enter a valid email' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item label="Legal name" name="legalName">
            <Input />
          </Form.Item>
          <Form.Item label="Address" name="address">
            <Input.TextArea rows={2} />
          </Form.Item>
          <Form.Item label="Phone" name="phone">
            <Input />
          </Form.Item>
          <Form.Item label="Company email" name="email" rules={[{ type: 'email' }]}>
            <Input />
          </Form.Item>
          <Form.Item label="Tax type" name="taxType">
            <Input />
          </Form.Item>
          <Form.Item label="Currency" name="currencyCode">
            <Input />
          </Form.Item>
          <Form.Item label="Timezone" name="timeZoneId">
            <Input />
          </Form.Item>
          <Form.Item label="Database mode" name="dbMode" rules={[{ required: true }]}>
            <Select
              options={[
                { value: 'Provisioned', label: 'Provisioned (create on platform Postgres)' },
                { value: 'External', label: 'External (attach existing database)' },
              ]}
            />
          </Form.Item>
          {dbMode === 'External' ? (
            <>
              <Form.Item label="DB host" name="dbHost" rules={[{ required: true }]}>
                <Input placeholder="db.example.com" />
              </Form.Item>
              <Form.Item label="DB port" name="dbPort" rules={[{ required: true }]}>
                <InputNumber min={1} max={65535} style={{ width: '100%' }} />
              </Form.Item>
              <Form.Item label="Database name" name="databaseName" rules={[{ required: true }]}>
                <Input placeholder="ozone_t_acme" />
              </Form.Item>
              <Form.Item label="DB username" name="dbUsername" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
              <Form.Item label="DB password" name="dbPassword" rules={[{ required: true }]}>
                <Input.Password />
              </Form.Item>
              <Form.Item label="SSL mode" name="sslMode" extra="Optional (e.g. Prefer, Require)">
                <Select
                  allowClear
                  options={[
                    { value: 'Disable', label: 'Disable' },
                    { value: 'Prefer', label: 'Prefer' },
                    { value: 'Require', label: 'Require' },
                  ]}
                />
              </Form.Item>
            </>
          ) : null}
          <Button type="primary" htmlType="submit" block loading={creating}>
            {dbMode === 'External' ? 'Attach & migrate' : 'Provision database'}
          </Button>
        </Form>
      </Modal>

      <Modal
        title="Test SMTP"
        open={testEmailOpen}
        onCancel={() => setTestEmailOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Form form={emailForm} layout="vertical" onFinish={onTestEmail}>
          <Form.Item
            label="To address"
            name="toAddress"
            rules={[{ required: true, type: 'email' }]}
          >
            <Input placeholder="you@example.com" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={testingEmail}>
            Send test email
          </Button>
        </Form>
      </Modal>
    </Layout>
  )
}

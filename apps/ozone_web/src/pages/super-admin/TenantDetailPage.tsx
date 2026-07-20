import { useCallback, useEffect, useState } from 'react'
import { Link, Navigate, useNavigate, useParams } from 'react-router-dom'
import {
  Alert,
  Button,
  Descriptions,
  Form,
  Input,
  InputNumber,
  Layout,
  Modal,
  Select,
  Space,
  Table,
  Tag,
  Typography,
  message,
  theme,
} from 'antd'
import { clearPlatformToken, getPlatformToken, platformAuthHeaders } from '../../auth/platformToken'
import { setTenantToken } from '../../auth/tenantToken'

const { Header, Content } = Layout

type CredentialRow = {
  id: string
  role: string
  host: string
  port: number
  username: string
  passwordSet: boolean
  isActive: boolean
  sslMode?: string | null
  rotatedAt?: string | null
}

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
  credentials: CredentialRow[]
}

type AuditRow = {
  id: string
  platformUserId: string
  platformUsername?: string | null
  reason: string
  createdAt: string
  expiresAt: string
  ipAddress?: string | null
}

type CredForm = {
  host: string
  port: number
  databaseName: string
  username: string
  password?: string
  sslMode?: string
}

export function SuperAdminTenantDetailPage() {
  const { companyId } = useParams<{ companyId: string }>()
  const navigate = useNavigate()
  const token = getPlatformToken()
  const [detail, setDetail] = useState<TenantDetail | null>(null)
  const [audits, setAudits] = useState<AuditRow[]>([])
  const [error, setError] = useState<string>()
  const [busy, setBusy] = useState(false)
  const [accessMsg, setAccessMsg] = useState<string>()
  const [impersonateOpen, setImpersonateOpen] = useState(false)
  const [impersonating, setImpersonating] = useState(false)
  const [credOpen, setCredOpen] = useState(false)
  const [credRole, setCredRole] = useState<'Write' | 'Read'>('Write')
  const [savingCred, setSavingCred] = useState(false)
  const [resetOpen, setResetOpen] = useState(false)
  const [resetting, setResetting] = useState(false)
  const [form] = Form.useForm<{ reason: string }>()
  const [credForm] = Form.useForm<CredForm>()
  const [resetForm] = Form.useForm<{ newPassword: string }>()
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken()

  const loadAudits = useCallback(async () => {
    if (!companyId) return
    const res = await fetch(`/v1/platform/tenants/${companyId}/impersonation-audits`, {
      headers: platformAuthHeaders(),
    })
    if (res.ok) {
      setAudits((await res.json()) as AuditRow[])
    }
  }, [companyId])

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
    await loadAudits()
  }, [companyId, navigate, loadAudits])

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

  async function onImpersonate(values: { reason: string }) {
    if (!companyId) return
    setImpersonating(true)
    try {
      const res = await fetch(`/v1/platform/tenants/${companyId}/impersonate`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...platformAuthHeaders(),
        },
        body: JSON.stringify({ reason: values.reason }),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string } | null
        throw new Error(body?.error ?? `Impersonate failed (${res.status})`)
      }
      const body = (await res.json()) as { accessToken: string }
      setTenantToken(body.accessToken)
      message.success('Opening tenant session')
      setImpersonateOpen(false)
      form.resetFields()
      navigate('/', { replace: true })
    } catch (e) {
      message.error(e instanceof Error ? e.message : 'Impersonate failed')
    } finally {
      setImpersonating(false)
    }
  }

  function openCredModal(role: 'Write' | 'Read', existing?: CredentialRow) {
    if (!detail) return
    setCredRole(role)
    credForm.setFieldsValue({
      host: existing?.host ?? '',
      port: existing?.port ?? 5432,
      databaseName: detail.databaseName,
      username: existing?.username ?? '',
      password: undefined,
      sslMode: existing?.sslMode ?? undefined,
    })
    setCredOpen(true)
  }

  async function onSaveCred(values: CredForm) {
    if (!companyId) return
    setSavingCred(true)
    try {
      const res = await fetch(`/catalog/companies/${companyId}/db-credentials/${credRole}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          ...platformAuthHeaders(),
        },
        body: JSON.stringify({
          host: values.host,
          port: values.port,
          databaseName: values.databaseName,
          username: values.username,
          password: values.password || null,
          sslMode: values.sslMode || null,
        }),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string } | null
        throw new Error(body?.error ?? 'Credential update failed')
      }
      message.success(`${credRole} credentials saved`)
      setCredOpen(false)
      await load()
    } catch (e) {
      message.error(e instanceof Error ? e.message : 'Credential update failed')
    } finally {
      setSavingCred(false)
    }
  }

  async function onResetAdmin(values: { newPassword: string }) {
    if (!companyId) return
    setResetting(true)
    try {
      const res = await fetch(`/v1/platform/tenants/${companyId}/reset-admin-password`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...platformAuthHeaders(),
        },
        body: JSON.stringify({ newPassword: values.newPassword }),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string } | null
        throw new Error(body?.error ?? 'Reset failed')
      }
      message.success('Admin password updated')
      setResetOpen(false)
      resetForm.resetFields()
    } catch (e) {
      message.error(e instanceof Error ? e.message : 'Reset failed')
    } finally {
      setResetting(false)
    }
  }

  const statusColor =
    detail?.status === 'Active'
      ? 'success'
      : detail?.status === 'Suspended'
        ? 'warning'
        : 'default'

  const writeCred = detail?.credentials.find((c) => c.role === 'Write')
  const readCred = detail?.credentials.find((c) => c.role === 'Read')

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
                <Space wrap>
                  {detail.status === 'Active' ? (
                    <>
                      <Button type="primary" onClick={() => setImpersonateOpen(true)}>
                        Impersonate
                      </Button>
                      <Button danger loading={busy} onClick={() => void setStatus('suspend')}>
                        Suspend
                      </Button>
                    </>
                  ) : null}
                  {detail.status === 'Suspended' ? (
                    <Button type="primary" loading={busy} onClick={() => void setStatus('activate')}>
                      Activate
                    </Button>
                  ) : null}
                  <Button onClick={() => setResetOpen(true)}>Reset admin password</Button>
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
                ]}
              />

              <div
                style={{
                  marginTop: 32,
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                }}
              >
                <Typography.Title level={5} style={{ margin: 0 }}>
                  Database credentials
                </Typography.Title>
                <Space>
                  <Button size="small" onClick={() => openCredModal('Write', writeCred)}>
                    {writeCred ? 'Edit Write' : 'Add Write'}
                  </Button>
                  <Button size="small" onClick={() => openCredModal('Read', readCred)}>
                    {readCred ? 'Edit Read' : 'Add Read'}
                  </Button>
                </Space>
              </div>
              <Typography.Paragraph type="secondary" style={{ marginTop: 8 }}>
                Separate fields only (host, port, database, username, password, SSL). Never a blob
                connection string — supports external Postgres hosts.
              </Typography.Paragraph>
              <Table
                rowKey="id"
                size="small"
                pagination={false}
                dataSource={detail.credentials}
                columns={[
                  { title: 'Role', dataIndex: 'role' },
                  { title: 'Host', dataIndex: 'host' },
                  { title: 'Port', dataIndex: 'port' },
                  {
                    title: 'Database',
                    key: 'db',
                    render: () => detail.databaseName,
                  },
                  { title: 'Username', dataIndex: 'username' },
                  {
                    title: 'Password',
                    dataIndex: 'passwordSet',
                    render: (v: boolean) => (v ? 'Set' : '—'),
                  },
                  {
                    title: 'SSL',
                    dataIndex: 'sslMode',
                    render: (v: string | null | undefined) => v ?? '—',
                  },
                  {
                    title: 'Rotated',
                    dataIndex: 'rotatedAt',
                    render: (v: string | null | undefined) =>
                      v ? new Date(v).toLocaleString() : '—',
                  },
                ]}
              />

              <Typography.Title level={5} style={{ marginTop: 32 }}>
                Impersonation audit
              </Typography.Title>
              <Table
                rowKey="id"
                size="small"
                pagination={false}
                dataSource={audits}
                columns={[
                  {
                    title: 'When',
                    dataIndex: 'createdAt',
                    render: (v: string) => new Date(v).toLocaleString(),
                  },
                  {
                    title: 'By',
                    dataIndex: 'platformUsername',
                    render: (v: string | null | undefined) => v ?? '—',
                  },
                  { title: 'Reason', dataIndex: 'reason' },
                  {
                    title: 'Expires',
                    dataIndex: 'expiresAt',
                    render: (v: string) => new Date(v).toLocaleString(),
                  },
                  {
                    title: 'IP',
                    dataIndex: 'ipAddress',
                    render: (v: string | null | undefined) => v ?? '—',
                  },
                ]}
              />
            </>
          ) : null}
        </div>
      </Content>

      <Modal
        title="Impersonate tenant"
        open={impersonateOpen}
        onCancel={() => setImpersonateOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Typography.Paragraph type="secondary">
          Opens a short-lived tenant Admin session. Your action is audited.
        </Typography.Paragraph>
        <Form form={form} layout="vertical" onFinish={onImpersonate}>
          <Form.Item
            label="Reason"
            name="reason"
            rules={[
              { required: true, message: 'Enter a reason' },
              { min: 3, message: 'At least 3 characters' },
            ]}
          >
            <Input.TextArea rows={3} placeholder="Support ticket #…" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={impersonating}>
            Start impersonation
          </Button>
        </Form>
      </Modal>

      <Modal
        title={`${credRole} database credentials`}
        open={credOpen}
        onCancel={() => setCredOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Form form={credForm} layout="vertical" onFinish={onSaveCred}>
          <Form.Item label="Host" name="host" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item label="Port" name="port" rules={[{ required: true }]}>
            <InputNumber min={1} max={65535} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item label="Database name" name="databaseName" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item label="Username" name="username" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item
            label="Password"
            name="password"
            extra={writeCred || readCred ? 'Leave blank to keep existing password' : undefined}
            rules={
              detail?.credentials.some((c) => c.role === credRole)
                ? []
                : [{ required: true, message: 'Password required for new credentials' }]
            }
          >
            <Input.Password />
          </Form.Item>
          <Form.Item label="SSL mode" name="sslMode">
            <Select
              allowClear
              options={[
                { value: 'Disable', label: 'Disable' },
                { value: 'Prefer', label: 'Prefer' },
                { value: 'Require', label: 'Require' },
              ]}
            />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={savingCred}>
            Save credentials
          </Button>
        </Form>
      </Modal>

      <Modal
        title="Reset tenant admin password"
        open={resetOpen}
        onCancel={() => setResetOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Typography.Paragraph type="secondary">
          Force-sets the first active Admin user password (no email). For self-service, use
          forgot-password.
        </Typography.Paragraph>
        <Form form={resetForm} layout="vertical" onFinish={onResetAdmin}>
          <Form.Item
            label="New password"
            name="newPassword"
            rules={[{ required: true, min: 8 }]}
          >
            <Input.Password />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={resetting} danger>
            Reset password
          </Button>
        </Form>
      </Modal>
    </Layout>
  )
}

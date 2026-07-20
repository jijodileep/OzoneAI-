import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { Alert, Button, Card, Form, Input, Typography } from 'antd'
import { getPlatformToken, setPlatformToken } from '../../auth/platformToken'

type LoginResponse = {
  accessToken: string
  expiresInSeconds: number
  user: { id: string; username: string; displayName: string }
}

export function SuperAdminLoginPage() {
  const navigate = useNavigate()
  const [error, setError] = useState<string>()
  const [loading, setLoading] = useState(false)

  if (getPlatformToken()) {
    return <Navigate to="/super-admin" replace />
  }

  async function onFinish(values: { username: string; password: string }) {
    setLoading(true)
    setError(undefined)
    try {
      const res = await fetch('/v1/platform/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(values),
      })
      if (!res.ok) {
        setError('Invalid username or password.')
        return
      }
      const data = (await res.json()) as LoginResponse
      setPlatformToken(data.accessToken)
      navigate('/super-admin', { replace: true })
    } catch {
      setError('Could not reach API. Is the server running on :5080?')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="super-admin-login">
      <Card className="super-admin-login-card">
        <Typography.Title level={2} className="super-admin-brand">
          OzoneAI
        </Typography.Title>
        <Typography.Paragraph type="secondary">
          Super Admin sign-in (platform catalog)
        </Typography.Paragraph>
        {error ? <Alert type="error" message={error} showIcon style={{ marginBottom: 16 }} /> : null}
        <Form layout="vertical" onFinish={onFinish} requiredMark={false}>
          <Form.Item
            label="Username"
            name="username"
            rules={[{ required: true, message: 'Enter username' }]}
            initialValue="superadmin"
          >
            <Input autoComplete="username" size="large" />
          </Form.Item>
          <Form.Item
            label="Password"
            name="password"
            rules={[{ required: true, message: 'Enter password' }]}
          >
            <Input.Password autoComplete="current-password" size="large" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block size="large" loading={loading}>
            Sign in
          </Button>
          <div style={{ marginTop: 12, textAlign: 'center' }}>
            <Link to="/super-admin/forgot-password">Forgot password?</Link>
          </div>
        </Form>
      </Card>
    </div>
  )
}

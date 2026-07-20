import { useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { Alert, Button, Card, Form, Input, Typography } from 'antd'

export function ResetPasswordPage() {
  const [params] = useSearchParams()
  const companyKey = params.get('companyKey') ?? ''
  const token = params.get('token') ?? ''
  const [done, setDone] = useState(false)
  const [error, setError] = useState<string>()
  const [loading, setLoading] = useState(false)

  async function onFinish(values: { newPassword: string }) {
    setLoading(true)
    setError(undefined)
    try {
      const res = await fetch('/v1/auth/reset-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          companyKey,
          token,
          newPassword: values.newPassword,
        }),
      })
      if (!res.ok) {
        const body = (await res.json().catch(() => null)) as { error?: string } | null
        setError(body?.error ?? 'Reset failed.')
        return
      }
      setDone(true)
    } catch {
      setError('Could not reach API.')
    } finally {
      setLoading(false)
    }
  }

  if (!companyKey || !token) {
    return (
      <div className="super-admin-login">
        <Card className="super-admin-login-card">
          <Alert type="error" message="Missing company key or token in the reset link." showIcon />
          <div style={{ marginTop: 16 }}>
            <Link to="/forgot-password">Request a new link</Link>
          </div>
        </Card>
      </div>
    )
  }

  return (
    <div className="super-admin-login">
      <Card className="super-admin-login-card">
        <Typography.Title level={2} className="super-admin-brand">
          OzoneAI
        </Typography.Title>
        <Typography.Paragraph type="secondary">
          Set a new password for {companyKey}
        </Typography.Paragraph>
        {error ? <Alert type="error" message={error} showIcon style={{ marginBottom: 16 }} /> : null}
        {done ? (
          <>
            <Alert type="success" showIcon message="Password updated." />
            <div style={{ marginTop: 16 }}>
              <Link to="/login">Sign in</Link>
            </div>
          </>
        ) : (
          <Form layout="vertical" onFinish={onFinish} requiredMark={false}>
            <Form.Item
              label="New password"
              name="newPassword"
              rules={[{ required: true, min: 8 }]}
            >
              <Input.Password size="large" autoComplete="new-password" />
            </Form.Item>
            <Button type="primary" htmlType="submit" block size="large" loading={loading}>
              Update password
            </Button>
          </Form>
        )}
      </Card>
    </div>
  )
}

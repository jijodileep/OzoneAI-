import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Alert, Button, Card, Form, Input, Typography } from 'antd'

export function ForgotPasswordPage() {
  const [done, setDone] = useState(false)
  const [error, setError] = useState<string>()
  const [loading, setLoading] = useState(false)

  async function onFinish(values: { companyKey: string; usernameOrEmail: string }) {
    setLoading(true)
    setError(undefined)
    try {
      const res = await fetch('/v1/auth/forgot-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(values),
      })
      if (!res.ok) {
        setError('Could not submit request.')
        return
      }
      setDone(true)
    } catch {
      setError('Could not reach API.')
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
        <Typography.Paragraph type="secondary">Forgot password</Typography.Paragraph>
        {error ? <Alert type="error" message={error} showIcon style={{ marginBottom: 16 }} /> : null}
        {done ? (
          <Alert
            type="success"
            showIcon
            message="If an account with that username or email exists, a reset link has been sent."
          />
        ) : (
          <Form layout="vertical" onFinish={onFinish} requiredMark={false}>
            <Form.Item
              label="Company key"
              name="companyKey"
              rules={[{ required: true }]}
              initialValue="demo"
            >
              <Input size="large" />
            </Form.Item>
            <Form.Item
              label="Username or email"
              name="usernameOrEmail"
              rules={[{ required: true }]}
            >
              <Input size="large" />
            </Form.Item>
            <Button type="primary" htmlType="submit" block size="large" loading={loading}>
              Send reset link
            </Button>
          </Form>
        )}
        <div style={{ marginTop: 16 }}>
          <Link to="/login">Back to sign-in</Link>
        </div>
      </Card>
    </div>
  )
}

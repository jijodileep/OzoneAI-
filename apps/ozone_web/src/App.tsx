import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { TenantHomePage } from './pages/TenantHomePage'
import { TenantLoginPage } from './pages/TenantLoginPage'
import { ForgotPasswordPage } from './pages/ForgotPasswordPage'
import { ResetPasswordPage } from './pages/ResetPasswordPage'
import { SuperAdminLoginPage } from './pages/super-admin/LoginPage'
import { SuperAdminForgotPasswordPage } from './pages/super-admin/ForgotPasswordPage'
import { SuperAdminResetPasswordPage } from './pages/super-admin/ResetPasswordPage'
import { SuperAdminDashboardPage } from './pages/super-admin/DashboardPage'
import { SuperAdminTenantDetailPage } from './pages/super-admin/TenantDetailPage'
import './App.css'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<TenantLoginPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />
        <Route path="/" element={<TenantHomePage />} />
        <Route path="/super-admin/login" element={<SuperAdminLoginPage />} />
        <Route path="/super-admin/forgot-password" element={<SuperAdminForgotPasswordPage />} />
        <Route path="/super-admin/reset-password" element={<SuperAdminResetPasswordPage />} />
        <Route path="/super-admin" element={<SuperAdminDashboardPage />} />
        <Route path="/super-admin/tenants/:companyId" element={<SuperAdminTenantDetailPage />} />
        <Route path="/super-admin/*" element={<Navigate to="/super-admin" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { TenantHomePage } from './pages/TenantHomePage'
import { TenantLoginPage } from './pages/TenantLoginPage'
import { SuperAdminLoginPage } from './pages/super-admin/LoginPage'
import { SuperAdminDashboardPage } from './pages/super-admin/DashboardPage'
import { SuperAdminTenantDetailPage } from './pages/super-admin/TenantDetailPage'
import './App.css'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<TenantLoginPage />} />
        <Route path="/" element={<TenantHomePage />} />
        <Route path="/super-admin/login" element={<SuperAdminLoginPage />} />
        <Route path="/super-admin" element={<SuperAdminDashboardPage />} />
        <Route path="/super-admin/tenants/:companyId" element={<SuperAdminTenantDetailPage />} />
        <Route path="/super-admin/*" element={<Navigate to="/super-admin" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

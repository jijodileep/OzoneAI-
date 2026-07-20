# OzoneAI Web (admin ERP)

React + TypeScript (Vite) + Ant Design + AG Grid (later).

## Dev

```bash
npm install
npm run dev
```

App: http://localhost:5173  
API proxy: `/v1`, `/catalog`, `/health` → http://localhost:5080

## Build

```bash
npm run build
```

## Notes

- Shell layout: sider + header with `FinancialYearSwitcher`.
- Super Admin: `/super-admin/login` (seed `superadmin` / `ChangeMe!123` unless overridden in API config).
- Create tenant: Super Admin → **Create tenant** → `POST /v1/platform/tenants` provisions `ozone_t_{key}` + admin user.
- Tenant login: `/login` with company key + user (seed `demo` / `admin` / `ChangeMe!123`).
- Impersonate: Super Admin → tenant detail → **Impersonate** (audited; banner on tenant shell).

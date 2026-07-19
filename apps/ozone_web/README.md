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
- Tenant company-key login: E3.1.

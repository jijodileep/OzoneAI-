# Super Admin Dashboard

## 1. Identity
- Module / area: Dashboards
- Page type: Dashboard
- Primary users / roles: Super Admin
- Entry points: Login redirect for Super Admin
- Priority for rebuild: P0

## 2. Purpose
Provide Super Admin with company-level overview: sales/purchase KPIs plus list of companies in the deployment.

## 3. Preconditions
- Authenticated as Super Admin
- Access to registry company list + tenant aggregates as designed

## 4. Layout and UI
- Same KPI/chart pattern as Admin dashboard
- Companies list widget (multi-tenant registry)
- Quick menu

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Companies | companies | list | — | all | — | company registry | Super Admin | Tenant list |
| Sales/Purchase KPIs | (same as admin) | KPI/chart | — | period | — | transactions | always | |

## 6. Actions and flows
- View company list; optionally open company settings (new system)
- Drill into KPIs

## 7. Business rules
- Super Admin bypasses granular menu restrictions (full menus)
- Company list from master registry, not tenant DB alone

## 8. Data requirements
- Company registry entity; aggregate metrics per company if multi-tenant SaaS console is desired

## 9. Integrations
- None

## 10. Permissions
- Super Admin only for company-wide widgets

## 11. Reports / exports
- Optional company summary export

## 12. Edge cases
- No companies configured
- Tenant DB offline for one company

## 13. Acceptance criteria
- Super Admin lands here after login
- Sees company list and core KPIs
- Cannot access without Super Admin role

## 14. Legacy reference
- Old URL: `dashboard/superadmin`
- Controller: `Dashboard.php::superadmin`
- View: `dashboard/main`
- Open questions: whether Super Admin operates only on master DB or can impersonate tenants

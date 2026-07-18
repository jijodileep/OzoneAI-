# Admin Dashboard

## 1. Identity
- Module / area: Dashboards
- Page type: Dashboard
- Primary users / roles: Admin
- Entry points: Post-login redirect for Admin; menu "Admin Dashboard" / DashBoards
- Priority for rebuild: P0

## 2. Purpose
Give administrators an at-a-glance view of today’s sales, purchases, estimates, returns, and quick navigation into frequent tasks.

## 3. Preconditions
- Authenticated Admin (or equivalent) session
- Tenant DB connected
- Optional: financial year context

## 4. Layout and UI
- KPI cards: cash/credit/bank sales, estimates, sale returns, purchases, purchase orders/returns
- Charts: sales vs purchase trend series
- Quick menu shortcuts
- Recent transactions widgets (DataTables AJAX)
- Optional: receivable lists, margin widgets, stock value

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Cash sales | cashsales | KPI | — | today | — | sales | always | Count/value |
| Credit sales | creditsales | KPI | — | today | — | sales | always | |
| Bank sales | banksales | KPI | — | today | — | sales | always | |
| Estimates | estimates | KPI | — | today | — | sales type estimate | always | |
| Sale returns | salereturn | KPI | — | today | — | sales_return | always | |
| Purchases | purchasereport | KPI | — | today | — | purchase | always | |
| Purchase orders | purchaseorder | KPI | — | today | — | order | always | |
| Quick menu | quickmenu | links | — | user prefs | — | menus | always | |

## 6. Actions and flows
- Click KPI → drill to related register/report (new system should deep-link)
- Refresh widgets via AJAX
- Open calculator helper
- Navigate via quick menu

## 7. Business rules
- Default filter: current day (Admin variant)
- Numbers use company round-off / decimal settings
- Do **not** run full DB backup on every dashboard load in new system (legacy side-effect); use scheduled jobs

## 8. Data requirements
- Entities: Sale, Purchase, Voucher aggregates; QuickMenu preference
- Read-only aggregates; no posting from dashboard

## 9. Integrations
- None required (legacy may trigger DB export — remove)

## 10. Permissions
- Admin / Super Admin full; others only if menu granted

## 11. Reports / exports
- Optional export of widget data; not required for MVP

## 12. Edge cases
- Empty day (zeros)
- Slow aggregate queries — cache/async for new tech

## 13. Acceptance criteria
- Admin sees today’s KPIs after login
- Widgets load without blocking navigation
- No destructive side effects on page load

## 14. Legacy reference
- Old URL: `dashboard/admin`, menu may use `dashboard/summary`
- Controller: `Dashboard.php` — `admin`, AJAX helpers
- Views: `dashboard/main`, related partials
- Models: `Dashboard_model`
- Open questions: which KPI set is product-standard vs tenant-custom

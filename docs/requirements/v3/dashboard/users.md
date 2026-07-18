# Users Dashboard

## 1. Identity
- Module / area: Dashboards
- Page type: Dashboard
- Primary users / roles: General users (non-Admin)
- Entry points: Login redirect for non-admin/non-clinic roles
- Priority for rebuild: P0

## 2. Purpose
Show a lighter operational dashboard (cash/credit counts, estimates, purchases) and quick menu limited to granted menus.

## 3. Preconditions
- Authenticated user with menus assigned via role/permissions

## 4. Layout and UI
- Reduced KPI set vs Admin
- Quick menu from user-granted menus
- View variant legacy: `dashboard/sadmin`

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Cash/credit counts | cashsales, creditsales | KPI | — | today | — | sales | always | |
| Estimates / returns / purchase | estimates, salereturn, purchasereport | KPI | — | today | — | transactions | always | |
| Quick menu | quickmenu | links | — | granted menus | — | menu_to_user | always | |

## 6. Actions and flows
- Navigate via quick menu only to permitted screens

## 7. Business rules
- Menus filtered by `menu_to_user` (or role grants in new system)
- Write actions elsewhere still check add/edit/delete/cancel flags

## 8. Data requirements
- Same aggregate entities as Admin; permission-scoped menu list

## 9. Integrations
- None

## 10. Permissions
- View dashboard if menu granted; widgets may be further scoped later

## 11. Reports / exports
- None required

## 12. Edge cases
- User with no menus — empty quick menu, still see landing page

## 13. Acceptance criteria
- Non-admin login lands here
- Quick menu only shows permitted items

## 14. Legacy reference
- Old URL: `dashboard/users`
- Controller: `Dashboard.php::users`
- View: `dashboard/sadmin`

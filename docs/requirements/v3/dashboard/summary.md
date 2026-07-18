# Admin Summary Dashboard

## 1. Identity
- Module / area: Dashboards
- Page type: Dashboard
- Primary users / roles: Admin
- Entry points: Menu "Admin Dashboard" → `dashboard/summary`
- Priority for rebuild: P0

## 2. Purpose
Full KPI dashboard: sales/purchase by cash/credit/bank, estimates, returns, payments and collections.

## 3. Preconditions
- Authenticated Admin; tenant connected

## 4. Layout and UI
- Dense KPI cards for sales, purchase, payments, collections
- Charts and recent lists via AJAX
- Legacy view: `dashboard/adminlte`

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Cash/Credit/Bank sales values | cashsales, creditsales, banksales | KPI | — | period | — | sales | always | Count + value |
| Estimates | estimates | KPI | — | period | — | estimate | always | |
| Sale/Purchase returns | salereturn, purchasereturn | KPI | — | period | — | returns | always | |
| Cash/Bank payment | cashpayment, bankpayment | KPI | — | period | — | vouchers | always | |
| Cash/Bank collection | cashcollection, bankcollection | KPI | — | period | — | receipts | always | |

## 6. Actions and flows
- Date/period filter (new system should make period explicit)
- Drill-down to registers

## 7. Business rules
- Aggregate from posted (published) transactions only
- Round using company decimal settings

## 8. Data requirements
- Sale, Purchase, Voucher aggregates by payment mode

## 9. Integrations
- None (avoid legacy on-load DB dump)

## 10. Permissions
- Admin / granted menu

## 11. Reports / exports
- Optional CSV of KPIs

## 12. Edge cases
- Large date ranges — performance

## 13. Acceptance criteria
- All KPI categories render with correct totals for selected period
- Drill links open matching filtered registers

## 14. Legacy reference
- Old URL: `dashboard/summary`
- Controller: `Dashboard.php::summary`
- View: `dashboard/adminfix`

# API: Sales Create (Mobile / Device)

## 1. Identity
- Module / area: API / Sales
- Page type: API
- Primary users / roles: Mobile sales apps
- Entry points: `restapi/Sales/create/{comid}`, `api/Sales/create/{comid}`
- Priority for rebuild: P1

## 2. Purpose
Bulk create/update sales invoices and lines from devices, including discounts/margins/cess and estimate/order linkage; return updated ledgers/customer points.

## 3. Preconditions
- Authenticated device/user; valid comid; items/customers exist or upsert rules defined

## 4. Layout and UI
- N/A — JSON API

## 5. Fields (detailed)
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| comid | path | yes | Tenant |
| sales[] | array | yes | Bulk payload |
| sales[].invoice / date / customer_id / totals | mixed | yes | Align with Sale entity |
| sales[].items[] | array | yes | item_id, qty, rate, tax, godown, discounts, cess |
| sales[].cash_type / amounts | mixed | yes | Posting mode |
| estimate/order links | ids | no | Conversion linkage |

Response: validity, saved ids, ledgers/points updates, errors per row.

## 6. Actions and flows
- POST bulk sales → validate → upsert header/lines → stock/ledger posting (same rules as web Sales) → response

## 7. Business rules
- Same domain rules as [transaction/sales.md](../transaction/sales.md)
- Idempotency key per device bill recommended
- Partial success reporting for bulk

## 8. Data requirements
- Sale, SaleLine, ItemGodown, AccLedgerTrans, AccAgeing, CustomerPoints

## 9. Integrations
- Optional FCM after save

## 10. Permissions
- Token with `sales:write` scope for tenant

## 11. Reports / exports
- N/A

## 12. Edge cases
- Duplicate device invoice; insufficient stock; invalid tax

## 13. Acceptance criteria
- Posted mobile sale appears in web Sales Register with correct stock/ledger
- Reject unauthorized comid access
- Bulk returns per-record errors without failing entire batch silently

## 14. Legacy reference
- `restapi/Sales.php`, `api/Sales.php` — consolidate
- Ecom variant `restapi/ecom/Sales.php` is single-tenant — see ecommerce API

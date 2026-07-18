# API: Replacement Voucher

## 1. Identity
- Module / area: API / Vouchers
- Page type: API
- Primary users / roles: Mobile apps
- Entry points: `restapi/Replacement/save_replacement/{comid}`
- Priority for rebuild: P2

## 2. Purpose
Save product replacement/exchange vouchers from mobile with explicit validation and HTTP status codes.

## 3. Preconditions
- Auth; customer; items

## 4. Layout and UI
- N/A

## 5. Fields (detailed)
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| comid | path | yes | |
| customer_id | id | yes | Validated present |
| id | id | yes for update | |
| replacement lines | array | yes | items exchanged |

## 6. Actions and flows
- POST JSON or form → validate → `Replace_items_model::save_mobile_data` → 200/400/500

## 7. Business rules
- Align stock effects with web Replacement voucher screen
- Prefer JSON-only in new API

## 8. Data requirements
- ReplacementVoucher, lines, ItemGodown, Customer

## 9. Integrations
- None required

## 10. Permissions
- `replacement:write`

## 11. Reports / exports
- N/A

## 12. Edge cases
- Missing customer_id → 400

## 13. Acceptance criteria
- Valid payload saves and visible in web replacement list
- Invalid payload returns 400 with message

## 14. Legacy reference
- `restapi/Replacement.php`

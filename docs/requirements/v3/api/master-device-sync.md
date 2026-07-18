# API: Master / Device Sync

## 1. Identity
- Module / area: API / Mobile sync
- Page type: API (resource group)
- Primary users / roles: Mobile apps / field devices
- Entry points: `restapi/Master`, `api/Master`, `api_version2/MasterApi` (consolidate to one in new tech)
- Priority for rebuild: P1

## 2. Purpose
Bootstrap and sync tenant masters and operational data to offline-capable devices: login, device registration, items/customers/stock, ledgers, ageing, config, location/tracking.

## 3. Preconditions
- Valid `comid` resolving to tenant DB
- **New system must require authentication** (legacy REST auth disabled — do not repeat)

## 4. Layout and UI
- N/A (JSON API). Provide OpenAPI spec in new stack.

## 5. Fields (detailed)
Request/response contracts (logical):

| Endpoint (logical) | Method | Key request fields | Key response | Notes |
|--------------------|--------|--------------------|--------------|-------|
| login | POST | comid, username, password | user, session/token, device binding | Issue JWT/refresh in new system |
| devicereg / updatedevice | POST | comid, device id, meta | device record | |
| devicedetails | POST | comid, device | full bootstrap masters payload | Heavy; paginate in new system |
| config | POST | comid | company settings, bill types | |
| customerreg / customer_info | POST | customer payload | saved customer | |
| getitemstock / item_info | POST | filters | stock/item data | |
| ledger_balance / ledgerstatement | POST | ledger id, dates | balances/movements | |
| get_aeging | POST | customer | open invoices | |
| billtype | POST | — | bill type series | |
| itemtransfer | POST | transfer lines | result | |
| cash_bank_ledgers | POST | — | cash/bank ledgers | |
| createArea | POST | area | saved | |
| user_location / executive_tracking / UserPunching | POST | geo/time | ack | Field force |
| masters (v2) | POST | comid | hsn, category, brand, area, tax, route, discount_group, price_list, items, cess, customers, units | Offline pack |
| CustomerBalance (v2) | POST | comid | balances | |

## 6. Actions and flows
1. App login with comid → token
2. Register device → pull masters pack
3. Periodic delta sync
4. Push customers/transfers/locations

## 7. Business rules
- Multi-tenant via comid → tenant connection
- Idempotent upserts where devices retry
- Prefer delta sync over full dump
- Fix legacy broken auth (`authorization('123456')`)

## 8. Data requirements
- Device, User, Item, Customer, Ledger, Ageing, Area, BillType, LocationPing, SyncCursor

## 9. Integrations
- Mobile apps; optional push

## 10. Permissions
- Device-scoped API keys or user tokens with scopes
- Never leave open CORS + no auth

## 11. Reports / exports
- Sync audit logs

## 12. Edge cases
- Offline conflict; partial sync; revoked device

## 13. Acceptance criteria
- Authenticated device can pull masters for its tenant only
- Stock/balance endpoints match web UI numbers
- Unauthorized calls rejected
- Duplicate trees (`api/` vs `restapi/`) consolidated to one versioned API

## 14. Legacy reference
- `restapi/Master.php`, `api/Master.php`, `restapi/MasterApi.php`, `api_version2/MasterApi.php`
- Open questions: which production clients call which tree

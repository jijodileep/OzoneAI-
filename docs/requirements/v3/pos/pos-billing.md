# POS Billing

## 1. Identity
- Module / area: POS
- Page type: Create | List
- Primary users / roles: Cashiers, Admin
- Entry points: Menu "POS" → `pos/Main/listall`; `pos_register` → bill type → `create_pos`
- Priority for rebuild: P1

## 2. Purpose
Fast retail billing: select bill type/counter, scan/search items, tender payment, save as sales document with stock/ledger effects.

## 3. Preconditions
- Bill types for POS; items with barcodes; optional counters; cash/bank ledgers

## 4. Layout and UI
- Billboard-style entry: large item search, cart grid, totals, tender panel
- Customer quick select / cash customer
- Pending bills list to resume
- Counter open/close screens

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Bill type | bill_type | select | yes | from register | — | set_billtype | start | |
| Counter | counter_id | select | conditional | open counter | — | counter | counters on | |
| Customer | customer_id | lookup | no | cash | — | customers | always | |
| Item search / barcode | search | text | yes | — | found item | item | always | |
| Qty / Rate / Tax / Disc | line fields | grid | yes | item defaults | qty>0 | — | always | |
| Tender / cash type | cash_type, amounts | select/number | yes | Cash | — | — | pay | |
| Total | total | calc | — | — | — | — | always | |

## 6. Actions and flows
1. Open POS register → choose bill type
2. Add lines via barcode/search
3. Save → same posting rules as Sales (stock out, ledger)
4. Print receipt; optionally park pending bill
5. Counter open: opening float; close: reconciliation

## 7. Business rules
- Persists through `sales` / `sales_items` (POS channel flag recommended in new system)
- Stock/ledger identical to sales cash/credit rules
- Counter_trans audits till movements

## 8. Data requirements
- Counter, CounterTrans, Sale, SaleLine, BillType, Item

## 9. Integrations
- Receipt printer; barcode scanner; optional cash drawer

## 10. Permissions
- POS menu; separate void/cancel privileges

## 11. Reports / exports
- POS sales list; day/branch POS report

## 12. Edge cases
- Offline park (future); price override permission; negative stock

## 13. Acceptance criteria
- Cashier can complete a cash sale in under typical retail steps
- Stock decreases; cash ledger posts
- Pending bill can be resumed and completed
- Counter close stores reconciliation

## 14. Legacy reference
- Controller: `pos/Main.php`, `Pos_model`
- URLs: `pos/Main/listall`, `create_pos`, `list_counter`, `pendinglist`

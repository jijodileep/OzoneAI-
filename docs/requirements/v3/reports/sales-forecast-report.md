# Sales Forecast Report

## 1. Identity
- Module / area: reports
- Page type: Report
- Primary users / roles: Authenticated tenant users with menu grant; Admin/Super Admin unrestricted for administration screens
- Entry points: Menu **Sales Forecast Report**; deep links from related registers/dashboards where applicable
- Priority for rebuild: P1
- Wave: 2
- Menu ID: 457

## 2. Purpose
Read-only analytical/report screen. Provide filters (date range, customer/supplier/item/godown/route as relevant), tabular results, and Excel/PDF export. No stock/ledger posting from this page.

Success criteria: user can open the screen, complete the primary task (view list / save record / run report), and see consistent tenant data with correct permissions.

## 3. Preconditions
- Authenticated session with tenant database context
- Financial year / company settings loaded when required by transactions
- Menu permission for this URL (read at minimum; write for mutations)
- Dependent masters exist (customers/items/ledgers/etc. as required by this screen)

## 4. Layout and UI
- AdminLTE-style authenticated layout with module menu
- Typical sections for this page type:
  - **List/Report:** filter bar, results grid (DataTables), row action buttons, export
  - **Form:** header fields, optional line-item grid, totals, Save/Cancel
  - **Settings:** configuration form or Grocery-CRUD style table editor
- Support desktop; preserve mobile-friendly filters for high-traffic ops screens
- Key components: date pickers, typeahead lookups (customer/item/ledger), select2-style dropdowns, print preview modal where printing exists

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Primary date / period | date_from, date_to or doc_date | date | usually yes for reports/txns | today / month | valid range | — | list/report/txn | Adjust per screen |
| Primary party | customer_id / supplier_id / ledger_id | lookup | when applicable | — | must exist | party masters | txn/CRM/hotel | |
| Name / Title | name | text | yes for masters | — | non-empty; unique if master | — | masters | |
| Code | code | text | often yes | auto | unique | — | masters | |
| Status / Published | published / is_active | flag | no | active | — | — | most entities | Soft delete |
| Amount / Total | amount / total | number | when financial | 0 | >=0 or balanced | calculated | vouchers/txns | |
| Narration / Notes | narration / remarks | textarea | no | — | max length | — | many forms | |
| Line items grid | lines[] | grid | when document | — | at least one line | items/ledgers | sales/purchase/BOM/booking | See linked engine docs |

> New-tech implementers must expand this table from UX review of the live screen; column set above is the minimum shared contract. High-risk screens (Sales, Purchase, Vouchers, Item Master) have dedicated deep-dive files in this folder tree.

## 6. Actions and flows
- **Open / List:** load filtered data for tenant; respect permissions
- **Create:** open blank form → validate → save → toast + redirect/list refresh
- **Edit:** load by id → save → unpost/repost if financial document
- **View:** read-only form
- **Delete / Cancel:** confirm; prefer soft cancel for posted documents; block if dependencies
- **Print / Export:** PDF or Excel using current filters
- **Search / Filter:** apply without full page reload (API-driven grid)

Failure messages: validation errors inline; permission denied; not found; concurrency/conflict.

## 7. Business rules
- Tenant isolation on every query
- Role/menu permissions: read, add, write, delete, cancel as granted
- Financial documents: numbering series, double-entry integrity, stock symmetry (in vs out), edit via reverse+repost or equivalent audit-safe method
- Masters: prevent delete when referenced by transactions
- Reports: posted/active records only unless "include cancelled" filter is provided
- Round amounts using company decimal settings

## 8. Data requirements (for new DB/API design)
- Entities involved: Read models over Sale/Purchase/Item/Voucher/Ledger (no writes)
- Relationships: follow parent/child for documents (header→lines); FKs to Item, Party, Ledger, Godown as applicable
- Fields that must persist: all user-enterable fields + audit (created_by, created_at, updated_by, updated_at) + status
- Soft delete / cancel flag rather than hard delete for transactional data

## 9. Integrations
- PDF/print when the legacy screen offers Print/Download
- Excel export for registers and reports
- Email/SMS/WhatsApp only if this screen triggers them today (PO email, voucher WhatsApp, certificates) — make configurable
- No card payment gateway assumed unless explicitly added later

## 10. Permissions
- Visibility via menu assignment
- Mutations require add/write; cancel/delete require dedicated flags
- Admin / Super Admin: full access
- Export/print can be separate privileges in the new system

## 11. Reports / exports from this page
- Grid Excel/CSV export
- PDF print for documents/certificates/labels when applicable
- Filters on screen carry into export

## 12. Edge cases and errors
- Empty result set
- Invalid date range
- Duplicate codes / document numbers
- Saving with missing required masters
- Permission denied
- Referential delete blocked
- Large report timeouts — require async/export job in new tech if heavy

## 13. Acceptance criteria
- Given a user with menu access, they can open **Sales Forecast Report** and see tenant-scoped data
- Given valid input, Create/Save persists and reappears in list/report
- Given missing permission, mutations are rejected
- Export/print (if offered) reflects current filters
- No cross-tenant data leakage

## 14. Legacy reference (traceability only)
- Old URL: `reports/invoice/sales_forecast`
- Controller / methods: CodeIgniter controller mapped from URL path under `v3/application/controllers/`
- Main views: under `v3/application/views/` matching module path
- Main models / tables: module model + entities listed in §8; schema hints in `testdb.sql`
- Open questions / unclear behavior in old code: confirm field-level validation and any tenant-specific branches (`login_comid`) during implementation UAT

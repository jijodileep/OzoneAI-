# <Page Name>

## 1. Identity
- Module / area:
- Page type: List | Create | Edit | View | Report | Dashboard | Settings | Modal
- Primary users / roles:
- Entry points (menu label, deep links from other pages):
- Priority for rebuild: P0 / P1 / P2

## 2. Purpose
What the user must accomplish. Success criteria in business terms.

## 3. Preconditions
Login, company/tenant, financial year, permissions, master data that must exist first.

## 4. Layout and UI
- Page sections (header, filters, grid, form panels, totals bar, modals)
- Desktop vs mobile notes if the current screen differs
- Key UI components (tables, date pickers, item lookup, print preview)

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|

Include calculated fields and read-only displays.

## 6. Actions and flows
For each button/link:
- Name
- Who can use it
- Steps / confirmation
- Success result (stay, redirect, toast, print)
- Failure messages

Document main flows as short step lists, e.g. Create Sale, Edit Sale, Cancel Sale.

## 7. Business rules
Numbering, tax/GST, discounts, stock impact, ledger/voucher posting, rounding, status workflow, locks after save, edit restrictions, multi-godown, multi-branch, etc.

## 8. Data requirements (for new DB/API design)
- Entities involved (Item, Sale, SaleLine, Ledger, …)
- Relationships
- Fields that must persist
- Soft delete / audit / created-by if present today

## 9. Integrations
Email, SMS, WhatsApp, PDF/print, Excel import/export, push, e-invoice — when this page triggers them.

## 10. Permissions
read / add / edit / delete / cancel / print / export — and role differences.

## 11. Reports / exports from this page
Formats, columns, filters carried into export.

## 12. Edge cases and errors
Empty states, duplicate bill numbers, insufficient stock, permission denied, concurrent edit if any.

## 13. Acceptance criteria
Bullet checklist testable in the new system (Given/When/Then style where useful).

## 14. Legacy reference (traceability only)
- Old URL:
- Controller / methods:
- Main views:
- Main models / tables:
- Open questions / unclear behavior in old code:

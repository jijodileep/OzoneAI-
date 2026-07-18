# Godown (Warehouse)

## 1. Identity
- Module / area: Inventory Management
- Page type: List | Create | Edit
- Primary users / roles: Inventory Admin
- Entry points: Menu "Godown" → `master/godown/godown`
- Priority for rebuild: P0

## 2. Purpose
Define warehouses/locations for stock. Sales/purchase lines and stock reports use these godowns. Supports hierarchical parent godowns.

## 3. Preconditions
- Company exists; multi-godown setting enabled for full use

## 4. Layout and UI
- List: name, address, contact, person; edit/view; delete only if unused
- Form / grocery CRUD fields below

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Name | godown_name | text | yes | — | non-empty | — | always | |
| Code | godown_code | text | no | — | unique | — | always | |
| Address | godown_address | textarea | no | — | — | — | always | |
| Contact No | godown_contactno | text | no | — | numeric | — | always | |
| Contact Person | godown_contact_person | text | no | — | — | — | always | |
| Company | company | select | no | current | — | company | always | |
| Parent | parent | select | no | none | — | ims_godown | hierarchy | Nested godowns |
| Godown type | (godown_type master) | select | no | — | — | godown_type | optional | Separate small master |

## 6. Actions and flows
- CRUD; delete blocked when stock/transactions reference godown
- Used in item opening stock, sales/purchase lines, transfers

## 7. Business rules
- Authoritative table: `ims_godown` (not legacy `godown`)
- Hierarchical parent allowed
- Stock stored in `item_godown` per item+godown(+batch)

## 8. Data requirements
- Godown, GodownType, ItemGodown, Company

## 9. Integrations
- None

## 10. Permissions
- Inventory menu grants

## 11. Reports / exports
- Godown stock register / transfer reports

## 12. Edge cases
- Delete with stock → block
- Transfer between same godown

## 13. Acceptance criteria
- Created godown appears in sales line godown dropdown
- Delete blocked when referenced
- Parent/child hierarchy displayable

## 14. Legacy reference
- URL: `master/godown/godown`
- Controllers: `master/Godown.php` (primary); `settings/Godown.php` legacy — do not port dual tables
- Tables: `ims_godown`, `godown_type`

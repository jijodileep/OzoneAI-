# Item Master

## 1. Identity
- Module / area: Inventory Management
- Page type: List + Create + Edit + View
- Primary users / roles: Inventory Admin, Admin
- Entry points: Menu "Item Master" → `inventory_master/Item/listall`
- Priority for rebuild: P0

## 2. Purpose
Maintain product catalog: identity, pricing, tax, units, opening stock by godown/batch, barcodes, media, bulk rate/tax updates.

## 3. Preconditions
- Category, brand, HSN, tax, units, godown masters as needed
- Company settings: batch, godown, auto-barcode flags

## 4. Layout and UI
- List with search/filters; stock reconcile note on load (legacy)
- Form tabs/sections: general, pricing, tax, units, opening stock, media, barcode
- Tools: Change Item Rate, Change Item Tax, barcode print

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Name / Alt name | name, name_lan | text | yes | — | unique-ish | — | always | Duplicate check AJAX |
| Barcode | barcode | text | conditional | auto | unique | — | always | Autogen if enabled |
| Brand / Category / Group / Type | brand_id, item_category_id, item_group_id, item_type_id | select | no/yes | — | — | masters | always | |
| HSN | hsncode | select | GST | — | — | hsncode | GST | |
| Units | unit_id, sub_unit_* | select | yes | — | — | units | always | Multi-unit conversion |
| Purchase / Sale / Special / MRP / WH rates | prate, srate, special_rate*, MRP, wh_rate | number | yes | 0 | >=0 | — | always | |
| Tax / Cess / Tax ledgers | tax_id, cess_id, tax_*_id | select | yes | — | — | taxgst, cess | always | |
| Min/Max/Reorder | min_level, max_level, reorder_point | number | no | 0 | >=0 | — | always | Alerts |
| Opening stock / rate | open_stock, open_stk_rate | number | no | 0 | — | — | create | Per godown/batch grid |
| Godown/Batch opening rows | gid[], gqty[], batch | grid | conditional | — | — | ims_godown | multi modes | |
| Rack / Part nos / Description | rack, part_no*, description | text | no | — | — | — | always | |
| Online / Active / Warranty | item_online, is_active, war_period | flags | no | — | — | — | settings | |
| Media | images/videos/urls | upload | no | — | file types | gallery | always | |
| Auto rate rules | item_auto_rate | subform | no | — | % markup | — | settings | |

## 6. Actions and flows
- CRUD item
- Bulk update rates by filter (tax/brand/HSN/category) on allow-listed fields
- Bulk change tax
- Generate/print barcodes (configurable label templates)
- View stock/transaction history

## 7. Business rules
- Opening stock creates item_godown / stock ledgers
- Auto barcode when enabled and free
- Multi-unit prices via conversion factors
- Stock modes: single godown vs multi vs batch
- List may reconcile ledger-derived stock (move to job in new system)

## 8. Data requirements
- Item, ItemGodown, ItemLedger, ItemAutoRate, Unit, Category, Brand, Group, Type, HSN, Tax, Cess, Media

## 9. Integrations
- Barcode/label PDF; image CDN/storage

## 10. Permissions
- Menu-level CRUD; protect bulk tools behind Admin

## 11. Reports / exports
- Item list; stock reports; barcode sheets

## 12. Edge cases
- Delete item with transactions (block)
- Duplicate barcode
- Negative opening stock

## 13. Acceptance criteria
- Create item with tax and sale rate; appears in sales item lookup
- Opening stock reflects in stock report
- Bulk rate update only changes selected field
- Barcode uniqueness enforced

## 14. Legacy reference
- URL: `inventory_master/Item/listall`, `create`
- Controller: `inventory_master/Item.php`, `inventory_model/Items_model`
- Tables: `item`, `item_godown`, `item_auto_rate`

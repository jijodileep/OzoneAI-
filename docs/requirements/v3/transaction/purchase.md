# Purchase Bill / Purchase Register

## 1. Identity
- Module / area: Transactions / Purchase
- Page type: List + Create + Edit + View
- Primary users / roles: Purchase staff, Admin
- Entry points: Menu Purchase Register / Purchase Bill / Purchase Order / Purchase Return
- Priority for rebuild: P0

## 2. Purpose
Record purchases that increase stock and post supplier/cash/bank ledgers; manage purchase orders (no stock/ledger until confirmed/received); purchase returns; barcode labels from receipt.

Types: `purchase`, `purchase_return`, `order` (purchase order).

## 3. Preconditions
- Suppliers, items, tax, godown, bill types, ledgers configured
- Permission for purchase / order / purchase_return

## 4. Layout and UI
- List with filters and actions: Edit, View, Print, Barcode, Email (PO), Confirm Order, Cancel/Delete
- Form: supplier header + line grid (qty, free, rate, MRP, tax, godown, batch, mfg/exp) + totals + cash type
- Optional IMEI capture per line
- Barcode label print dialog

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Invoice / Bill No | invoice, bill_no | text | yes | auto | unique series | set_billtype | purchase/return | |
| Date | date | date | yes | today | — | — | always | |
| Supplier | supplier_id | lookup | yes | — | — | suppliers | always | Auto-create ledger |
| Type | type | hidden | yes | purchase | enum | purchase, purchase_return, order | by menu | |
| Bill type | bill_type | select | yes | — | — | set_billtype | always | |
| PO ref / Order link | po_ref_no, order_id | text/ref | no | — | — | purchase orders | purchase | |
| Cash type | cash_type | select | yes | Credit | — | — | purchase/return | |
| Totals | total, tax | calc | — | — | — | lines | always | |
| Rate auto update | rate_auto_update | checkbox | no | off | — | item_auto_rate | settings | Updates item sell rates |
| Line: item, unit, qty, free, rate, mrp, tax, godown, batch, mfg/exp, sale_rate | (purchase_items) | grid | yes | — | qty>0 | item masters | always | |
| IMEI serials | purchase_items_imei | subgrid | conditional | — | unique | — | serialized items | |

## 6. Actions and flows
- Create/Edit purchase → stock in + ledger/ageing (mirror of sales)
- Purchase order → document only; Confirm; Email PDF to supplier
- Purchase return → stock out
- Print bill / delivery challan / barcodes
- Cancel (soft restore) / Delete (hard after restore)

## 7. Business rules
- Stock: purchase `dt` (in), return `cr` (out); order no stock
- Edit = unpost + repost
- Optional auto update of item selling rates from landing cost rules
- Supplier ledger auto-created with ref linkage
- Order status: draft → send → confirmed

## 8. Data requirements
- Purchase, PurchaseLine, PurchaseLedgerLine, PurchaseItemImei, Item, ItemGodown, AccLedgerTrans, AccAgeing, OrderMail, BillType

## 9. Integrations
- Email PO PDF; barcode/label PDF; Excel

## 10. Permissions
- Per type: purchase, order, purchase_return — read/add/write/cancel

## 11. Reports / exports
- Register export; PO PDF; barcode sheets

## 12. Edge cases
- Receiving against PO partial quantities
- Return exceeding purchased qty
- Duplicate supplier invoice numbers
- IMEI already sold

## 13. Acceptance criteria
- Purchase increases stock and posts supplier credit (or cash/bank)
- PO does not change stock/ledger until converted/received
- Return reverses stock
- Email marks PO as sent
- Barcode print available after purchase save

## 14. Legacy reference
- URL: `transaction/purchase/listall/{type}`, `create/{type}`
- Controller: `transaction/Purchase.php`, `purchase_model`
- Tables: `purchase`, `purchase_items`, `purchase_items_imei`, `order_mail`

# Sales Bill / Sales Register

## 1. Identity
- Module / area: Transactions / Sales
- Page type: List + Create + Edit + View
- Primary users / roles: Sales staff, Admin
- Entry points: Menu "Sales Register", "Sales Bill"; convert from estimate/delivery note; duplicate
- Priority for rebuild: P0

## 2. Purpose
Create and manage sales invoices that reduce stock, post customer/cash/bank ledgers, track receivables (ageing), support tax/GST, print bills, and optionally e-way/e-invoice.

Related document types sharing the same engine (separate menu entries may point here with a type):
- `sales` — financial + stock
- `sales_return` — reverse stock + credit note style effects
- `estimate` / Sales Order — document only (no stock/ledger until converted)
- `delivery_note` — stock out; convert to sales

## 3. Preconditions
- Login + tenant
- Masters: customers (or cash customer), items, tax, units, godown (if multi-godown), bill types, ledgers for cash/bank/tax/discount/freight
- User permission for sales type (read/add/write/cancel)

## 4. Layout and UI
### List
- Filters: date range, customer, bill type, status
- Grid: invoice, date, customer, totals, cash type, actions
- Row actions: Print, PDF, View, Edit, Cancel/Delete, Verify, e-way/e-invoice, Duplicate, convert estimate

### Form
- Header panel: customer, invoice #/date, bill type, salesman, area/route/vehicle, cash type, consignee, GST fields
- Line grid: item lookup, godown, batch, qty, free/damage qty, rate, MRP, tax, discount, totals
- Totals bar: subtotal, tax, cess, discount, freight, other, round-off, grand total, advances/redeem
- Payment split for mixed cash/bank/credit modes
- Print preview / size selection

## 5. Fields (detailed)
### Header
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Invoice No | invoice | text | yes | auto from bill type | unique per series | set_billtype | always | Manual override if allowed |
| Invoice Date | invoice_date | date | yes | today | valid date | — | always | |
| Bill Type | bill_type | select | yes | default series | — | set_billtype | always | Drives numbering/print |
| Type | type | hidden/select | yes | sales | enum | sales, sales_return, estimate, delivery_note, quotation | by menu | Discriminator |
| Customer | customer_id | lookup | yes* | — | exists | suppliers/customers | always | *cash customer allowed |
| Customer name/phone/email/GST/state | cusname, cusdetails, cusemail, cusgst, cusstate | text | conditional | from customer | GST format if B2B | — | always | |
| Sale type | saletype | select | no | b2c | b2b/b2c | — | GST on | |
| Salesman | salesman | select | no | current user | — | users/agents | settings | |
| Area / Route / Vehicle | area, route, vehicle | select | no | — | — | masters | settings | |
| Cash type | cash_type | select | yes | Credit | Cash/Bank/Credit/COB/mixed | — | sales/return | Drives ledger posting |
| Cash / Bank amount | cash_amount, bank_amount | number | conditional | 0 | >=0; splits sum rules | — | mixed modes | |
| Bank | bank_id | select | conditional | — | — | bank ledgers | bank modes | |
| Discount / Freight / Other / Round off | discount, freight, other, roundoff | number | no | 0 | >=0 or signed round | — | always | Separate ledger posts |
| Tax / Cess totals | tax, cess, additional_cess | number | calc | 0 | — | line taxes | always | Calculated |
| Grand total | total | number | calc | 0 | — | — | always | |
| Advance / Redeemed | advance, redeemed_amount, redeemed_points | number | no | 0 | points rules | loyalty | settings | |
| Consignee fields | con_name/addr/contact/gst/state | text | no | — | — | — | e-way/transport | |
| E-way / transport | ewb_no, distance, vehicle_no, transport_mode | text/number | conditional | — | e-way rules | — | GST | |
| Terms / packing / ref | terms_condition, packing_details, ref_no, place | text | no | — | — | — | always | |
| Print type / paper | printtype, paper_size | select | no | company default | — | print types | print | |

### Lines
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Item | item_id | lookup | yes | — | active item | item | always | |
| Godown | godown | select | conditional | default | — | ims_godown | multi-godown | |
| Batch | batch | select/text | conditional | — | — | item_godown | batch mode | |
| Unit | unit_id | select | yes | item unit | — | units | always | Base qty conversion |
| Qty / Free / Damage | quantity, free_quantity, damage_quantity | number | qty yes | 0 | >0 for qty | — | always | Separate stock posts |
| Rate / MRP | rate, mrp, salemrp | number | yes | item rates | >=0 | item / price list | always | |
| Tax | tax_id, tax | select/number | yes | item tax | — | taxgst | always | |
| Discount | dis_amt, add_dis_per/amt | number | no | 0 | — | — | always | |
| Cess | itm_ces_* / ces_amt | number | no | 0 | — | cess | settings | |
| Line total | total | calc | — | — | — | — | always | |
| Description / IMEI / warranty / SQFT | item_discription, emei, warranty*, SQFT* | text | conditional | — | serial rules | — | settings | |

## 6. Actions and flows
### Create Sale
1. Open Sales Bill (type=sales).
2. Select customer + bill type; lines added via item search.
3. Set cash type and payment split.
4. Save → validate → insert sales + lines → stock out → ledger + ageing → optional print/PDF JSON response.

### Edit Sale
1. Load bill; if permitted.
2. Save → **unpost** prior stock/ledger/ageing (soft-invalidate) → **repost** new values.

### Cancel / Delete
- Cancel: soft unpublish + restore stock/ledger
- Delete: hard remove after restore (restrict by policy in new system; prefer cancel)

### Convert Estimate / Delivery Note → Sale
- Select source document(s); create sales linked via ref ids; then stock/ledger as sales.

### Quick receipt against sale
- Enter amount against outstanding ageing; create receipt voucher + ageing allocation.

### Print / e-invoice / e-way
- Generate PDF by print profile; call compliance APIs when enabled.

## 7. Business rules
- Numbering from bill type series (prefix/separator/number/suffix)
- Stock: sales/delivery_note decrease; sales_return increase; estimate/quotation **no stock**
- Ledger: by cash_type (cash/bank/credit/splits); discount/freight/other/round/cess as separate posts
- Ageing for credit portions; receipts allocate against ageing
- Edit = unpost + repost (audit via published flag)
- Loyalty points when enabled
- Permission: GetUserPermission / menu_to_user is_write for edits
- New system: replace hardcoded tenant bill templates with configurable print templates

## 8. Data requirements
- Entities: Sale, SaleLine, SaleLedgerLine, SaleCoupon, Item, ItemGodown, ItemLedger, AccLedger, AccLedgerTrans, AccAgeing, AccAgeingTrans, Voucher (quick receipt), BillType, Tax, Cess, EWay, EInvoice
- Relationships: Sale 1→N SaleLine; Sale N→1 Customer; SaleLine N→1 Item/Godown; Sale 1→N AccLedgerTrans; Sale 1→N AccAgeing
- Soft delete via published/status; audit created-by/updated-by recommended

## 9. Integrations
- PDF print (many layouts)
- E-way / E-invoice APIs
- FCM push (optional)
- WhatsApp/SMS (tenant-specific)
- Excel export from list

## 10. Permissions
- Menu grants: read, add, write, delete, cancel, print/export as separate flags in new system

## 11. Reports / exports
- List export; bill PDF; packing/scheme annexures (tenant features → config)

## 12. Edge cases and errors
- Duplicate invoice number
- Insufficient stock
- Edit after e-invoice generated (lock)
- Mixed payment splits not totaling invoice
- Cancel of partially receipted bill
- Concurrent edits

## 13. Acceptance criteria
- Saving a credit sale decreases stock, posts customer debit + tax ledgers, creates ageing
- Cash sale posts cash ledger and does not create open receivable
- Estimate saves without stock/ledger; convert creates real sale
- Edit recalculates stock and ledgers correctly
- Cancel restores stock and clears financial impact
- Unauthorized user cannot edit

## 14. Legacy reference
- Old URL: `transaction/sales/listall/{type}`, `transaction/sales/sales/{type}`
- Controller: `transaction/Sales.php`; model `Sales_model`; base posting in `dataTable.php`
- Views: `transaction/sales/list_all`, `sales`, `estimate`, `salesr`, bill templates
- Tables: `sales`, `sales_items`, `acc_ledger_trans`, `acc_ageing`, `item`/`item_godown`, `set_billtype`
- Open questions: which print templates are product defaults vs one-off tenants; delivery_note exact ledger rules per deployment

# Quotation

## 1. Identity
- Module / area: Transactions / Sales
- Page type: List + Create + Edit + View
- Primary users / roles: Sales staff
- Entry points: Menu "Quotation"
- Priority for rebuild: P0

## 2. Purpose
Create non-financial sales quotations for customers (pricing proposals). No stock or ledger impact until converted to sales/estimate workflow as product defines.

## 3. Preconditions
- Customers, items, tax, bill type for quotation series

## 4. Layout and UI
- List: invoice, date, customer, total; Print/View/Edit/Cancel
- Form similar to sales header + lines (simplified totals)

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Quotation No | invoice | text | yes | auto | unique | set_billtype | always | Stored in sales.type=quotation |
| Date | invoice_date | date | yes | today | — | — | always | |
| Customer | customer_id | lookup | yes | — | — | customers | always | |
| Lines | item, qty, rate, tax, discount, total | grid | yes | — | qty>0 | items | always | Same shape as sales lines |
| Grand total | total | calc | — | — | — | — | always | |

## 6. Actions and flows
- Create/Edit/Save quotation document
- Print PDF (shared bill templates)
- Optional: convert to sales/estimate (define explicitly in new system)

## 7. Business rules
- No stock posting
- No ledger / ageing posting
- Numbering via bill type
- Permission gap in legacy — new system must enforce menu grants

## 8. Data requirements
- Reuse Sale/SaleLine with document type `quotation` OR separate Quotation entity (prefer clear document type enum)

## 9. Integrations
- PDF print

## 10. Permissions
- read/add/write/cancel/print

## 11. Reports / exports
- Quotation list report

## 12. Edge cases
- Converting expired quotation
- Price changes after quotation date

## 13. Acceptance criteria
- Save quotation without changing stock or ledgers
- Print produces customer-facing PDF
- List filters by date/customer

## 14. Legacy reference
- URL: `transaction/Quotation/listall`, `quotation`
- Controller: `transaction/Quotation.php`, `Quotation_model`
- Tables: `sales`, `sales_items` with type=quotation
- Open questions: broken delegated methods referencing `$this->sales` in legacy — do not port bugs

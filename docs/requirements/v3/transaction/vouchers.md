# Vouchers (Payment / Receipt / Journal / Contra)

## 1. Identity
- Module / area: Transactions / Vouchers
- Page type: List + Create + Edit + View
- Primary users / roles: Accounts staff, Admin
- Entry points: Menu Payment/Receipt/Journal/Contra Vouchers; quick receipt from Sales
- Priority for rebuild: P0

## 2. Purpose
Record general ledger vouchers:
- **Payment** — pay supplier/expense from cash/bank; allocate to ageing
- **Receipt** — collect from customer to cash/bank; allocate to invoices
- **Journal** — multi-line balanced debit/credit entries
- **Contra** — transfer between cash and bank ledgers

## 3. Preconditions
- Chart of accounts / ledgers; bill types for voucher numbering; cost centers optional
- Permissions per voucher type

## 4. Layout and UI
- List per type with print/view/edit/cancel
- Payment/Receipt form: party ledger, cash/bank, amount, narration, date, salesman, **ageing allocation grid**
- Journal: multi-line debit/credit grid + narration
- Contra: from account, to account, amount, type

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Voucher No | voucher_invoice | text | yes | auto | unique | set_billtype | always | |
| Date | trans_date | date | yes | today | — | — | always | |
| Bill type | bill_type | select | yes | — | — | set_billtype | always | |
| From / Party ledger | ledger / frmact | lookup | yes | — | — | acc_ledgers | pay/rec | |
| To / Cash-Bank | toid | select | yes | — | — | cash/bank ledgers | pay/rec | |
| Amount | amount | number | yes | — | >0 | — | pay/rec/contra | |
| Narration | naration | text | no | — | — | — | always | |
| Salesman / Cost center | salesman, cost_center | select | no | — | — | users / cost_centre | settings | |
| Linked invoices / ageing | invoices, age_id[], txtamt[] | grid | no | open bills | sum ≤ amount | acc_ageing | pay/rec | Bill-wise settlement |
| Journal lines | id[], debit[], credit[], narration_sub[] | grid | yes | — | sum debit = sum credit | acc_ledgers | journal | Enforce balance in new system |
| Contra from/to | frm_act, to_act, contra_type | select | yes | — | distinct accounts | cash/bank | contra | |

## 6. Actions and flows
- Save payment/receipt → voucher + voucher_trans + acc_ledger_trans + optional ageing_trans
- Save journal → balanced multi-line posts
- Save contra → two-sided transfer
- Cancel/Delete with unpost pattern
- Print voucher PDF
- Block edit/delete of auto receipts spawned from Sales (`sales_ref_id`)

## 7. Business rules
- Numbering via bill type series
- Ageing allocation reduces outstanding
- Journal must balance (add server validation in new system)
- Edit = invalidate old lines + insert new
- WhatsApp confirmation for some tenants → make configurable

## 8. Data requirements
- Voucher, VoucherTrans, AccLedger, AccLedgerTrans, AccAgeing, AccAgeingTrans, CostCentre, BillType

## 9. Integrations
- PDF; WhatsApp/SMS optional; Excel list export

## 10. Permissions
- Separate grants: payment, receipt, journal, contra

## 11. Reports / exports
- Voucher registers; day book; payment/receipt reports

## 12. Edge cases
- Over-allocation to ageing
- Unbalanced journal
- Editing sales-linked receipt
- Duplicate voucher numbers

## 13. Acceptance criteria
- Receipt against customer reduces ageing for selected bills
- Payment posts expense/party and cash/bank correctly
- Journal rejects unbalanced entries
- Contra moves value between cash and bank only
- Sales-auto receipts not independently deletable

## 14. Legacy reference
- URL: `transaction/vouchers/listall/{type}`, `payment|receipt|journal|contra`
- Controller: `transaction/Vouchers.php`, `vouchers_model`
- Tables: `voucher`, `voucher_trans`, `acc_ledger_trans`, `acc_ageing_trans`

# API: Voucher Payment / Receipt

## 1. Identity
- Module / area: API / Vouchers
- Page type: API
- Primary users / roles: Field collection apps
- Entry points: `restapi/vouchers/Payment/create/{comid}`
- Priority for rebuild: P1

## 2. Purpose
Create payment and receipt vouchers from mobile collections, including ageing allocation and optional WhatsApp notify.

## 3. Preconditions
- Auth token; ledgers; open ageing rows for receipts

## 4. Layout and UI
- N/A

## 5. Fields (detailed)
| Field | Type | Required | Notes |
|-------|------|----------|-------|
| comid | path | yes | Tenant |
| vouchers[] | array | yes | Bulk |
| type | Payment/Receipt | yes | Maps pay/rec |
| ledger / cash-bank / amount / date / narration | mixed | yes | |
| ageing[] | array | no | age_id, amount allocations |

## 6. Actions and flows
- POST → create voucher + trans + ledger + ageing_trans → optional WhatsApp

## 7. Business rules
- Same as [transaction/vouchers.md](../transaction/vouchers.md)
- WhatsApp triggers configurable per tenant (not hardcoded comids)

## 8. Data requirements
- Voucher, VoucherTrans, AccLedgerTrans, AccAgeingTrans

## 9. Integrations
- WhatsApp/SMS gateway

## 10. Permissions
- `vouchers:write` scope

## 11. Reports / exports
- N/A

## 12. Edge cases
- Over-allocation; offline duplicate receipts

## 13. Acceptance criteria
- Mobile receipt reduces customer outstanding like web receipt
- Ageing lines match allocated bills
- Auth required

## 14. Legacy reference
- `restapi/vouchers/Payment.php`

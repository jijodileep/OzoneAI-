# Ledgers (Chart of Accounts)

## 1. Identity
- Module / area: Accounts Master
- Page type: List | Create | Edit | View
- Primary users / roles: Accounts, Admin
- Entry points: Menu "Ledgers" → `accounts_master/Ledger/listall`; also COA grocery screens for types
- Priority for rebuild: P0

## 2. Purpose
Maintain ledger accounts used by all vouchers, sales, purchase, and financial reports. Support opening balances and Dr/Cr nature.

## 3. Preconditions
- Account Type / Sub Type / Main Ledger hierarchy exists (or created inline)
- Tax masters if tax linked ledgers used

## 4. Layout and UI
- List with running balance (opening ± transactions)
- Create/Edit form for ledger attributes
- Related grocery CRUD screens: Main Account, Sub Account, Ledger Group, Financial Year

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Name / Alt | name, name_lan | text | yes | — | non-empty | — | always | |
| Code | code / acc_ledger_code | text | yes | auto | unique | hierarchy | always | Auto from parent |
| Main ledger | acc_main_id | select | yes | — | — | acc_main | always | |
| Type | type | select | yes | Dr | Dr/Cr | — | always | Balance sign |
| Opening balance | openning_bal | number | no | 0 | — | — | always | |
| Tax | tax_id | select | no | — | — | taxgst | tax ledgers | |
| System ref | ref_id, ref_code, is_default | hidden | — | — | — | customer/supplier link | system | Protect |

## 6. Actions and flows
- CRUD ledgers; drill to ledger book report
- Auto-create when customer/supplier saved
- Block delete when transactions exist or is_default

## 7. Business rules
- Dr/Cr determines balance formula everywhere
- Cascading auto-codes down COA hierarchy
- System ledgers (cus/sup/Cess/cash defaults) not freely deletable

## 8. Data requirements
- AccType, AccSubType, AccMain, AccLedger, FinYear, AccLedgerTrans, CompanyBranch

## 9. Integrations
- None

## 10. Permissions
- Accounts menu grants; Admin full

## 11. Reports / exports
- Ledger list; links to Ledger Book / Trial Balance

## 12. Edge cases
- Changing Dr/Cr after postings
- Duplicate names
- Orphan system ledger if party deleted

## 13. Acceptance criteria
- Create ledger under main account with opening balance
- Balance on list matches ledger book
- Customer create auto-creates linked ledger
- Cannot delete ledger with transactions

## 14. Legacy reference
- URL: `accounts_master/Ledger/listall`, `acctype`, `accsubtype`, `mainledgers`, `ledgers`, `finacial`
- Controller: `accounts_master/Ledger.php`
- Tables: `acc_type`, `acc_sub_type`, `acc_main`, `acc_ledgers`, `fin_year`

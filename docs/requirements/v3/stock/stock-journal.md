# Stock Journal

## 1. Identity
- Module / area: Inventory / Stock
- Page type: List | Create | Edit
- Primary users / roles: Inventory staff
- Entry points: Menu "Stock Journal" → `Stock_journal/listall`
- Priority for rebuild: P0

## 2. Purpose
Adjust stock between items/godowns or correct quantities via journal entries that post stock ledgers without a sales/purchase invoice.

## 3. Preconditions
- Items and godowns; permission for stock journal

## 4. Layout and UI
- List of journals by date/number
- Form with lines: item, godown, qty in/out or from/to, narration

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Journal No | invoice/number | text | yes | auto | unique | series | always | |
| Date | date | date | yes | today | — | — | always | |
| Narration | narration | text | no | — | — | — | always | |
| Lines: item, godown, batch, qty, rate | lines[] | grid | yes | — | qty≠0 | item, godown | always | Direction in/out |

## 6. Actions and flows
- Create journal → update item_godown / item_ledger
- Edit → reverse previous + apply new
- Cancel → restore

## 7. Business rules
- Must not break negative stock policy (configurable)
- Audit trail of adjustments
- No customer/supplier ledger impact (stock only) unless product defines otherwise

## 8. Data requirements
- StockJournal, StockJournalLine, Item, ItemGodown, ItemLedger

## 9. Integrations
- Print/export optional

## 10. Permissions
- Stock journal menu add/write/cancel

## 11. Reports / exports
- Stock Journal Report

## 12. Edge cases
- Zero lines; insufficient stock on outward

## 13. Acceptance criteria
- Saving outward line decreases godown stock
- Inward increases stock
- Cancel restores prior quantities

## 14. Legacy reference
- URL: `Stock_journal/listall`
- Controller: `Stock_journal.php`
- Report: `reports/ItemTransaction/stock_journal`

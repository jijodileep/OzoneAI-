# Physical Stock Update

## 1. Identity
- Module / area: Inventory / Stock
- Page type: List | Create | Edit
- Primary users / roles: Inventory staff, Admin
- Entry points: Menu "Physical Stock Update" → `Physical_stock/list`
- Priority for rebuild: P0

## 2. Purpose
Capture physical count vs system stock and post adjustments (gain/loss) to align `item_godown` quantities.

## 3. Preconditions
- Items/godowns; optionally freeze transactions during count (new system recommendation)

## 4. Layout and UI
- Select godown/date; grid of items with system qty, counted qty, difference
- Save posts adjustment movements
- Link to Physical Stock Report

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Count date | date | date | yes | today | — | — | always | |
| Godown | godown_id | select | yes | — | — | ims_godown | always | |
| Item | item_id | lookup/grid | yes | — | — | item | always | |
| System qty | system_qty | readonly | — | from stock | — | item_godown | always | |
| Physical qty | physical_qty | number | yes | system | >=0 | — | always | |
| Difference | diff | calc | — | physical-system | — | — | always | Posted as adjustment |

## 6. Actions and flows
- Load stock snapshot → enter counts → save adjustments → optional report

## 7. Business rules
- Positive diff → stock in; negative → stock out
- Keep audit of count session
- Batch-aware when batch mode on

## 8. Data requirements
- PhysicalStockSession, PhysicalStockLine, ItemGodown, ItemLedger

## 9. Integrations
- Excel import of counts (recommended for new tech)

## 10. Permissions
- Inventory write; approve step optional for new system

## 11. Reports / exports
- Physical Stock Report

## 12. Edge cases
- Counting during active sales; large variance approval

## 13. Acceptance criteria
- After save, godown stock equals physical qty for counted items
- Uncounted items unchanged
- Report shows variances

## 14. Legacy reference
- URL: `Physical_stock/list`
- Report: `reports/ItemTransaction/physical_stk_update`

# Bill of Materials (BOM) & Production Process

## 1. Identity
- Module / area: Production
- Page type: List | Create | Edit
- Primary users / roles: Production planners
- Entry points: `production/bom/bom_controller/listall`; process/stage/status/label; Job Card; Material In/Out
- Priority for rebuild: P2

## 2. Purpose
Define BOM for finished goods (materials + ledger costs), run production processes through stages/statuses, and record material/job card movements.

## 3. Preconditions
- Finished and raw items; production stage/status masters; ledgers for costing

## 4. Layout and UI
- BOM form: finished product, material lines, ledger cost lines, totals
- Process form: products, status, assigned user
- Job card in/out; material in/out lists

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Finished product | product_id / item_id | lookup | yes | — | — | item | BOM | |
| Output qty / price | pro_quantity, price_per_product | number | yes | — | >0 | — | BOM | |
| Material lines | materialid, quantity, price, total | grid | yes | — | qty>0 | item | BOM | |
| Ledger cost lines | ledgerid, ledger_amount | grid | no | — | — | ledgers | BOM | |
| Totals | materialamount, ledgeramount, amount | calc | — | — | — | — | BOM | |
| Process status / user | status, user | select | yes | — | — | production_status, users | process | |

## 6. Actions and flows
- Define BOM → start process → consume materials (stock out) → produce FG (stock in) → complete status
- Job card / material in-out as supporting documents

## 7. Business rules
- BOM materials should drive stock consumption on process completion (enforce in new system)
- Stages/statuses are configurable masters
- Cost = materials + ledger allocations

## 8. Data requirements
- BomMain, BomMaterialLine, BomLedgerLine, ProductionProcess, ProductionProcessProduct, ProductionStage, ProductionStatus, ProductionLabel, JobCard, MaterialDoc

## 9. Integrations
- Optional labels/print

## 10. Permissions
- Production menus

## 11. Reports / exports
- Process status; material movement reports

## 12. Edge cases
- BOM change mid-process; insufficient raw stock

## 13. Acceptance criteria
- Save BOM with materials and costs
- Process completion updates stock for RM/FG per rules
- Status searchable on process list

## 14. Legacy reference
- Controllers: `production/bom/Bom_controller.php`, `process/Process_controller.php`, stage/status/label; `Jobcard.php`, `Material.php`
- Tables: `bom_main`, `production_process_*`, `production_stage`, `production_status`

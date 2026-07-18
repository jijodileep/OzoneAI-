# Top / Non-Selling Items Analytics

## 1. Identity
- Module / area: Analytics
- Page type: Report | Dashboard
- Primary users / roles: Management, Inventory
- Entry points: `analytics/Item/topselling_list`, `not_selling`
- Priority for rebuild: P1

## 2. Purpose
Identify best- and worst-performing items by sales volume over a period to guide purchasing and promotions.

## 3. Preconditions
- Sales history; items master

## 4. Layout and UI
- Period filters; ranked tables; optional charts in new UI

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Date from/to | date_from, date_to | date | yes | month | — | — | always | |
| Rank / Item / Qty / Value | result columns | grid | — | — | — | sales_items+item | always | |
| Threshold for non-selling | min_qty | number | no | 0 | >=0 | — | non-selling | |

## 6. Actions and flows
- Run analysis → export → navigate to item master

## 7. Business rules
- Posted sales only; exclude cancelled
- Non-selling = no/low movements in period

## 8. Data requirements
- Item, Sale, SaleLine aggregates

## 9. Integrations
- Excel export

## 10. Permissions
- Analytics/report menu

## 11. Reports / exports
- Excel of rankings

## 12. Edge cases
- New items with no history

## 13. Acceptance criteria
- Top list ordered by qty/value for period
- Non-selling list excludes items above threshold

## 14. Legacy reference
- `analytics/Item.php`

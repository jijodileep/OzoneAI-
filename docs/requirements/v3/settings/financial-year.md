# Financial Year

## 1. Identity
- Module / area: Settings / Session
- Page type: List | Create | Edit | Settings
- Primary users / roles: Admin (manage/close); all users (switch)
- Entry points: Header FY switcher; Settings → Financial Years
- Priority for rebuild: P0

## 2. Purpose
Maintain financial years in the **same company database**, switch the active FY in session, enter FY opening balances, and close a year (carry openings to the next FY). Success: user works in one FY at a time; BS/stock use that FY’s openings + movements.

## 3. Preconditions
- Tenant login (company key)
- Company profile exists
- Spec: [docs/architecture/financial-year.md](../../../architecture/financial-year.md)

## 4. Layout and UI
- Header dropdown: current FY name; list open (+ closed as view-only)
- Settings list: Name, Start, End, Status, Default
- Create FY form: name, start/end dates
- Year close wizard: checklist → confirm → progress
- Opening balances screens: ledger OB grid; stock OB grid (item/godown)

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Name | name | text | yes | — | unique | — | always | e.g. 2025-26 |
| Start date | startDate | date | yes | — | &lt; end | — | always | |
| End date | endDate | date | yes | — | &gt; start | — | always | |
| Status | status | badge | — | Open | — | Open/Closing/Closed | always | |
| Default | isDefault | toggle | — | false | one default open | — | Open only | Login default |
| Switch to | financialYearId | select | yes | current | belongs to tenant | financial_years | header | Issues new JWT |

## 6. Actions and flows
- **Switch FY:** select → `POST /v1/session/financial-year` → new JWT → reload lists
- **Create FY:** Admin → save Open year
- **Set default:** Admin marks IsDefault
- **Close year:** Admin wizard → Hangfire job → next FY openings written
- **Edit openings:** only for Open FY (or first FY seed)

## 7. Business rules
- Document date must fall in active FY range
- Cannot post to Closed FY
- Closing FY is read-only for edits
- Year close writes `ledger_opening_balances` + `stock_opening_balances` for next FY
- No new database on year close

## 8. Data requirements
- `financial_years`, `ledger_opening_balances`, `stock_opening_balances`
- All txn tables: `FinancialYearId`
- JWT claim: `financial_year_id`

## 9. Integrations
- Balance sheet, trial balance, stock reports filter by FY
- Offline sync scoped to active FY

## 10. Permissions
- Switch: any authenticated user
- Create/Close/Edit openings: Admin

## 11. Reports / exports
- Balance sheet / stock valuation for active FY

## 12. Edge cases
- Overlapping date ranges forbidden
- Switch while unsaved form — warn/discard
- Close with unbalanced TB — block

## 13. Acceptance criteria
- Switch FY updates JWT; lists show only that year’s docs
- Cannot post into closed FY
- Year close creates next openings without new DB
- Ledger balance = Opening(FY) + movements(FY)

## 14. Legacy reference
- Tables: `fin_year`, `acc_ledgers.openning_bal`, optional year DB via `set_database`
- OzoneAI: FY id key only; OB in dedicated FY tables

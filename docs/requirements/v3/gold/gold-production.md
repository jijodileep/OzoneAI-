# Gold Production / Melting / Design / Sales-Job

## 1. Identity
- Module / area: Gold
- Page type: List | Create | Edit
- Primary users / roles: Jewellery ops
- Entry points: Menus under Gold — Melting, Production, Design, Sales And Jobcard
- Priority for rebuild: P2

## 2. Purpose
Track jewellery manufacturing workflow: melting, production jobs, design job cards with materials, and combined sales/jobcard billing against customers.

## 3. Preconditions
- Customers/suppliers; items/materials; gold module enabled

## 4. Layout and UI
- Separate lists/forms per sub-process
- Design: material consumption + jobcard items + bill
- Common header: date, customer, salesman/user

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Date | date | date | yes | today | — | — | always | |
| Customer | customer_id | lookup | yes | — | — | suppliers | always | |
| Salesman / User | salesman, user | select | no | current | — | users | always | |
| Materials / job items | grids | grid | conditional | — | qty>0 | items | design/production | |
| Totals / bill | totals | calc | — | — | — | — | billable | |

## 6. Actions and flows
- Melting → Production → Design jobcard → Sales/Jobcard bill
- Print bill from design

## 7. Business rules
- Customer-linked jobs
- Material consumption should affect stock in new system (confirm vs legacy)
- Bill generation links to sales where applicable

## 8. Data requirements
- GoldMelting, GoldProduction, GoldDesign, SalesAndJobcard, Customer, Item

## 9. Integrations
- Bill PDF

## 10. Permissions
- Gold menus

## 11. Reports / exports
- Job registers export

## 12. Edge cases
- Job without materials; cancelling after bill

## 13. Acceptance criteria
- Create melting/production/design against customer
- Design bill printable
- Sales-jobcard list shows saved jobs

## 14. Legacy reference
- Controllers: `gold/Melting.php`, `Production.php`, `Design.php`, `SalesJob.php`
- Tables: `gold_melting`, `gold_production`, `gold_design`, `sales_and_jobcard`

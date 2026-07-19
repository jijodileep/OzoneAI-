# Company Profile & Address

## 1. Identity
- Module / area: Settings
- Page type: Edit | View
- Primary users / roles: Admin
- Entry points: Settings → Company Profile; used on invoices / balance sheet header
- Priority for rebuild: P0

## 2. Purpose
Maintain company legal/trade identity and **address** in the tenant DB (separate from catalog registry). Support branches with their own addresses.

## 3. Preconditions
- Tenant provisioned; catalog `companies` row exists
- Spec: [docs/architecture/financial-year.md](../../../architecture/financial-year.md)

## 4. Layout and UI
- Profile form: name, address, tax IDs, logo, bank
- Branches list + edit modal
- Settings flags screen (separate tab): tax type, currency, feature toggles

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Legal name | legalName | text | yes | — | non-empty | — | always | |
| Trade name | tradeName | text | no | — | — | — | always | |
| Address | address | textarea | yes | — | — | — | always | Bill / BS header |
| Address (local) | addressLocal | textarea | no | — | — | — | always | Alt language |
| Phone | phone | text | yes | — | — | — | always | |
| Email | email | text | no | — | email | — | always | |
| GSTIN / TIN | taxNumber | text | no | — | — | — | always | |
| FSSAI | fssai | text | no | — | — | — | always | |
| Logo | logoObjectKey | upload | no | — | image | MinIO | always | `{companyKey}/logo/` |
| Branch name | name | text | yes | — | — | company_branches | branch form | |
| Branch address | address | textarea | yes | — | — | — | branch form | |

## 6. Actions and flows
- Save profile → updates `company_profile`
- Add/edit/delete branch (block delete if referenced)
- Upload logo → MinIO; store object key

## 7. Business rules
- Catalog registry does not store full address (ops only: key, plan, connection)
- Profile seeded at create-tenant from Super Admin form

## 8. Data requirements
- `company_profile`, `company_branches`, `company_settings`
- Catalog: `companies.PlanId` → `subscription_plans`

## 9. Integrations
- Invoice print, balance sheet header, Super Admin tenant create

## 10. Permissions
- Admin edit; others read if needed for print

## 11. Reports / exports
- Address appears on BS and bill formats

## 12. Edge cases
- Missing address → warn on print
- Logo too large

## 13. Acceptance criteria
- Admin can save address; BS/invoice show it
- Branches CRUD works
- Plan assigned in catalog; profile independent in tenant DB

## 14. Legacy reference
- Tables: `company`, `company_branch`, `company_settings`, `company_subcription`

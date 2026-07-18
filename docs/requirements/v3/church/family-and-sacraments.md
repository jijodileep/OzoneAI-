# Church Family Registry & Sacraments

## 1. Identity
- Module / area: Church
- Page type: List | Create | Edit | View
- Primary users / roles: Parish admin
- Entry points: Family head/members; Baptism/Marriage/Death registers; Receipts
- Priority for rebuild: P2

## 2. Purpose
Maintain parish family registry and sacrament registers with certificate printing; handle donations/subscription receipts.

## 3. Preconditions
- Parish masters: diocese, parish, locality, wards, relations, etc.

## 4. Layout and UI
- Family head list → members
- Sacrament forms with certificate print
- Receipt generate/complete by type
- Auction / Qurbana registers

## 5. Fields (detailed)
### Family
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Family no / Head | familyno, head fields | text | yes | — | unique family no | — | always | |
| Members | member + relation | grid/form | yes | — | — | relations | members | |

### Baptism (representative sacrament)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Baptised on | baptised_on | date | yes | — | — | — | baptism | |
| Child / parents | child_name, dad_name, mom_name | text | yes | — | — | family | baptism | |
| Place / diocese / locality | place, diocese, locality | select/text | yes | — | — | masters | baptism | |
| Amount / remarks | amount, remarks | number/text | no | — | — | — | baptism | |

## 6. Actions and flows
- Register family → add members → record sacrament → print certificate → optional receipt

## 7. Business rules
- Sacraments link to family head/member
- Certificate is primary legal/output artifact
- Receipt types configurable

## 8. Data requirements
- FamilyHead, FamilyMember, BaptismRegister, MarriageRegister, CremationRegister, ChurchReceipt, ReceiptType, parish masters

## 9. Integrations
- Certificate PDF; receipt print

## 10. Permissions
- Church menus

## 11. Reports / exports
- Baptism/Marriage/Death reports

## 12. Edge cases
- Member without head; certificate reprint

## 13. Acceptance criteria
- Create family with members
- Save baptism and print certificate
- Issue receipt against family/subscription

## 14. Legacy reference
- Controllers under `church/*` and `church/reports/*`
- Tables: `chr_family_head`, `chr_family_member`, `chr_*_register`, `chr_receipts*`

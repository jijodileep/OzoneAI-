# Enquiry Register (CRM)

## 1. Identity
- Module / area: CRM
- Page type: List | Create | Edit
- Primary users / roles: Sales / CRM users
- Entry points: Menu "Enquiry Register" (crm_general or crm_new / crm_service variants)
- Priority for rebuild: P1

## 2. Purpose
Capture sales/service enquiries with customer details, products of interest, source, and status; feed allocation and follow-up.

## 3. Preconditions
- Source of enquiry, status, products/items, optional leads

## 4. Layout and UI
- Enquiry list filters (date, status, source, salesperson)
- Form: party details, products grid, source, notes
- Actions: allocate, follow-up, close, convert to lead/bill (service)

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Enquiry date | enquiry_date | date | yes | today | — | — | always | |
| Customer / company | customer fields | text/lookup | yes | — | — | suppliers/customers | always | |
| Phone / Email | phone, email | text | yes/no | — | format | — | always | |
| Source | source_id | select | yes | — | — | source_enquiry | always | |
| Products | enquiry_product[] | grid | no | — | — | item | always | |
| Status | status_id | select | yes | new | — | status_master | always | |
| Assigned to | allocation user | select | no | — | — | users | after allocate | |
| Notes | remarks | textarea | no | — | — | — | always | |

## 6. Actions and flows
1. Register enquiry (± from lead)
2. Allocate to salesperson
3. Log follow-ups/status changes
4. Close with outcome / generate service bill (service CRM)

## 7. Business rules
- Lifecycle: Lead → Enquiry → Allocation → Follow-up → Closure
- Parallel modules (crm_general, crm_new, crm_service) should consolidate in new tech with a channel/type flag
- Excel import supported in general CRM

## 8. Data requirements
- LeadRegister, EnquiryRegister, EnquiryProduct, EnquiryAllocation, EnquiryFollowup, EnquiryClosure, StatusMaster, SourceEnquiry, LabelMaster

## 9. Integrations
- Excel import; optional SMS reminders (future)

## 10. Permissions
- CRM menus; allocate maybe supervisor-only

## 11. Reports / exports
- Enquiry/Lead/Follow-up/Allocation/Opportunity reports

## 12. Edge cases
- Duplicate enquiries same phone; closing without allocation

## 13. Acceptance criteria
- Create enquiry with source and product
- Allocate and see in allocated report
- Follow-up history retained
- Closure updates status

## 14. Legacy reference
- Controllers: `crm_general/Enquiry_register.php`, `Enquiry_allocation.php`, `Enquiry_followup.php`; parallels under `crm_new`, `crm_service`
- Tables: `enquiry_register`, `enquiry_allocation`, `enquiry_followup`, `lead_register`

# Student (Education)

## 1. Identity
- Module / area: Education
- Page type: List | Create | Edit
- Primary users / roles: Education admin
- Entry points: `education/Student/listall`
- Priority for rebuild: P2

## 2. Purpose
Admit students, assign courses/batches/fees, track documents, fee schedules, payments (via vouchers/ledgers), attendance, and year promotion.

## 3. Preconditions
- Courses, universities, fee structures, batches, districts/qualifications

## 4. Layout and UI
- Student list; admission form; fee list; document list; promote next year

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Student name | name | text | yes | — | — | — | always | |
| Course / Batch | course_id, batch | select | yes | — | — | course/item, batches | always | |
| Fee structure / schedules | feestructure, crm_student_feeschedules | grid | yes | from course | — | fees | always | |
| Contacts / address / district | phone, address, district | text/select | yes/no | — | — | district | always | |
| Documents | document_list | uploads | no | — | — | document master | always | |
| Ledger | acc_ledgers link | system | — | auto | — | ledgers | fees | Fee postings |

## 6. Actions and flows
- Admit → assign fees → collect payment (voucher) → attendance → promote year

## 7. Business rules
- Course modeled as item with fee components in legacy — new system may use Course entity
- Fee payments post to student ledger
- Promotion creates next-year record

## 8. Data requirements
- Student, Course, FeeStructure, FeeSchedule, Attendance, Schedule, Document, AccLedger, Voucher

## 9. Integrations
- Fee PDF/receipts; reports/Feesreport

## 10. Permissions
- Education menus

## 11. Reports / exports
- Fees report; examination report

## 12. Edge cases
- Partial fee payment; course change mid-year

## 13. Acceptance criteria
- Admit student with course and fee schedule
- Payment reduces fee outstanding
- Attendance markable by class/time

## 14. Legacy reference
- Controllers: `education/Student.php`, `Course.php`, `Attendance.php`, `Schedule.php`
- Tables: `crm_student`, `feestructure`, `crm_student_feeschedules`

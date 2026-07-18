# Employee

## 1. Identity
- Module / area: HR
- Page type: List | Create | Edit | View
- Primary users / roles: HR Admin
- Entry points: Menu "Employee" → `hr/Employee/list_all`
- Priority for rebuild: P1

## 2. Purpose
Maintain employee master: personal data, job role/department/designation, documents, visa/passport expiry tracking, and link to salary packages / payslips.

## 3. Preconditions
- Department, Designation, Role, Bloodgroup, Course masters as needed

## 4. Layout and UI
- Employee grid with search
- Multi-section form: personal, job, contacts, documents upload, visa/iqama/license/insurance
- Actions: salary list, payslip PDF receipt

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Employee name | name | text | yes | — | — | — | always | |
| Code | emp_code | text | yes | auto | unique | — | always | |
| Department / Designation / Role | dept_id, designation_id, job_role | select | yes | — | — | HR masters | always | |
| Join date | join_date | date | yes | — | — | — | always | |
| Phone / Email | phone, email | text | no | — | format | — | always | |
| Visa/Passport/Iqama details | hr_visa_passport_details | subform | no | — | expiry dates | visa types | always | Expiry alerts |
| Documents | uploads | file | no | — | types/size | — | always | |
| Salary package link | package_id | select | no | — | — | hr_salary_package | payroll | |

## 6. Actions and flows
- CRUD employee; upload docs; view salary history; print payslip

## 7. Business rules
- Soft deactivate preferred over delete
- Expiry tracking for compliance documents
- Salary distribution references employee profile

## 8. Data requirements
- HrProfile, HrVisaPassportDetails, HrDepartment, HrDesignation, HrRole, HrSalaryPackage, documents

## 9. Integrations
- Payslip PDF

## 10. Permissions
- HR menu grants

## 11. Reports / exports
- Employee Report; document expiry report

## 12. Edge cases
- Duplicate employee codes; deleting employee with payroll history

## 13. Acceptance criteria
- Create employee with dept/designation
- Appears in salary distribution employee list
- Payslip PDF generates for a payment

## 14. Legacy reference
- Controller: `hr/Employee.php`
- Tables: `hr_profile`, `hr_visa_passport_details`

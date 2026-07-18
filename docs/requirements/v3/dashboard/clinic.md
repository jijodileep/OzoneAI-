# Clinic Dashboard

## 1. Identity
- Module / area: Dashboards / Clinic
- Page type: Dashboard
- Primary users / roles: clinic role users
- Entry points: Login redirect when `utype` = clinic
- Priority for rebuild: P1

## 2. Purpose
Show clinic operational snapshot: appointments and patient status buckets (Approve, Checked, Payed).

## 3. Preconditions
- Authenticated clinic user; clinic module enabled for tenant

## 4. Layout and UI
- Appointment widgets
- Status counts: Approve / Checked / Payed
- DataTables list of bookings (patient name, phone)

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Appointments | appointments | list/KPI | — | today | — | cli_booking | always | |
| Status buckets | Approve, Checked, Payed | KPI | — | — | — | booking status | always | |
| Patient name/phone | Pat_Name, Pat_Phone | grid | — | — | — | cli_patient | list | |

## 6. Actions and flows
- Open booking / patient from grid
- Filter by status type via list endpoint

## 7. Business rules
- Status workflow owned by clinic booking module
- Counts reflect current tenant clinic data only

## 8. Data requirements
- Entities: ClinicBooking, Patient, Doctor, TimeSchedule

## 9. Integrations
- None on dashboard

## 10. Permissions
- Clinic role or menu grant

## 11. Reports / exports
- Optional booking list export

## 12. Edge cases
- No appointments today

## 13. Acceptance criteria
- Clinic login lands here with booking KPIs
- Grid lists bookings with patient contact

## 14. Legacy reference
- Old URL: `dashboard/clinic`
- Controller: `Dashboard.php::clinic`, `clinic_list`
- Models: `clinic/dash_model`

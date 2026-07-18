# Clinic Booking (Appointment)

## 1. Identity
- Module / area: Clinic
- Page type: List | Create | Edit
- Primary users / roles: Clinic staff
- Entry points: `clinic/Booking/listall`
- Priority for rebuild: P1

## 2. Purpose
Schedule appointments with doctor, procedure, time slot; progress through approval/check/payment/complete; print certificates/bills.

## 3. Preconditions
- Patients, doctors, procedures, time schedules

## 4. Layout and UI
- Booking list by date/status
- Form: patient, doctor, category, date/time, procedure, notes
- Payment and certificate actions

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Patient | customer_id / patient | lookup | yes | — | — | cli_patient | always | |
| Phone / Email | phone, email | text | no | from patient | — | — | always | |
| Doctor | doc | select | yes | — | — | cli_doctor | always | |
| Category | cat | select | no | — | — | clinic category | always | |
| Date / Time | date, at, from | date/time | yes | — | slot free | cli_timeschedule | always | |
| Procedure | procedure | select/multi | no | — | — | procedures | always | |
| Notes | notes | textarea | no | — | — | — | always | |
| Status | status | select | yes | booked | workflow | — | always | Approve/Checked/Payed/Complete |

## 6. Actions and flows
- Book → Approve → Check → Pay → Complete
- Generate bill / certificate PDF
- Medicine/questionnaire linked screens

## 7. Business rules
- Slot conflict detection (required in new system)
- Payment updates status to Payed
- Dashboard buckets use these statuses

## 8. Data requirements
- ClinicBooking, BookingProcedures, Patient, Doctor, TimeSchedule, Payment

## 9. Integrations
- PDF certificate/bill; clinic sync API

## 10. Permissions
- Clinic booking menu; approval may be restricted

## 11. Reports / exports
- Cash details; completed list

## 12. Edge cases
- Double-book same slot; cancel after payment

## 13. Acceptance criteria
- Create booking for patient+doctor+slot
- Status transitions reflected on clinic dashboard
- Bill/certificate printable after payment

## 14. Legacy reference
- Controllers: `clinic/Booking.php`, `Payment.php`, `Approvel.php`, `Completed.php`
- Tables: `cli_booking`, `cli_bookingprocedures`

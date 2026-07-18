# Clinic Patients

## 1. Identity
- Module / area: Clinic
- Page type: List | Create | Edit
- Primary users / roles: Clinic staff
- Entry points: Menu / `clinic/Patients/listall`
- Priority for rebuild: P1

## 2. Purpose
Maintain patient demographics and family relations for booking, billing, and clinical records.

## 3. Preconditions
- Blood group master; clinic module enabled

## 4. Layout and UI
- Patient list + calendar entry points
- Form: identity, contact, demographics, family link, notes

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Name | name | text | yes | — | — | — | always | |
| Phone | phone | text | yes | — | phone | — | always | Search key |
| Email | email | text | no | — | email | — | always | |
| Gender | gender | select | yes | — | — | M/F/O | always | |
| DOB / Age | dob, age | date/number | one of | — | — | — | always | |
| Blood group | bloodgrp | select | no | — | — | cli_bloodgroup | always | |
| Occupation / Reference | occupation, reference | text | no | — | — | — | always | |
| Family / Relation | fmly, relation, member_* | lookup | no | — | — | patients | always | |
| Notes | notes | textarea | no | — | — | — | always | |

## 6. Actions and flows
- CRUD patient; open booking from patient; calendar view

## 7. Business rules
- Unique phone recommended
- Bookings reference patient id
- Sync API may push/pull patients (see api/clinic-sync.md)

## 8. Data requirements
- ClinicPatient, BloodGroup, ClinicBooking

## 9. Integrations
- Clinic sync API; certificate print via booking

## 10. Permissions
- Clinic menus

## 11. Reports / exports
- Patient list export

## 12. Edge cases
- Duplicate phones; minor patients without phone

## 13. Acceptance criteria
- Create patient and select in new booking
- Search by phone finds patient
- Family relation stored

## 14. Legacy reference
- Controller: `clinic/Patients.php`
- Table: `cli_patient`

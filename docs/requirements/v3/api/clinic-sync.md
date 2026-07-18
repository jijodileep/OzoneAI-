# API: Clinic Bidirectional Sync

## 1. Identity
- Module / area: API / Clinic
- Page type: API
- Primary users / roles: Clinic local installs syncing to central server
- Entry points: `restapi/clinic/Server`, `api/clinic/Server`; legacy `clinic/api/*`
- Priority for rebuild: P1

## 2. Purpose
Sync patients, bookings, doctors, diagnoses, observations, procedures, potency, schedules, blood groups, categories, clinic profile between local clinic and central server (pull all, push saves, ack synced).

## 3. Preconditions
- Clinic credentials; **secure auth** (legacy has hardcoded DB credentials in controller — forbid in new system)

## 4. Layout and UI
- N/A

## 5. Fields (detailed)
Logical endpoint groups:

| Group | Methods | Entities |
|-------|---------|----------|
| Patients | save/update/getall/updatesync | ClinicPatient, relations |
| Bookings | save/update/getall/getsub/updatesync | ClinicBooking + junctions |
| Doctors | save/getall/updatesync | ClinicDoctor |
| Masters | diagnose, observation, potency, procedures, timeschedule, intervell, bloodgroup, category, clinic | respective tables |
| Ack | updatesync* | marks synced flags |

## 6. Actions and flows
1. Pull getall* since cursor
2. Push local changes save*/update*
3. Ack updatesync*
4. Resolve conflicts (define last-write-wins or vector clocks in new system)

## 7. Business rules
- Idempotent upserts by stable UUIDs (introduce if legacy uses int ids only)
- Do not embed DB passwords in clients/controllers
- Prefer `restapi/clinic/Server` surface; drop buggy legacy `clinic/api/Booking`

## 8. Data requirements
- All clinic entities + SyncMeta (device, cursor, last_sync)

## 9. Integrations
- Central clinic server

## 10. Permissions
- Clinic API client credentials per clinic tenant

## 11. Reports / exports
- Sync error dashboard (new)

## 12. Edge cases
- Partial failure mid-batch; clock skew; duplicate patients by phone

## 13. Acceptance criteria
- Local booking pushed appears on server and other clients after pull
- Ack prevents re-sending same records
- No plaintext credentials in source

## 14. Legacy reference
- `restapi/clinic/Server.php`, `api/clinic/Server.php`, `clinic/api/Server.php`, `clinic/api/Booking.php`

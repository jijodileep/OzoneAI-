# Login

## 1. Identity
- Module / area: Authentication / Tenant access
- Page type: Settings (auth form)
- Primary users / roles: All users before session established
- Entry points: Default app URL `/login`; logout redirect; session expiry redirect
- Priority for rebuild: P0

## 2. Purpose
Authenticate a user into the correct company (tenant) database and land them on the role-appropriate dashboard. Success: a secure session with tenant context and user identity.

## 3. Preconditions
- Master/registry database available (company key → tenant DB credentials)
- Tenant database reachable
- User record exists in tenant `users` table
- Company subscription / access not blocked (if enforced in new system)

## 4. Layout and UI
- Full-page login form (legacy: `_jlogin/login`)
- Fields: Company ID/key, Username, Password
- Error banner for invalid company or invalid credentials
- Optional branding (product skin / logo) from config
- No authenticated chrome / menu

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Company ID | comid | text | yes | empty | non-empty; must resolve to tenant | master company registry | always | Tenant selector |
| Username | uname | text | yes | empty | non-empty | tenant users | always | |
| Password | pswd | password | yes | empty | non-empty | — | always | Legacy compared plaintext — **new system must hash** |
| Error | error | display | no | — | — | — | on failure | Company vs credentials message |

## 6. Actions and flows
### Login
1. User submits company ID, username, password.
2. Resolve tenant DB from company key.
3. Authenticate user in tenant DB.
4. Create session: user, tenant connection, financial DB if any, login audit row.
5. Redirect by role:
   - Super Admin → Super Admin dashboard
   - Admin → Admin dashboard
   - clinic → Clinic dashboard
   - else → Users dashboard
6. On failure: stay on login with clear error (invalid company vs invalid user/password).

### Logout
1. Clear session auth flags.
2. Redirect to login.

## 7. Business rules
- Two-step auth: company resolution then user auth
- Session is sole auth gate for web UI
- Login attempts should be audited (user, IP, status, time)
- New system: password hashing (bcrypt/argon2), rate limiting, optional MFA
- Multi-tenant isolation via per-company DB today; new design may keep DB-per-tenant or shared schema with tenant_id

## 8. Data requirements (for new DB/API design)
- Entities: Company (registry), TenantDatabaseConnection, User, UserType/Role, LoginAudit/Session
- Relationships: Company 1→1 TenantConnection; User N→1 UserType; LoginAudit N→1 User
- Persist: company key, connection metadata, username, password hash, role, last login
- Audit: login success/failure with IP and timestamp

## 9. Integrations
- None required on login form itself (SMS/email reset can be future)

## 10. Permissions
- Public page (unauthenticated)
- Post-login menu permissions loaded from role / menu grants

## 11. Reports / exports from this page
- None

## 12. Edge cases and errors
- Unknown company key
- Valid company, wrong password
- Tenant DB unreachable
- Inactive / expired company subscription
- Concurrent sessions policy (define for new system)

## 13. Acceptance criteria
- Given valid company + credentials, user lands on correct role dashboard with tenant data loaded
- Given bad company, user sees company-specific error and is not authenticated
- Given bad password, user sees credentials error
- Logout clears session; protected pages redirect to login
- Passwords are never stored or compared in plaintext in the new system

## 14. Legacy reference (traceability only)
- Old URL: `/login`, POST `/login/loginset`, `/login/logout`
- Controller / methods: `application/controllers/Login.php` — `index`, `loginset`, `logout`
- Main views: `application/views/_jlogin/login.php`
- Main models / tables: `Getdata_model` (set_database/company), `User_model` (users), login audit insert
- Open questions: whether company subscription expiry is enforced at login in all deployments; plaintext password migration path for existing tenants

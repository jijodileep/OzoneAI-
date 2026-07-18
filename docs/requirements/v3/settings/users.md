# Users

## 1. Identity
- Module / area: Settings / User Settings
- Page type: List | Create | Edit
- Primary users / roles: Admin, Super Admin
- Entry points: Menu "Users" → `settings/NewMaster/Users`
- Priority for rebuild: P0

## 2. Purpose
Create and manage application users, credentials, and user types/roles for the tenant.

## 3. Preconditions
- Admin session; user types defined

## 4. Layout and UI
- User list grid
- Form: username, password, user type, profile fields, active flag
- Link to Menu To User for permissions

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Username | username | text | yes | — | unique | — | always | Login id |
| Password | password | password | yes on create | — | policy in new system | — | create/reset | Hash at rest |
| User type | user_type / utype | select | yes | General | — | user_type | always | Drives dashboard |
| Name / contact | name, phone, email | text | no | — | email format | — | always | |
| Active | is_active | flag | no | yes | — | — | always | |

## 6. Actions and flows
- CRUD users; reset password; assign menus (separate screen)
- Disable user instead of delete when referenced

## 7. Business rules
- Super Admin / Admin bypass granular menus
- Others require menu_to_user grants
- New system: hashed passwords, optional MFA, audit log

## 8. Data requirements
- User, UserType, MenuToUser, LoginAudit

## 9. Integrations
- None required

## 10. Permissions
- Admin only for user admin

## 11. Reports / exports
- Login logs (separate menu)

## 12. Edge cases
- Last admin disable; duplicate username

## 13. Acceptance criteria
- Created user can log in with company key
- Role determines landing dashboard
- Disabled user cannot authenticate

## 14. Legacy reference
- URL: `settings/NewMaster/Users`
- Related: `settings/MenuToUser/listall`, `settings/NewMaster/user_settings`
- Tables: `users`, `user_type`, `menu_to_user`, `user_settings`

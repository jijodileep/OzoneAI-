# Menu To User (Permissions)

## 1. Identity
- Module / area: Settings / User Settings
- Page type: Settings
- Primary users / roles: Admin
- Entry points: Menu "Menu To User" → `settings/MenuToUser/listall`
- Priority for rebuild: P0

## 2. Purpose
Grant per-user menu visibility and action flags (read/write/add/delete/cancel/dashboard).

## 3. Preconditions
- Users and menus exist

## 4. Layout and UI
- Select user → matrix of menus with checkboxes for is_read, is_write, is_add, is_delete, is_cancel, is_dashboard
- Save replaces grants for that user

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| User | user_id | select | yes | — | — | users | always | |
| Menu | menu_id | row | yes | — | — | menus | always | |
| Read | is_read | checkbox | no | 0 | — | — | always | |
| Write | is_write | checkbox | no | 0 | — | — | always | Edit/save |
| Add | is_add | checkbox | no | 1 | — | — | always | |
| Delete | is_delete | checkbox | no | 0 | — | — | always | |
| Cancel | is_cancel | checkbox | no | 1 | — | — | always | |
| Dashboard | is_dashboard | checkbox | no | 0 | — | — | always | |

## 6. Actions and flows
- Load matrix → toggle → save
- Optional copy permissions from another user (recommended new feature)

## 7. Business rules
- Admin/Super Admin ignore matrix (full menus)
- Runtime checks use URL→menu mapping
- New system may prefer role-based templates plus user overrides

## 8. Data requirements
- Menu, MenuToUser, User, MenuToCompany (tenant menu pack)

## 9. Integrations
- None

## 10. Permissions
- Admin only

## 11. Reports / exports
- Optional permission matrix export

## 12. Edge cases
- Orphan menu URLs; user with zero menus

## 13. Acceptance criteria
- Granting write enables edit on that screen
- Revoking read hides menu and blocks route
- Admin still sees all menus

## 14. Legacy reference
- URL: `settings/MenuToUser/listall`
- Models: `MenuToUser_model`, `Menus_model`
- Tables: `menus`, `menu_to_user`, `menu_to_company`

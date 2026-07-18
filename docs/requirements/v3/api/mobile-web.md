# API / App: Mobile Web (Session)

## 1. Identity
- Module / area: Mobile web UI
- Page type: List | Create | API-AJAX (session)
- Primary users / roles: Field sales users on phones
- Entry points: `mobile/Main/*`
- Priority for rebuild: P1

## 2. Purpose
Session-authenticated mobile-optimized screens for sales orders, receipts, customer/item lookup — not a token REST API. New tech may replace with responsive PWA using the Sales/Voucher APIs.

## 3. Preconditions
- Web login session (`MY_Controller`)

## 4. Layout and UI
- Mobile-friendly sales order form
- Receipt form
- Lists: orders, customers, vouchers, item-wise order register
- AJAX DataTables JSON feeds

## 5. Fields (detailed)
Align with Sales / Receipt fields; simplified mobile layout:
- Customer search, item search/barcode, qty/rate, save
- Receipt: party, amount, ageing selection

## 6. Actions and flows
- `sales` / `newsales` views → `save()`
- `receipt` → `savereceipt()`
- Lookup endpoints: `getProducts`, `getcustomer`, `getitembyId`, etc.

## 7. Business rules
- Same posting rules as web Sales/Vouchers
- Session auth only (CSRF protection required in new system)

## 8. Data requirements
- Sale, Voucher, Customer, Item

## 9. Integrations
- None beyond core

## 10. Permissions
- Session user menu permissions

## 11. Reports / exports
- Order list / itemwise register views

## 12. Edge cases
- `mobile/index.php` is a view fragment mis-placed as controller — do not port as controller
- `pendingorder` raw print_r — fix as JSON

## 13. Acceptance criteria
- Logged-in mobile user can create order and see it in web register
- Receipt updates outstanding
- Unauthenticated users redirected to login

## 14. Legacy reference
- Controller: `mobile/Main.php` (extends `MY_Controller`, not REST_Controller)
- View fragment: `mobile/index.php`

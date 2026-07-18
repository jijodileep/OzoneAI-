# API: Ecommerce Storefront

## 1. Identity
- Module / area: API / Ecommerce
- Page type: API
- Primary users / roles: End customers / storefront app
- Entry points: `restapi/ecom/{Item,Masters,User,Sales}` (and `api/ecom/*` duplicates)
- Priority for rebuild: P1

## 2. Purpose
Public/customer APIs for catalog browsing, home content, customer registration/login, and order (sales) creation.

## 3. Preconditions
- Published items; delivery methods; **proper auth** (legacy MD5 + no token — replace)

## 4. Layout and UI
- N/A (consumed by storefront/mobile app)

## 5. Fields (detailed)
| Endpoint | Method | Request | Response |
|----------|--------|---------|----------|
| categorylist | GET | — | categories |
| allitems | GET | paging/filters | item list |
| subitem | POST | category_id | items in category |
| itemdetails | POST | id | item detail |
| itemlist | POST | barcode | item match |
| sliders / offers | GET | — | banners/promos |
| userreg | POST | name, email, phone, password | user |
| login | POST | username, password | user/session |
| config | POST | devid | bootstrap items/customers (legacy; redesign) |
| sales/create | POST | order header/lines | order result |

## 6. Actions and flows
- Browse → register/login → place order → confirmation

## 7. Business rules
- Only online/active items
- Passwords hashed (argon2/bcrypt); JWT sessions
- Orders map to Sale with channel=`ecommerce`
- Consolidate single-tenant hardcoded DB to multi-tenant

## 8. Data requirements
- Item, Category, Customer/User, Slider, Offer, Sale, DeliveryMethod, Faq

## 9. Integrations
- Payment gateway (new — not in legacy), email/SMS OTP optional

## 10. Permissions
- Public catalog; authenticated checkout; admin content separate

## 11. Reports / exports
- Order reports in admin

## 12. Edge cases
- Duplicate email/phone on register; stock shortage at checkout

## 13. Acceptance criteria
- Anonymous user browses categories/items
- Register/login secure
- Order creates sale and reduces stock
- Duplicate `Item`/`Item_h` classes not carried forward

## 14. Legacy reference
- `restapi/ecom/*`, `api/ecom/*`
- Admin UI: [ecommerce/storefront-admin.md](../ecommerce/storefront-admin.md)

# Ecommerce Admin (FAQ & Delivery Methods)

## 1. Identity
- Module / area: Ecommerce
- Page type: List | Create | Edit | Settings
- Primary users / roles: Admin / content managers
- Entry points: `ecommerce/Faq`, `ecommerce/Delivery_method`
- Priority for rebuild: P2

## 2. Purpose
Maintain storefront content (FAQs) and delivery method options. Customer catalog/login APIs are separate (see api/ecommerce.md).

## 3. Preconditions
- Ecommerce channel enabled

## 4. Layout and UI
- FAQ CRUD: question, answer, published
- Delivery method form by type

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Question / Answer | question, answer | text/html | yes | — | non-empty | — | FAQ | Legacy table mapping unclear (`cost_centre`) — redesign entity |
| Published | published | flag | no | yes | — | — | FAQ | |
| Delivery method fields | name, type, rates… | form | yes | — | — | delivery_method | delivery | Expand from live form |

## 6. Actions and flows
- Publish FAQ → visible via storefront API/content
- Configure delivery methods used at checkout (new checkout to be designed)

## 7. Business rules
- Only published FAQs public
- Delivery methods selectable at order time in new storefront

## 8. Data requirements
- Faq, DeliveryMethod (clean schema; do not reuse unrelated tables)

## 9. Integrations
- Storefront API

## 10. Permissions
- Admin content menus

## 11. Reports / exports
- None required

## 12. Edge cases
- Empty FAQ list; unpublished leakage

## 13. Acceptance criteria
- Published FAQ returned by public content API
- Delivery methods listable for checkout design

## 14. Legacy reference
- Controllers: `ecommerce/Faq.php`, `Delivery_method.php`
- APIs: `restapi/ecom/*`

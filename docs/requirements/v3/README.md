# v3 Requirements Pack — New-Tech Rebuild

This folder contains **one requirements file per screen/API** extracted from the legacy CodeIgniter app in `v3/`. Use these specs to rebuild in any modern stack.

## Start here

1. [INDEX.md](INDEX.md) — full inventory of menu pages + API list  
2. [_template.md](_template.md) — standard structure every page file follows  
3. [_inventory.csv](_inventory.csv) — machine-readable menu extract from `testdb.sql`  
4. New-system architecture: [../../architecture/tech-stack.md](../../architecture/tech-stack.md)

## Deep-dive specs (read first for core domain)

| Area | File |
|------|------|
| Login | [auth/login.md](auth/login.md) |
| Dashboards | [dashboard/](dashboard/) |
| Sales engine | [transaction/sales.md](transaction/sales.md) |
| Purchase engine | [transaction/purchase.md](transaction/purchase.md) |
| Vouchers | [transaction/vouchers.md](transaction/vouchers.md) |
| Quotation | [transaction/quotation.md](transaction/quotation.md) |
| Item master | [master/item-master.md](master/item-master.md) |
| Godown | [master/godown.md](master/godown.md) |
| Ledgers | [accounts_master/ledgers.md](accounts_master/ledgers.md) |
| Stock journal / physical | [stock/](stock/) |
| Users & permissions | [settings/users.md](settings/users.md), [settings/menu-to-user.md](settings/menu-to-user.md) |
| POS | [pos/pos-billing.md](pos/pos-billing.md) |
| CRM | [crm/enquiry-register.md](crm/enquiry-register.md) |
| HR | [hr/employee.md](hr/employee.md) |
| Clinic | [clinic/patients.md](clinic/patients.md), [clinic/booking.md](clinic/booking.md) |
| Education / Hotel / Gold / Church / Production / Ecommerce | matching folders |
| APIs | [api/](api/) |

## Waves

- **Wave 1 (P0):** Auth, dashboards, masters, sales/purchase/vouchers, stock, users/settings  
- **Wave 2 (P1):** POS, reports, CRM, HR, analytics  
- **Wave 3 (P1/P2):** Vertical packs  
- **Wave 4 (P1):** REST/mobile APIs  

## Conventions for implementers

- Specs are **technology-agnostic**. Legacy CI paths are only in §14.  
- Prefer **role + permission** model over copying `menu_to_user` bit-for-bit, but preserve the same capabilities.  
- Replace plaintext passwords, open REST endpoints, and hardcoded tenant branches with secure, configurable design.  
- Financial edits should remain audit-safe (reverse + repost or equivalent).  

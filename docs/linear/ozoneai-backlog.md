# OzoneAI — Full Linear backlog (E1–E7)

Use this with [ozoneai-backlog.csv](ozoneai-backlog.csv). Create each row as a Linear issue under project **OzoneAI**.

## E1 — Platform scaffold (P0)

- [ ] **E1.1** Docker Compose: Postgres catalog + tenants, Redis, MinIO, Meilisearch  
- [ ] **E1.2** ASP.NET Core solution skeleton (Clean Architecture) — `/health` 200  
- [ ] **E1.3** Catalog DB EF migrations + Companies table  
- [x] **E1.4** React Vite + Ant Design shell  
- [ ] **E1.5** CI GitHub Actions: build API + web  
- [ ] **E1.6** Cursor rules locked to tech-stack.md  

## E2 — Super Admin (P0/P1)

- [ ] **E2.1** Super Admin auth (platform users)  
- [ ] **E2.2** Create tenant → `ozone_t_{key}` DB + seed admin  
- [ ] **E2.3** Tenant list: last used, active users  
- [ ] **E2.4** Tenant detail + suspend/activate  
- [ ] **E2.5** Nightly job: active users + last-used rollup  
- [ ] **E2.6** Impersonate tenant (audited)  

## E3 — Auth & tenancy (P0/P1)

- [ ] **E3.1** Company-key login → JWT  
- [ ] **E3.2** Tenant connection factory (Write/Read)  
- [ ] **E3.3** Role-based menus  
- [ ] **E3.4** Device binding for mobile  

## E4 — ERP Core Wave 1 (P0/P1)

- [ ] **E4.1** Items CRUD — `docs/requirements/v3/master/item-master.md`  
- [ ] **E4.2** Customers / Suppliers  
- [ ] **E4.3** Ledgers / COA — `accounts_master/ledgers.md`  
- [ ] **E4.4** Godown — `master/godown.md`  
- [ ] **E4.5** Sales post + list — `transaction/sales.md`  
- [ ] **E4.6** Purchase post + list — `transaction/purchase.md`  
- [ ] **E4.7** Vouchers P/R/J/C — `transaction/vouchers.md`  
- [ ] **E4.8** Stock journal / physical  
- [ ] **E4.9** Users + menu permissions  
- [ ] **E4.10** Meilisearch index items/customers  

## E5 — Legacy migration (P0)

- [ ] **E5.1** Migration wizard UI + package checklist  
- [ ] **E5.2** Connect legacy MySQL + discover counts  
- [ ] **E5.3** Migrate masters / parties / items  
- [ ] **E5.4** Migrate all selected txn packages  
- [ ] **E5.5** Validation report (counts + money)  
- [ ] **E5.6** Cutover + Meilisearch rebuild  
- [ ] **E5.7** Pilot one real company end-to-end  

## E6 — Field Flutter (P1)

- [ ] **E6.1** Flutter flavors: sales / van / customer  
- [ ] **E6.2** Offline sync scopes + SQLite  
- [ ] **E6.3** Van sales offline invoice → sync  
- [ ] **E6.4** Sales collections + GPS check-in  
- [ ] **E6.5** Customer app: catalog + order + statement  

## E7 — AI P0 (P1/P2)

- [ ] **E7.1** Semantic Kernel + Ollama config switch  
- [ ] **E7.2** AI-01 Smart typeahead  
- [ ] **E7.3** AI-02 Ask your data  
- [ ] **E7.4** AI-03 Help RAG  
- [ ] **E7.5** AI-04 OCR → draft purchase  
- [ ] **E7.6** AI-05 Basic anomaly alerts  

## Success gate (demo)

After E1–E3 + E4.1 + E4.5: create tenant → login → create item → post sale → list register. Then start E5 pilot.

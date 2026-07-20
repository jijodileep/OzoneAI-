# OzoneAI — local / Docker credentials (dev only)

Do not use these values in production. Change all passwords before any shared or public deploy.

## Apps

| Service | URL | Username | Password |
|---------|-----|----------|----------|
| Admin UI (Vite) | http://localhost:5173/ | — | — |
| Super Admin login | http://localhost:5173/super-admin/login | `superadmin` | `ChangeMe!123` |
| Tenant login | http://localhost:5173/login | company `demo` / `admin` | `ChangeMe!123` |
| Demo tenant admin (seed) | tenant DB `ozone_t_demo` | `admin` / `demo@example.com` | `ChangeMe!123` |
| Super Admin email (seed) | catalog | `superadmin@example.com` | — |
| API | http://localhost:5080/ | — | — |
| API health | http://localhost:5080/health | — | — |

## Infrastructure (Docker Compose)

| Service | Host URL | Username / key | Password / secret |
|---------|----------|----------------|-------------------|
| PostgreSQL (catalog + tenants) | `localhost:5433` | `ozone` | `ozone_dev_password` |
| Redis | `localhost:6380` | — (no auth) | — |
| MinIO API | http://localhost:9000 | `ozoneai` | `ozoneai_dev_password` |
| MinIO console | http://localhost:9001 | `ozoneai` | `ozoneai_dev_password` |
| Meilisearch | http://localhost:7700 | master key | `ozoneai_meili_dev_master_key_change_me` |

### Postgres databases

| Database | Purpose |
|----------|---------|
| `ozone_catalog` | Platform catalog (companies, plans, Super Admin) |
| `ozone_t_demo` | Seeded demo tenant |

Connection string (host):

```
Host=localhost;Port=5433;Database=ozone_catalog;Username=ozone;Password=ozone_dev_password
```

## API / JWT (dev)

| Setting | Value |
|---------|-------|
| JWT signing key | `OzoneAI-dev-signing-key-change-me-32chars!` |
| JWT issuer | `OzoneAI` |
| JWT audience | `OzoneAI` |

## Seed / provisioning defaults (`appsettings.json`)

| Setting | Value |
|---------|-------|
| `PlatformAuth:SeedUsername` | `superadmin` |
| `PlatformAuth:SeedPassword` | `ChangeMe!123` |
| `PlatformAuth:SeedDisplayName` | `Platform Super Admin` |
| `TenantProvisioning:DbUsername` | `ozone` |
| `TenantProvisioning:DbPassword` | `ozone_dev_password` |
| `TenantProvisioning:DefaultHost` | `localhost` |
| `TenantProvisioning:DefaultPort` | `5433` |

## Quick start

```bash
docker compose up -d --build
cd apps/ozone_web && npm run dev
```

Sources: `docker-compose.yml`, `src/OzoneAI.Api/appsettings.json`, `src/OzoneAI.Api/Program.cs` seed.

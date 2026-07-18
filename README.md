# OzoneAI

AI-integrated, multi-tenant ERP (open source / self-hosted).

- **GitHub:** https://github.com/jijodileep/OzoneAI-
- **Legacy reference:** `C:\xampp\htdocs\Ozonelatest` (CodeIgniter v3 — do not develop new features there)
- **Architecture:** [docs/architecture/tech-stack.md](docs/architecture/tech-stack.md)
- **Screen requirements:** [docs/requirements/v3/README.md](docs/requirements/v3/README.md)
- **Linear backlog:** [docs/linear/README.md](docs/linear/README.md)

## Stack (summary)

| Layer | Choice |
|-------|--------|
| API | ASP.NET Core 9 |
| Admin UI | React + TypeScript + Ant Design + AG Grid |
| Mobile | Flutter (customer / sales / van) |
| DB | PostgreSQL — **one database per company** + `ozone_catalog` |
| Search | Meilisearch (Docker) |
| AI | Semantic Kernel + Ollama / OpenAI-compatible |
| Workflow | Linear + Cursor + GitHub |

## Quick start (after E1)

```bash
docker compose up -d
cd src && dotnet run --project OzoneAI.Api --urls http://localhost:5080
```

| Service | Host URL |
|---------|----------|
| API health | `GET http://localhost:5080/health` |
| Postgres catalog | `localhost:5433` (container 5432) |
| Redis | `localhost:6380` |
| MinIO | `http://localhost:9000` (console `:9001`) |
| Meilisearch | `http://localhost:7700` |

## Branches

- `main` — release / protected
- `develop` — integration
- `feat/*` — one Linear issue per branch

## License

Proprietary — OzoneAI / CPZSAS product line.

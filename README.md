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
cd src/OzoneAI.Api && dotnet run
```

Health: `GET http://localhost:5080/health`

## Branches

- `main` — release / protected
- `develop` — integration
- `feat/*` — one Linear issue per branch

## License

Proprietary — OzoneAI / CPZSAS product line.

# OzoneAI Linear backlog

Project in Linear: **OzoneAI**  
GitHub: https://github.com/jijodileep/OzoneAI-

## Files

| File | Purpose |
|------|---------|
| [ozoneai-backlog.csv](ozoneai-backlog.csv) | Import / API source for all epics E1–E7 |
| [ozoneai-backlog.md](ozoneai-backlog.md) | Human-readable backlog |
| [create-linear-issues.ps1](create-linear-issues.ps1) | Creates issues via Linear GraphQL API |

## Import into Linear (no API key)

Linear’s UI CSV import expects its own columns. Prefer one of:

1. **API script (preferred):** set `LINEAR_API_KEY` and run `create-linear-issues.ps1` (creates all E1–E7 issues with descriptions).  
2. **Manual from markdown:** open [ozoneai-backlog.md](ozoneai-backlog.md) and create issues in project **OzoneAI** (titles already match the plan).  
3. Connect **GitHub integration** to `jijodileep/OzoneAI-`.

> Status: backlog files are complete in-repo. Bulk create via API requires `LINEAR_API_KEY` in the environment (not available at bootstrap time).

## Create via API

```powershell
$env:LINEAR_API_KEY = "lin_api_..."
# Optional: $env:LINEAR_TEAM_ID = "uuid-of-team"
.\docs\linear\create-linear-issues.ps1
```

## Workflow

`Linear issue` → Cursor Agent → `feat/ISSUE-key-...` branch → GitHub PR → CI → merge → Linear Done.

<#
.SYNOPSIS
  Create OzoneAI backlog issues in Linear from ozoneai-backlog.csv

.NOTES
  Requires: $env:LINEAR_API_KEY
  Optional: $env:LINEAR_TEAM_ID (UUID). If missing, uses the first team from the API.
#>
$ErrorActionPreference = "Stop"
$apiKey = $env:LINEAR_API_KEY
if (-not $apiKey) {
  Write-Error "Set LINEAR_API_KEY first. Example: `$env:LINEAR_API_KEY='lin_api_...'"
}

$csvPath = Join-Path $PSScriptRoot "ozoneai-backlog.csv"
$rows = Import-Csv $csvPath

function Invoke-Linear($query, $variables = @{}) {
  $body = @{ query = $query; variables = $variables } | ConvertTo-Json -Depth 20 -Compress
  $headers = @{
    Authorization = $apiKey
    "Content-Type" = "application/json"
  }
  $resp = Invoke-RestMethod -Uri "https://api.linear.app/graphql" -Method Post -Headers $headers -Body $body
  if ($resp.errors) {
    throw ($resp.errors | ConvertTo-Json -Depth 5)
  }
  return $resp.data
}

# Resolve team
$teamId = $env:LINEAR_TEAM_ID
if (-not $teamId) {
  $data = Invoke-Linear "query { teams { nodes { id name } } }"
  $team = $data.teams.nodes | Select-Object -First 1
  if (-not $team) { throw "No Linear teams found for this API key." }
  $teamId = $team.id
  Write-Host "Using team: $($team.name) ($teamId)"
}

$priorityMap = @{ P0 = 2; P1 = 3; P2 = 4 } # Linear: 1 urgent, 2 high, 3 medium, 4 low

$created = 0
foreach ($r in $rows) {
  $title = "[$($r.IssueKey)] $($r.Title)"
  $desc = @"
**Epic:** $($r.Epic)
**Issue key:** $($r.IssueKey)
**Team (planned):** $($r.Team)
**Spec:** $($r.SpecPath)

## Acceptance criteria
$($r.AcceptanceCriteria)

## Description
$($r.Description)

## Workflow
Branch: ``feat/$($r.IssueKey.ToLower().Replace('.','-'))-short-name``
PR must mention this issue title key for Linear↔GitHub linking.
"@

  $prio = 3
  if ($priorityMap.ContainsKey($r.Priority)) { $prio = $priorityMap[$r.Priority] }

  $mutation = @'
mutation IssueCreate($input: IssueCreateInput!) {
  issueCreate(input: $input) {
    success
    issue { id identifier title url }
  }
}
'@

  $vars = @{
    input = @{
      teamId = $teamId
      title = $title
      description = $desc
      priority = $prio
    }
  }

  try {
    $result = Invoke-Linear $mutation $vars
    if ($result.issueCreate.success) {
      Write-Host "Created $($result.issueCreate.issue.identifier) — $($result.issueCreate.issue.title)"
      $created++
    }
  }
  catch {
    Write-Warning "Failed $($r.IssueKey): $_"
  }
  Start-Sleep -Milliseconds 200
}

Write-Host "Done. Created $created / $($rows.Count) issues."
